namespace PivotViewer.Core.Tests;

public class TileManagerUnitTests
{
	[Theory]
	[InlineData(0, 0, 100, 100, 100, 100, 0, 0, 100, 100)]
	[InlineData(25, 75, 100, 100, 100, 100, 25, 75, 100, 100)]
	[InlineData(0, 0, 100, 50, 100, 100, 0, -25, 100, 100)]
	[InlineData(25, 75, 100, 50, 100, 100, 25, 50, 100, 100)]
	[InlineData(0, 0, 100, 100, 100, 50, -50, 0, 200, 100)]
	[InlineData(25, 75, 100, 100, 100, 50, -25, 75, 200, 100)]
	public void AspectFillImageIntoFrame(float fX, float fY, float fW, float fH, float iW, float iH, float rX, float rY, float rW, float rH)
	{
		var frame = new RectangleF(fX, fY, fW, fH);
		var size = new SizeF(iW, iH);

		var result = TileManager.AspectFillImageIntoFrame(frame, size);

		Assert.Equal(new RectangleF(rX, rY, rW, rH), result);
	}

	//[Theory]
	//[InlineData(0, 0, 0, 0, 1, 1, "Fake_files/0/0_0.jpg")]
	//[InlineData(0, 1, 0, 0, 2, 2, "Fake_files/1/0_0.jpg")]
	//[InlineData(0, 2, 0, 0, 4, 4, "Fake_files/2/0_0.jpg")]
	//[InlineData(0, 3, 0, 0, 8, 8, "Fake_files/3/0_0.jpg")]
	//[InlineData(0, 4, 0, 0, 16, 16, "Fake_files/4/0_0.jpg")]
	//[InlineData(0, 5, 0, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	//[InlineData(0, 6, 0, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	//[InlineData(1, 0, 1, 0, 1, 1, "Fake_files/0/0_0.jpg")]
	//[InlineData(1, 1, 2, 0, 2, 2, "Fake_files/1/0_0.jpg")]
	//[InlineData(1, 2, 4, 0, 4, 4, "Fake_files/2/0_0.jpg")]
	//[InlineData(1, 3, 8, 0, 8, 8, "Fake_files/3/0_0.jpg")]
	//[InlineData(1, 4, 16, 0, 16, 16, "Fake_files/4/0_0.jpg")]
	//[InlineData(1, 5, 32, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	//[InlineData(1, 6, 32, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	//public void GetImageTilesReturnsFakeImage(int itemN, int level, int x, int y, int width, int height, string file)
	//{
	//	var tileSource = new DeepZoomCollectionTileSource("<anything>", itemN, 512, 512, 5, 256, "Fake_files", "jpg");

	//	var img = Assert.Single(tileSource.GetImageTiles(level));

	//	Assert.Equal(file, img.Uri);
	//	Assert.Equal(new Rectangle(x, y, width, height), img.CropRect);
	//}

	[Theory]
	// 
	[InlineData(64, 64, 32, 6, 0, -1, 4)]
	[InlineData(64, 64, 32, 6, 0, 0, 1)]
	[InlineData(64, 64, 32, 6, 0, 1, 1)]
	[InlineData(64, 64, 32, 6, 0, 2, 1)]
	[InlineData(64, 64, 32, 6, 0, 3, 1)]
	[InlineData(64, 64, 32, 6, 0, 4, 1)]
	[InlineData(64, 64, 32, 6, 0, 5, 1)]
	[InlineData(64, 64, 32, 6, 0, 6, 4)]
	[InlineData(64, 64, 32, 6, 0, 7, 4)]
	[InlineData(64, 64, 32, 6, 0, 8, 4)]
	// "0"
	[InlineData(640, 426, 254, 10, 1, -1, 6)]
	[InlineData(640, 426, 254, 10, 1, 0, 1)]
	[InlineData(640, 426, 254, 10, 1, 1, 1)]
	[InlineData(640, 426, 254, 10, 1, 2, 1)]
	[InlineData(640, 426, 254, 10, 1, 3, 1)]
	[InlineData(640, 426, 254, 10, 1, 4, 1)]
	[InlineData(640, 426, 254, 10, 1, 5, 1)]
	[InlineData(640, 426, 254, 10, 1, 6, 1)]
	[InlineData(640, 426, 254, 10, 1, 7, 1)]
	[InlineData(640, 426, 254, 10, 1, 8, 1)]
	[InlineData(640, 426, 254, 10, 1, 9, 2)]
	[InlineData(640, 426, 254, 10, 1, 10, 6)]
	// "7"
	[InlineData(454, 480, 254, 9, 1, -1, 4)]
	[InlineData(454, 480, 254, 9, 1, 0, 1)]
	[InlineData(454, 480, 254, 9, 1, 1, 1)]
	[InlineData(454, 480, 254, 9, 1, 2, 1)]
	[InlineData(454, 480, 254, 9, 1, 3, 1)]
	[InlineData(454, 480, 254, 9, 1, 4, 1)]
	[InlineData(454, 480, 254, 9, 1, 5, 1)]
	[InlineData(454, 480, 254, 9, 1, 6, 1)]
	[InlineData(454, 480, 254, 9, 1, 7, 1)]
	[InlineData(454, 480, 254, 9, 1, 8, 1)]
	[InlineData(454, 480, 254, 9, 1, 9, 4)]
	public async Task ImageTileSourceUpdateLoadsCorrectNumberOfTiles(int iW, int iH, int tileSize, int maxLevel, int overlap, int level, int expectedTiles)
	{
		// the tile source
		var tileSource = new DeepZoomImageTileSource("<anything>", iW, iH, maxLevel, tileSize, overlap, "Test_files", "jpg");

		// the UI info
		var frame = new RectangleF(0, 0, iW, iH);
		var clip = new RectangleF(0, 0, iW, iH);
		var canvas = new TestDeepZoomCanvas();

		// the tile manager
		var imageLoader = new TestImageLoader();
		var tileCache = new TileCache<ITileLoadingInfo>(expectedTiles + 1);
		var tileLoader = new TileLoader(imageLoader, tileCache, 1);
		var tileManager = new TileManager(tileLoader)
		{
			TileSource = tileSource,
			LevelOverride = level,
		};

		// fake "render loop"
		await RenderAllTiles(tileManager, expectedTiles, () => tileManager.Update(frame, clip, canvas));

		// make sure all 4 tiles were loaded at full res
		Assert.Equal(expectedTiles, imageLoader.LoadedUris.Count);
		Assert.Equal(expectedTiles, tileCache.Count);
		Assert.Equal(expectedTiles, canvas.DrawnTiles.Count);
	}

	private static Task RenderAllTiles(TileManager tileManager, int expectedTiles, Action update)
	{
		// fake "render loop"
		var tcs = new TaskCompletionSource();
		var tilesLoaded = 0;
		tileManager.TileLoaded += (s, e) =>
		{
			tilesLoaded++;
			if (tilesLoaded == expectedTiles)
			{
				// trigger a "refresh" when all tiles are loaded
				update();

				tcs.SetResult();
			}
		};

		// request the initial "frame"
		update();

		return tcs.Task;
	}

	class TestImageLoader : IImageLoader
	{
		public List<string> LoadedUris { get; } = new();

		public Task<object?> LoadAsync(string uri, CancellationToken cancellationToken = default)
		{
			LoadedUris.Add(uri);

			return Task.FromResult<object?>(uri);
		}
	}

	class TestDeepZoomCanvas : IDeepZoomCanvas
	{
		public List<(string Image, RectangleF Source, RectangleF Destination)> DrawnTiles { get; } = new();

		public void DrawTile(object image, RectangleF source, RectangleF destination)
		{
			DrawnTiles.Add((image.ToString()!, source, destination));
		}
	}
}
