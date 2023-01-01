namespace PivotViewer.Core.DeepZoom;

public class TileManager
{
	private readonly TileLoader tileLoader;
	private readonly Dictionary<int, LevelData> levelDataMap = new();

	public TileManager(TileLoader tileLoader)
	{
		this.tileLoader = tileLoader;

		tileLoader.TileLoaded += OnTileLoaded;
	}

	public IDeepZoomTileSource? TileSource { get; set; }

	public int LevelOverride { get; set; } = -1;

	public event EventHandler? TileLoaded;

	public void Update(RectangleF frame, RectangleF clip, IDeepZoomCanvas canvas)
	{
		if (TileSource is null)
			return;

		// determine the level to load
		var level = LevelOverride >= 0
			? LevelOverride
			: GetIdealLevel(frame);
		level = Math.Clamp(level, 0, TileSource.MaxLevel);

		// determine the best frame for the image to fill
		var aspectFillFrame = AspectFillImageIntoFrame(frame, TileSource.ItemSize);

		// determine visible frame
		var pixelClipped = aspectFillFrame;
		pixelClipped.Intersect(clip);

		// determine the normalized (0.0 - 1.0) clipping frame that will render tiles
		var normClipped = new RectangleF(
			(pixelClipped.X - aspectFillFrame.X) / aspectFillFrame.Width,
			(pixelClipped.Y - aspectFillFrame.Y) / aspectFillFrame.Height,
			pixelClipped.Width / aspectFillFrame.Width,
			pixelClipped.Height / aspectFillFrame.Height);

		// pre-populate the level data with all the re-usable information
		var levelData = PrepareLevelData(level);

		// get all the tiles that wil be visible
		var tiles = TileSource.GetTiles(level, normClipped);

		for (var y = tiles.Top; y < tiles.Bottom; y++)
		{
			for (var x = tiles.Left; x < tiles.Right; x++)
			{
				var tile = levelData.Tiles[new(x, y)];
				var info = tileLoader.GetTileInfo(tile);

				// tile is not yet loaded, so load now
				if (info is null || info.State == TileLoadState.Unloaded)
				{
					tileLoader.Request(tile);
					continue;
				}

				// tile is currently being loaded, so skip as we will get it the next time
				if (info.State == TileLoadState.Loading)
					continue;

				// tile failed to load, so not much we can do here
				if (info.State == TileLoadState.Error)
					continue;

				// something is horribly wrong so just skip
				if (info.State != TileLoadState.Loaded || info.Image is null)
					continue;

				var pixelBounds = new RectangleF(
					tile.Bounds.X * aspectFillFrame.Width + aspectFillFrame.X,
					tile.Bounds.Y * aspectFillFrame.Height + aspectFillFrame.Y,
					tile.Bounds.Width * aspectFillFrame.Width,
					tile.Bounds.Height * aspectFillFrame.Height);

				// tile is loaded and we can draw
				canvas.DrawTile(info.Image, tile.CropRect, pixelBounds);
			}
		}
	}

	private LevelData PrepareLevelData(int level)
	{
		if (TileSource is null)
			throw new InvalidOperationException("Tile source was null.");

		// get the tiles for this level
		if (levelDataMap.TryGetValue(level, out var levelData))
			return levelData;

		// the level was not found at all, populate the level data from the tile source
		levelData = new LevelData
		{
			Level = level,
			TileCount = TileSource.GetTileCount(level),
		};

		foreach (var tile in TileSource.GetImageTiles(level))
		{
			levelData.Tiles[tile.Coordinates] =
				new Tile
				{
					Level = level,
					X = tile.Coordinates.X,
					Y = tile.Coordinates.Y,
					Uri = tile.Uri,
					CropRect = tile.CropRect,
					Bounds = TileSource.GetTileBounds(level, tile.Coordinates.X, tile.Coordinates.Y),
				};
		}

		levelDataMap[level] = levelData;

		return levelData;
	}

	public static RectangleF AspectFillImageIntoFrame(RectangleF frame, SizeF imageSize)
	{
		var imageFrame = new RectangleF(PointF.Empty, imageSize);

		var imageAspect = imageFrame.Width / imageFrame.Height;
		var frameAspect = frame.Width / frame.Height;

		if (frameAspect < imageAspect)
		{
			// the image is "wider" than the frame
			imageFrame.Height = frame.Height;
			imageFrame.Width = imageFrame.Height * imageAspect;
		}
		else
		{
			// the image is "taller" than the frame
			imageFrame.Width = frame.Width;
			imageFrame.Height = imageFrame.Width / imageAspect;
		}

		imageFrame.X = frame.X + (frame.Width - imageFrame.Width) / 2f;
		imageFrame.Y = frame.Y + (frame.Height - imageFrame.Height) / 2f;

		return imageFrame;
	}

	private void OnTileLoaded(object? sender, EventArgs e)
	{
		TileLoaded?.Invoke(this, e);
	}

	private static int GetIdealLevel(RectangleF frame)
	{
		var longest = Math.Max(frame.Width, frame.Height);
		return (int)Math.Ceiling(Math.Log2(longest));
	}

	class LevelData
	{
		public int Level { get; set; }

		public Dictionary<Point, Tile> Tiles { get; set; } = new();

		public Size TileCount { get; set; }
	}
}
