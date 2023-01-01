namespace PivotViewer.Core.DeepZoom;

public class DeepZoomImageTileSource : IDeepZoomTileSource
{
	public DeepZoomImageTileSource(string itemId, int itemWidth, int itemHeight, int maxLevel, int tileSize, int tileOverlap, string tileBaseUri, string tileFileFormat)
	{
		ItemId = itemId;
		ItemSize = new Size(itemWidth, itemHeight);
		ItemAspectRatio =
			itemWidth == 0 || itemHeight == 0
				? 1.0f
				: (float)itemWidth / itemHeight;

		MaxLevel = maxLevel;

		TileSize = new Size(tileSize, tileSize);
		TileOverlap = tileOverlap;
		TileBaseUri = tileBaseUri;
		TileFileFormat = tileFileFormat;
	}

	public string ItemId { get; }

	public Size ItemSize { get; }

	/// <summary>
	/// Width / Height
	/// </summary>
	public float ItemAspectRatio { get; }

	public int MaxLevel { get; }

	public Size TileSize { get; }

	public int TileOverlap { get; }

	public string TileBaseUri { get; }

	public string TileFileFormat { get; }

	public double GetScale(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		return 1.0 / Math.Pow(2, MaxLevel - level);
	}

	public Size GetScaledItemSize(int level)
	{
		var scale = GetScale(level);

		var scaled = new Size(
			(int)Math.Ceiling(ItemSize.Width * scale),
			(int)Math.Ceiling(ItemSize.Height * scale));

		return scaled;
	}

	public Size GetTileCount(int level)
	{
		var scale = GetScale(level);

		var tiles = new Size(
			(int)Math.Ceiling(scale * ItemSize.Width / TileSize.Width),
			(int)Math.Ceiling(scale * ItemSize.Height / TileSize.Height));

		return tiles;
	}

	public Size GetItemCount(int level) => new(1, 1);

	public RectangleF GetTileBounds(int level, int x, int y)
	{
		var scaled = GetScaledItemSize(level);
		var pixelBounds = GetTilePixelBounds(level, x, y);

		var rect = new RectangleF(
			(float)pixelBounds.X / scaled.Width,
			(float)pixelBounds.Y / scaled.Height,
			(float)pixelBounds.Width / scaled.Width,
			(float)pixelBounds.Height / scaled.Height);

		return rect;
	}

	public Rectangle GetTilePixelBounds(int level, int x, int y)
	{
		// scale the image for this level
		var scaled = GetScaledItemSize(level);

		// determine base pixel bounds of the tile
		double pixelW = TileSize.Width;
		double pixelH = TileSize.Height;
		double pixelX = x * TileSize.Width;
		double pixelY = y * TileSize.Height;

		// adjust bounds for overlap
		// left
		if (x > 0)
		{
			pixelX -= TileOverlap;
			pixelW += TileOverlap;
		}
		// top
		if (y > 0)
		{
			pixelY -= TileOverlap;
			pixelH += TileOverlap;
		}
		// right
		var remainingW = scaled.Width - pixelX;
		if (remainingW >= TileSize.Width)
			pixelW += TileOverlap;
		else
			pixelW = remainingW;
		// bottom
		var remainingH = scaled.Height - pixelY;
		if (remainingH >= TileSize.Height)
			pixelH += TileOverlap;
		else
			pixelH = remainingH;

		var rect = new Rectangle((int)pixelX, (int)pixelY, (int)pixelW, (int)pixelH);

		return rect;
	}

	public IList<DeepZoomImageTile> GetImageTiles(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		var scale = GetScale(level);
		var levelHeight = Math.Ceiling(ItemSize.Width / ItemAspectRatio * scale);
		var levelWidth = Math.Ceiling(ItemSize.Width * scale);

		var hslices = (int)Math.Ceiling(levelWidth / TileSize.Width);
		var vslices = (int)Math.Ceiling(levelHeight / TileSize.Height);

		var fileNames = new List<DeepZoomImageTile>(hslices * vslices);
		for (var i = 0; i < hslices; i++)
		{
			for (var j = 0; j < vslices; j++)
			{
				fileNames.Add(new DeepZoomImageTile(
					$"{TileBaseUri}/{level}/{i}_{j}.{TileFileFormat}",
					default,
					new Point(i, j)));
			}
		}
		return fileNames;
	}

	public Rectangle GetTiles(int level, RectangleF frame)
	{
		var tl = GetTile(level, frame.Location, Size.Empty);
		var br = GetTile(level, frame.Location + frame.Size, new(-1, -1));

		var tiles = Rectangle.FromLTRB(tl.X, tl.Y, br.X + 1, br.Y + 1);

		return tiles;
	}

	private Point GetTile(int level, PointF point, Size seamAdjustment)
	{
		var scaled = GetScaledItemSize(level);

		point = new PointF(
			point.X * scaled.Width,
			point.Y * scaled.Height);

		if (point.X % 1 == 0)
			point.X += seamAdjustment.Width;
		if (point.Y % 1 == 0)
			point.Y += seamAdjustment.Height;

		var tile = new Point(
			(int)Math.Floor(point.X / TileSize.Width),
			(int)Math.Floor(point.Y / TileSize.Height));

		return tile;
	}
}
