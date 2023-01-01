namespace PivotViewer.Core.DeepZoom;

public class TileManager
{
	private readonly TileLoader tileLoader;
	private readonly Dictionary<int, LevelData> levelData = new();

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

		// get the tiles for this level
		if (!levelData.TryGetValue(level, out var data))
		{
			// the level was not found at all, populate the level data from the tile source
			data = new LevelData
			{
				Level = level,
				TileCount = TileSource.GetTileCount(level),
			};
			foreach (var tile in TileSource.GetImageTiles(level))
			{
				data.Tiles.Add(
					tile.Coordinates,
					new Tile
					{
						Level = level,
						X = tile.Coordinates.X,
						Y = tile.Coordinates.Y,
						Uri = tile.Uri,
						CropRect = tile.CropRect
					});
			}
			levelData.Add(level, data);
		}

		// TODO: determine the tiles that fit in the frame

		//var itemTileSize = data.ItemTileSize; // 64, 64
		//var tileSize = TileSource.TileSize; // 256

		//var normalizedFrame = new RectangleF(0, 0, frame.Width, frame.Height); // 0, 0, 1000, 1000
		//var imageFrame = AspectFillImageIntoFrame(normalizedFrame, TileSource.ItemSize); // x, y, 640, 480

		//var tilesX = (int)Math.Ceiling((double)TileSource.ItemSize.Width / TileSource.TileSize);
		//var tilesY = (int)Math.Ceiling((double)TileSource.ItemSize.Height / TileSource.TileSize);


		var normalizedFrame = new RectangleF(0, 0, 1, 1);

		//TileSource.GetTileBounds



		for (int y = 0; y < data.TileCount.Height; y++)
		{
			for (int x = 0; x < data.TileCount.Width; x++)
			{

			}
		}


		//

		// the level data was populated/queued, determine if any tiles are loaded
		foreach (var tilePair in data.Tiles)
		{
			var tile = tilePair.Value;
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
			if (info.State != TileLoadState.Loaded)
				continue;

			// tile is loaded and we can draw
			canvas.DrawTile(tile);
		}
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
