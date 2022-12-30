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

		TileSize = tileSize;
		TileOverlap = tileOverlap;
		TileBaseUri = tileBaseUri;
		TileFileFormat = tileFileFormat;
	}

	public string ItemId { get; }

	public Size ItemSize { get; }

	public float ItemAspectRatio { get; }

	public int MaxLevel { get; }

	public int TileSize { get; }

	public int TileOverlap { get; }

	public string TileBaseUri { get; }

	public string TileFileFormat { get; }

	public double GetScale(int level) =>
		1.0 / Math.Pow(2, MaxLevel - level);

	public Size GetTileCount(int level)
	{
		var scale = GetScale(level);

		var tiles = new Size(
			(int)Math.Ceiling(scale * ItemSize.Width / TileSize),
			(int)Math.Ceiling(scale * ItemSize.Height / TileSize));

		return tiles;
	}

	public Size GetItemCount(int level) => new(1, 1);

	public RectangleF GetTileBounds(int level, int x, int y)
	{
		// scale the image for this level
		var scale = GetScale(level);
		var scaledWidth = ItemSize.Width * scale;
		var scaledHeight = ItemSize.Height * scale;

		// deterine the normalized tile size (0.0 - 1.0) for a full tile
		var normTileW = TileSize / scaledWidth;
		var normTileH = TileSize / scaledHeight;

		// determine the normalized tile size for (X, Y)
		var width = Math.Min(1, (scaledWidth / TileSize) - x);
		var height = Math.Min(1, (scaledHeight / TileSize) - y);

		var rect = new RectangleF(
			(float)(x * normTileW),
			(float)(y * normTileH),
			(float)(width * normTileW),
			(float)(height * normTileH));

		return rect;
	}

	public IList<DeepZoomImageTile> GetImageTiles(int level)
	{
		var scale = GetScale(level);
		var levelHeight = Math.Ceiling(ItemSize.Width / ItemAspectRatio * scale);
		var levelWidth = Math.Ceiling(ItemSize.Width * scale);

		var hslices = (int)Math.Ceiling(levelWidth / TileSize);
		var vslices = (int)Math.Ceiling(levelHeight / TileSize);

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
}
