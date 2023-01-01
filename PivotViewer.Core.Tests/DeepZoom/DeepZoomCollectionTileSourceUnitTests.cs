namespace PivotViewer.Core.Tests;

public class DeepZoomCollectionTileSourceUnitTests
{
	[Theory]
	[InlineData(0, 0, 0)]
	[InlineData(1, 1, 0)]
	[InlineData(2, 0, 1)]
	[InlineData(3, 1, 1)]
	[InlineData(4, 2, 0)]
	[InlineData(5, 3, 0)]
	[InlineData(6, 2, 1)]
	[InlineData(10, 0, 3)]
	[InlineData(11, 1, 3)]
	[InlineData(15, 3, 3)]
	[InlineData(31, 7, 3)]
	public void TileCoordinatesAreCorrect(int itemN, int x, int y)
	{
		var tileSource = new DeepZoomCollectionTileSource("<anything>", itemN, 640, 426, 7, 256, "TestData\\collection-dz_deepzoom\\collection-dz_files", "jpg");

		Assert.Equal(new Point(x, y), tileSource.TileCoordinates);
	}

	[Theory]
	// "ideal" - perfect fit
	[InlineData(0, 128, 128, 7, 0, 0)]
	[InlineData(0, 128, 128, 7, 1, 0)]
	[InlineData(0, 128, 128, 7, 2, 0)]
	[InlineData(0, 128, 128, 7, 3, 0)]
	[InlineData(0, 128, 128, 7, 4, 0)]
	[InlineData(0, 128, 128, 7, 5, 0)]
	[InlineData(0, 128, 128, 7, 6, 0)]
	[InlineData(0, 128, 128, 7, 7, 0)]
	// "ideal" - large image needs an extra scale down
	[InlineData(0, 256, 256, 7, 0, 1)]
	[InlineData(0, 256, 256, 7, 1, 1)]
	[InlineData(0, 256, 256, 7, 2, 1)]
	[InlineData(0, 256, 256, 7, 3, 1)]
	[InlineData(0, 256, 256, 7, 4, 1)]
	[InlineData(0, 256, 256, 7, 5, 1)]
	[InlineData(0, 256, 256, 7, 6, 1)]
	[InlineData(0, 256, 256, 7, 7, 1)]
	// "ideal" - more levels means an extra scale up
	[InlineData(0, 128, 128, 10, 0, -3)]
	[InlineData(0, 128, 128, 10, 1, -3)]
	[InlineData(0, 128, 128, 10, 2, -3)]
	[InlineData(0, 128, 128, 10, 3, -3)]
	[InlineData(0, 128, 128, 10, 4, -3)]
	[InlineData(0, 128, 128, 10, 5, -3)]
	[InlineData(0, 128, 128, 10, 6, -3)]
	[InlineData(0, 128, 128, 10, 7, -3)]
	// "0"
	[InlineData(0, 640, 426, 7, 0, 3)]
	[InlineData(0, 640, 426, 7, 1, 3)]
	[InlineData(0, 640, 426, 7, 2, 3)]
	[InlineData(0, 640, 426, 7, 3, 3)]
	[InlineData(0, 640, 426, 7, 4, 3)]
	[InlineData(0, 640, 426, 7, 5, 3)]
	[InlineData(0, 640, 426, 7, 6, 3)]
	[InlineData(0, 640, 426, 7, 7, 3)]
	// "7"
	[InlineData(7, 454, 480, 7, 0, 2)]
	[InlineData(7, 454, 480, 7, 1, 2)]
	[InlineData(7, 454, 480, 7, 2, 2)]
	[InlineData(7, 454, 480, 7, 3, 2)]
	[InlineData(7, 454, 480, 7, 4, 2)]
	[InlineData(7, 454, 480, 7, 5, 2)]
	[InlineData(7, 454, 480, 7, 6, 2)]
	[InlineData(7, 454, 480, 7, 7, 2)]

	public void GetScaleIsCorrect(int itemN, int width, int height, int maxLevel, int level, int scaleAdjustment)
	{
		var tileSource = new DeepZoomCollectionTileSource("<anything>", itemN, width, height, maxLevel, 256, "<fake>", "jpg");

		var expectedScale = 1.0 / (1 << (maxLevel - level + scaleAdjustment));

		Assert.Equal(expectedScale, tileSource.GetScale(level));
	}

	[Theory]
	// "ideal" - perfect fit
	[InlineData(0, 128, 128, 256, 7, 0, 1, 1)]
	[InlineData(0, 128, 128, 256, 7, 1, 2, 2)]
	[InlineData(0, 128, 128, 256, 7, 2, 4, 4)]
	[InlineData(0, 128, 128, 256, 7, 3, 8, 8)]
	[InlineData(0, 128, 128, 256, 7, 4, 16, 16)]
	[InlineData(0, 128, 128, 256, 7, 5, 32, 32)]
	[InlineData(0, 128, 128, 256, 7, 6, 64, 64)]
	[InlineData(0, 128, 128, 256, 7, 7, 128, 128)]
	// "ideal" - large image needs an extra scale down
	[InlineData(0, 256, 256, 256, 7, 0, 1, 1)]
	[InlineData(0, 256, 256, 256, 7, 1, 2, 2)]
	[InlineData(0, 256, 256, 256, 7, 2, 4, 4)]
	[InlineData(0, 256, 256, 256, 7, 3, 8, 8)]
	[InlineData(0, 256, 256, 256, 7, 4, 16, 16)]
	[InlineData(0, 256, 256, 256, 7, 5, 32, 32)]
	[InlineData(0, 256, 256, 256, 7, 6, 64, 64)]
	[InlineData(0, 256, 256, 256, 7, 7, 128, 128)]
	// "ideal" - more levels means an extra scale up
	[InlineData(0, 128, 128, 256, 10, 0, 1, 1)]
	[InlineData(0, 128, 128, 256, 10, 1, 2, 2)]
	[InlineData(0, 128, 128, 256, 10, 2, 4, 4)]
	[InlineData(0, 128, 128, 256, 10, 3, 8, 8)]
	[InlineData(0, 128, 128, 256, 10, 4, 16, 16)]
	[InlineData(0, 128, 128, 256, 10, 5, 32, 32)]
	[InlineData(0, 128, 128, 256, 10, 6, 64, 64)]
	[InlineData(0, 128, 128, 256, 10, 7, 128, 128)]
	// "0"
	[InlineData(0, 640, 426, 256, 7, 0, 1, 1)]
	[InlineData(0, 640, 426, 256, 7, 1, 2, 1)]
	[InlineData(0, 640, 426, 256, 7, 2, 3, 2)]
	[InlineData(0, 640, 426, 256, 7, 3, 5, 4)]
	[InlineData(0, 640, 426, 256, 7, 4, 10, 7)]
	[InlineData(0, 640, 426, 256, 7, 5, 20, 14)]
	[InlineData(0, 640, 426, 256, 7, 6, 40, 27)]
	[InlineData(0, 640, 426, 256, 7, 7, 80, 54)]
	// "7"
	[InlineData(7, 454, 480, 256, 7, 0, 1, 1)]
	[InlineData(7, 454, 480, 256, 7, 1, 2, 2)]
	[InlineData(7, 454, 480, 256, 7, 2, 4, 4)]
	[InlineData(7, 454, 480, 256, 7, 3, 8, 8)]
	[InlineData(7, 454, 480, 256, 7, 4, 15, 15)]
	[InlineData(7, 454, 480, 256, 7, 5, 29, 30)]
	[InlineData(7, 454, 480, 256, 7, 6, 57, 60)]
	[InlineData(7, 454, 480, 256, 7, 7, 114, 120)]

	public void GetScaledItemSizeIsCorrect(int itemN, int width, int height, int tileSize, int maxLevel, int level, int scaledW, int scaledH)
	{
		var tileSource = new DeepZoomCollectionTileSource("<anything>", itemN, width, height, maxLevel, tileSize, "<fake>", "jpg");

		Assert.Equal(new Size(scaledW, scaledH), tileSource.GetScaledItemSize(level));
	}

	[Theory]
	// "0"
	[InlineData(0, 640, 426, 256)]
	[InlineData(1, 640, 426, 256)]
	[InlineData(2, 640, 426, 256)]
	[InlineData(3, 640, 426, 256)]
	[InlineData(4, 640, 426, 256)]
	[InlineData(5, 640, 426, 256)]
	[InlineData(6, 640, 426, 256)]
	[InlineData(7, 640, 426, 256)]
	public void GetTileCountReturnsCorrectCount(int level, int width, int height, int tileSize)
	{
		var tileSource = new DeepZoomCollectionTileSource("<anything>", 0, width, height, 7, tileSize, "Fake_files", "jpg");

		var tileCount = tileSource.GetTileCount(level);

		Assert.Equal(new SizeF(1, 1), tileCount);
	}

	[Theory]
	// "0"
	[InlineData(0, 640, 426, 256, 256)]
	[InlineData(1, 640, 426, 256, 128)]
	[InlineData(2, 640, 426, 256, 64)]
	[InlineData(3, 640, 426, 256, 32)]
	[InlineData(4, 640, 426, 256, 16)]
	[InlineData(5, 640, 426, 256, 8)]
	[InlineData(6, 640, 426, 256, 4)]
	[InlineData(7, 640, 426, 256, 2)]
	public void GetItemCountReturnsCorrectCount(int level, int width, int height, int tileSize, int itemCount)
	{
		var tileSource = new DeepZoomCollectionTileSource("<anything>", 0, width, height, 7, tileSize, "Fake_files", "jpg");

		var count = tileSource.GetItemCount(level);

		Assert.Equal(new Size(itemCount, itemCount), count);
	}

	[Theory]
	[InlineData(0, 0, 0, 1, 1, "\\0\\0_0.jpg")]
	[InlineData(1, 0, 0, 2, 1, "\\1\\0_0.jpg")]
	[InlineData(2, 0, 0, 3, 2, "\\2\\0_0.jpg")]
	[InlineData(3, 0, 0, 5, 4, "\\3\\0_0.jpg")]
	[InlineData(4, 0, 0, 10, 7, "\\4\\0_0.jpg")]
	[InlineData(5, 0, 0, 20, 14, "\\5\\0_0.jpg")]
	[InlineData(6, 0, 0, 40, 27, "\\6\\0_0.jpg")]
	[InlineData(7, 0, 0, 80, 54, "\\7\\0_0.jpg")]
	public void GetImageTilesReturnsCorrectImage(int level, int x, int y, int width, int height, string file)
	{
		var expectedBase = Path.GetFullPath($"TestData/collection-dz_deepzoom/collection-dz_files/{level}/");

		var tileSource = new DeepZoomCollectionTileSource("0", 0, 640, 426, 7, 256, "TestData\\collection-dz_deepzoom\\collection-dz_files", "jpg");

		var img = Assert.Single(tileSource.GetImageTiles(level));

		Assert.StartsWith(expectedBase, Path.GetFullPath(img.Uri));
		Assert.EndsWith(file, Path.GetFullPath(img.Uri));
		Assert.True(File.Exists(img.Uri));
		Assert.Equal(new Rectangle(x, y, width, height), img.CropRect);
	}

	[Theory]
	[InlineData(0, 0, 0, 0, 1, 1, "Fake_files/0/0_0.jpg")]
	[InlineData(0, 1, 0, 0, 2, 2, "Fake_files/1/0_0.jpg")]
	[InlineData(0, 2, 0, 0, 4, 4, "Fake_files/2/0_0.jpg")]
	[InlineData(0, 3, 0, 0, 8, 8, "Fake_files/3/0_0.jpg")]
	[InlineData(0, 4, 0, 0, 16, 16, "Fake_files/4/0_0.jpg")]
	[InlineData(0, 5, 0, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	[InlineData(0, 6, 0, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	[InlineData(1, 0, 1, 0, 1, 1, "Fake_files/0/0_0.jpg")]
	[InlineData(1, 1, 2, 0, 2, 2, "Fake_files/1/0_0.jpg")]
	[InlineData(1, 2, 4, 0, 4, 4, "Fake_files/2/0_0.jpg")]
	[InlineData(1, 3, 8, 0, 8, 8, "Fake_files/3/0_0.jpg")]
	[InlineData(1, 4, 16, 0, 16, 16, "Fake_files/4/0_0.jpg")]
	[InlineData(1, 5, 32, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	[InlineData(1, 6, 32, 0, 32, 32, "Fake_files/5/0_0.jpg")]
	public void GetImageTilesReturnsFakeImage(int itemN, int level, int x, int y, int width, int height, string file)
	{
		var tileSource = new DeepZoomCollectionTileSource("<anything>", itemN, 512, 512, 5, 256, "Fake_files", "jpg");

		var img = Assert.Single(tileSource.GetImageTiles(level));

		Assert.Equal(file, img.Uri);
		Assert.Equal(new Rectangle(x, y, width, height), img.CropRect);
	}
}
