namespace PivotViewer.Core.Tests;

public class TileManagerUnitTests
{
	//private readonly IFileLoader fileLoader;
	//private readonly TileCache tileCache;

	//public TileManagerUnitTests()
	//{
	//	fileLoader = new TestFileLoader();
	//	tileCache = new TileCache();
	//}

	[Fact]
	public void Test()
	{
		var tileSource = new DeepZoomCollectionTileSource("0", 0, 64, 64, 5, 32, "TestData\\collection-dz_deepzoom\\collection-dz_files", "jpg");

		var cache = new TileCache<ITileLoadingInfo>(10);
		var imageLoader = new TestImageLoader();
		var tileLoader = new TileLoader(imageLoader, cache, 1);
		var manager = new TileManager(tileLoader)
		{
			TileSource = tileSource,
			LevelOverride = 0,
		};

		var canvas = new TestDeepZoomCanvas();

		manager.Update(new RectangleF(0, 0, 100, 100), new RectangleF(0, 0, 100, 100), canvas);
	}

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

	class TestImageLoader : IImageLoader
	{
		public int LoadCount { get; set; }

		public Task<object?> LoadAsync(string uri, CancellationToken cancellationToken = default)
		{
			LoadCount++;

			return Task.FromResult<object?>(uri);
		}
	}

	class TestDeepZoomCanvas : IDeepZoomCanvas
	{
		public void DrawTile(Tile tile)
		{
		}
	}
}
