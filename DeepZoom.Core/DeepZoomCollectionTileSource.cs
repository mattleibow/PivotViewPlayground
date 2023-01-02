namespace DeepZoom.Core;

public class DeepZoomCollectionTileSource : ITileSource
{
	public DeepZoomCollectionTileSource(string itemId, int itemN, int itemWidth, int itemHeight, int maxLevel, int tileSize, string tileBaseUri, string tileFileFormat)
	{
		ItemId = itemId;

		ItemN = itemN;
		ItemSize = new Size(itemWidth, itemHeight);

		MaxLevel = maxLevel;

		TileCoordinates = GetMortonPoint(ItemN);

		TileSize = new Size(tileSize, tileSize);
		TileBaseUri = tileBaseUri;
		TileFileFormat = tileFileFormat;
	}

	public string ItemId { get; }

	public int ItemN { get; }

	public Size ItemSize { get; }

	public Size TileSize { get; }

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

	public Size GetScaledItemSize(int level)
	{
		var scale = GetScale(level);

		var scaled = new Size(
			(int)Math.Ceiling(ItemSize.Width * scale),
			(int)Math.Ceiling(ItemSize.Height * scale));

		return scaled;
	}

	public Size GetTileCount(int level) => new(1, 1);

	public Size GetItemCount(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		var itemSize = TileSize / (1 << level);

		return itemSize;
	}

	public RectangleF GetTileBounds(int level, int x, int y) => new(x, y, 1, 1);

	public IList<DeepZoomTileSourceTile> GetImageTiles(int level)
	{
		level = Math.Clamp(level, 0, MaxLevel);

		var itemSize = 1 << level;
		var numItems = TileSize / itemSize;

		var itemCol = TileCoordinates.X;
		var itemRow = TileCoordinates.Y;

		var tileCol = (int)Math.Floor((double)itemCol / numItems.Width);
		var tileRow = (int)Math.Floor((double)itemRow / numItems.Height);

		var uri = $"{TileBaseUri}/{level}/{tileCol}_{tileRow}.{TileFileFormat}";

		var scale = GetScale(level);

		// DZC thumbnails are always >= 1px
		var cropRect = new Rectangle(
			(int)((itemCol % numItems.Width) * itemSize),
			(int)((itemRow % numItems.Height) * itemSize),
			Math.Max(1, (int)Math.Ceiling(ItemSize.Width * scale)),
			Math.Max(1, (int)Math.Ceiling(ItemSize.Height * scale)));

		return new[] { new DeepZoomTileSourceTile(uri, cropRect, default) };
	}

	public Rectangle GetTiles(int level, RectangleF frame) => new(0, 0, 1, 1);

	public static int GetMortonNumber(int x, int y)
	{
		var result = 0;

		for (var i = 0; i < 16; i++)
		{
			result |=
				((x & (1 << i)) << i) |
				((y & (1 << i)) << (i + 1));
		}

		return result;
	}

	public static Point GetMortonPoint(int morton)
	{
		var x = 0;
		var y = 0;

		for (var i = 0; i < 16; i++)
		{
			x |= (morton & (1 << (2 * i))) >> i;
			y |= (morton & (1 << (2 * i + 1))) >> (i + 1);
		}

		return new Point(x, y);
	}
}
