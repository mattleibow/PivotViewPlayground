namespace PivotViewer.Core.DeepZoom;

public class DeepZoomCollectionTileSource : IDeepZoomTileSource
{
	public DeepZoomCollectionTileSource(string itemId, int itemN, int itemWidth, int itemHeight, int maxLevel, int tileSize, string tileBaseUri, string tileFileFormat)
	{
		ItemId = itemId;

		ItemN = itemN;
		ItemSize = new Size(itemWidth, itemHeight);

		MaxLevel = maxLevel;

		TileCoordinates = DeepZoomExtensions.GetMortonPoint(ItemN);

		TileSize = tileSize;
		TileBaseUri = tileBaseUri;
		TileFileFormat = tileFileFormat;
	}

	public string ItemId { get; }

	public int ItemN { get; }

	public Size ItemSize { get; }

	public int TileSize { get; }

	public int MaxLevel { get; }

	public Point TileCoordinates { get; }

	public string TileBaseUri { get; }

	public string TileFileFormat { get; }

	public Size GetItemTileSize(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		var itemSize = 1 << level;

		return new Size(itemSize, itemSize);
	}

	public double GetScale(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		var itemSize = 1 << level;

		var biggest = Math.Max(ItemSize.Width, ItemSize.Height);
		var log = (int)Math.Ceiling(Math.Log2(biggest / itemSize));
		var scale = 1 << log;

		return 1.0 / scale;
	}

	public Size GetTileCount(int level) => new(1, 1);

	public Size GetItemCount(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		var itemSize = TileSize / (1 << level);

		return new(itemSize, itemSize);
	}

	public RectangleF GetTileBounds(int level, int x, int y) => new(x, y, 1, 1);

	public IList<DeepZoomImageTile> GetImageTiles(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		var itemSize = 1 << level;
		var numItems = TileSize / itemSize;

		var itemCol = TileCoordinates.X;
		var itemRow = TileCoordinates.Y;

		var tileCol = (int)Math.Floor((double)itemCol / numItems);
		var tileRow = (int)Math.Floor((double)itemRow / numItems);

		var uri = $"{TileBaseUri}/{level}/{tileCol}_{tileRow}.{TileFileFormat}";

		var scale = GetScale(level);

		// DZC thumbnails are always >= 1px
		var cropRect = new Rectangle(
			(int)((itemCol % numItems) * itemSize),
			(int)((itemRow % numItems) * itemSize),
			Math.Max(1, (int)Math.Ceiling(ItemSize.Width * scale)),
			Math.Max(1, (int)Math.Ceiling(ItemSize.Height * scale)));

		return new[] { new DeepZoomImageTile(uri, cropRect, default) };
	}
}
