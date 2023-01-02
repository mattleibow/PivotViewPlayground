namespace DeepZoom.Core.Tests;

public class DeepZoomImageTileSourceUnitTests
{
	[Theory]
	[InlineData(128, 128, 64, 7, 0, 1, 1)]
	[InlineData(128, 128, 64, 7, 1, 2, 2)]
	[InlineData(128, 128, 64, 7, 2, 4, 4)]
	[InlineData(128, 128, 64, 7, 3, 8, 8)]
	[InlineData(128, 128, 64, 7, 4, 16, 16)]
	[InlineData(128, 128, 64, 7, 5, 32, 32)]
	[InlineData(128, 128, 64, 7, 6, 64, 64)]
	[InlineData(128, 128, 64, 7, 7, 128, 128)]

	public void GetScaledItemSizeIsCorrect(int width, int height, int tileSize, int maxLevel, int level, int scaledW, int scaledH)
	{
		var tileSource = new DeepZoomImageTileSource("<anything>", width, height, maxLevel, tileSize, 0, "<fake>", "jpg");

		Assert.Equal(new Size(scaledW, scaledH), tileSource.GetScaledItemSize(level));
	}

	[Theory]
	// "0"
	[InlineData(640, 426, 10, 0, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 1, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 2, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 3, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 4, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 5, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 6, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 7, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 8, 254, 1f, 1f)]
	[InlineData(640, 426, 10, 9, 254, 2f, 1f)]
	[InlineData(640, 426, 10, 10, 254, 3f, 2f)]
	// "7"
	[InlineData(454, 480, 9, 0, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 1, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 2, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 3, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 4, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 5, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 6, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 7, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 8, 254, 1f, 1f)]
	[InlineData(454, 480, 9, 9, 254, 2f, 2f)]
	// 
	[InlineData(64, 64, 6, 0, 64, 1f, 1f)]
	[InlineData(64, 64, 6, 1, 64, 1f, 1f)]
	[InlineData(64, 64, 6, 2, 64, 1f, 1f)]
	[InlineData(64, 64, 6, 3, 64, 1f, 1f)]
	[InlineData(64, 64, 6, 4, 64, 1f, 1f)]
	[InlineData(64, 64, 6, 5, 64, 1f, 1f)]
	[InlineData(64, 64, 6, 6, 64, 1f, 1f)]
	// 
	[InlineData(64, 64, 6, 0, 16, 1f, 1f)]
	[InlineData(64, 64, 6, 1, 16, 1f, 1f)]
	[InlineData(64, 64, 6, 2, 16, 1f, 1f)]
	[InlineData(64, 64, 6, 3, 16, 1f, 1f)]
	[InlineData(64, 64, 6, 4, 16, 1f, 1f)]
	[InlineData(64, 64, 6, 5, 16, 2f, 2f)]
	[InlineData(64, 64, 6, 6, 16, 4f, 4f)]
	// 
	[InlineData(64, 32, 6, 0, 16, 1f, 1f)]
	[InlineData(64, 32, 6, 1, 16, 1f, 1f)]
	[InlineData(64, 32, 6, 2, 16, 1f, 1f)]
	[InlineData(64, 32, 6, 3, 16, 1f, 1f)]
	[InlineData(64, 32, 6, 4, 16, 1f, 1f)]
	[InlineData(64, 32, 6, 5, 16, 2f, 1f)]
	[InlineData(64, 32, 6, 6, 16, 4f, 2f)]
	// 
	[InlineData(32, 64, 6, 0, 16, 1f, 1f)]
	[InlineData(32, 64, 6, 1, 16, 1f, 1f)]
	[InlineData(32, 64, 6, 2, 16, 1f, 1f)]
	[InlineData(32, 64, 6, 3, 16, 1f, 1f)]
	[InlineData(32, 64, 6, 4, 16, 1f, 1f)]
	[InlineData(32, 64, 6, 5, 16, 1f, 2f)]
	[InlineData(32, 64, 6, 6, 16, 2f, 4f)]
	public void GetTileCountReturnsCorrectCount(int itemWidth, int itemHeight, int maxLevel, int level, int tileSize, float tilesX, float tilesY)
	{
		var tileSource = new DeepZoomImageTileSource("<anything>", itemWidth, itemHeight, maxLevel, tileSize, 0, "<anything>", "jpg");

		var tileCount = tileSource.GetTileCount(level);

		Assert.Equal(new SizeF(tilesX, tilesY), tileCount);
	}

	[Theory]
	// 64, 0/6
	[InlineData(64, 64, 6, 0, 16, 0, 0, 0f, 0f, 1f, 1f)]
	// 64, 1/6
	[InlineData(64, 64, 6, 1, 16, 0, 0, 0f, 0f, 1f, 1f)]
	// 64, 4/6
	[InlineData(64, 64, 6, 4, 16, 0, 0, 0f, 0f, 1f, 1f)]
	// 64, 5/6
	[InlineData(64, 64, 6, 5, 16, 0, 0, 0f, 0f, 0.5f, 0.5f)]
	[InlineData(64, 64, 6, 5, 16, 1, 0, 0.5f, 0f, 0.5f, 0.5f)]
	[InlineData(64, 64, 6, 5, 16, 0, 1, 0f, 0.5f, 0.5f, 0.5f)]
	[InlineData(64, 64, 6, 5, 16, 1, 1, 0.5f, 0.5f, 0.5f, 0.5f)]
	// 64, 6/6
	[InlineData(64, 64, 6, 6, 16, 0, 0, 0f, 0f, 0.25f, 0.25f)]
	[InlineData(64, 64, 6, 6, 16, 1, 1, 0.25f, 0.25f, 0.25f, 0.25f)]
	// 64x32, 4/6
	[InlineData(64, 32, 6, 4, 16, 0, 0, 0f, 0f, 1f, 1f)]
	// 64x32, 5/6
	[InlineData(64, 32, 6, 5, 16, 0, 0, 0f, 0f, 0.5f, 1f)]
	[InlineData(64, 32, 6, 5, 16, 1, 0, 0.5f, 0f, 0.5f, 1f)]
	// 32x64, 4/6
	[InlineData(32, 64, 6, 4, 16, 0, 0, 0f, 0f, 1f, 1f)]
	// 32x64, 5/6
	[InlineData(32, 64, 6, 5, 16, 0, 0, 0f, 0f, 1f, 0.5f)]
	[InlineData(32, 64, 6, 5, 16, 0, 1, 0f, 0.5f, 1f, 0.5f)]
	// "0"
	[InlineData(640, 426, 10, 0, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 1, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 2, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 3, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 4, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 5, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 6, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 7, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 8, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(640, 426, 10, 9, 254, 0, 0, 0f, 0f, 0.79375f, 1f)]
	[InlineData(640, 426, 10, 9, 254, 1, 0, 0.79375f, 0f, 0.20625f, 1f)]
	[InlineData(640, 426, 10, 10, 254, 0, 0, 0f, 0f, 0.396875f, 0.59624413145539906103286384976526f)]
	[InlineData(640, 426, 10, 10, 254, 1, 0, 0.396875f, 0f, 0.396875f, 0.59624413145539906103286384976526f)]
	[InlineData(640, 426, 10, 10, 254, 2, 0, 0.79375f, 0f, 0.20625f, 0.59624413145539906103286384976526f)]
	[InlineData(640, 426, 10, 10, 254, 0, 1, 0f, 0.59624413145539906103286384976526f, 0.396875f, 0.40375586854460093896713615023474f)]
	[InlineData(640, 426, 10, 10, 254, 1, 1, 0.396875f, 0.59624413145539906103286384976526f, 0.396875f, 0.40375586854460093896713615023474f)]
	[InlineData(640, 426, 10, 10, 254, 2, 1, 0.79375f, 0.59624413145539906103286384976526f, 0.20625f, 0.40375586854460093896713615023474f)]
	// "7"
	[InlineData(454, 480, 9, 0, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 1, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 2, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 3, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 4, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 5, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 6, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 7, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 8, 254, 0, 0, 0f, 0f, 1f, 1f)]
	[InlineData(454, 480, 9, 9, 254, 0, 0, 0f, 0f, 0.55947136563876651982378854625551f, 0.52916666666666666666666666666667f)]
	public void GetTileBoundsReturnsCorrectBounds(int iW, int iH, int maxLevel, int level, int tileSize, int x, int y, float eX, float eY, float eW, float eH)
	{
		var tileSource = new DeepZoomImageTileSource("<anything>", iW, iH, maxLevel, tileSize, 0, "Fake_files", "jpg");

		var bounds = tileSource.GetTileBounds(level, x, y);

		Assert.Equal(new RectangleF(eX, eY, eW, eH), bounds);
	}

	[Theory]
	// "0"
	[InlineData(640, 426, 10, 0, 254, 0, 0, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 1, 254, 0, 0, 0, 0, 2, 1)]
	[InlineData(640, 426, 10, 2, 254, 0, 0, 0, 0, 3, 2)]
	[InlineData(640, 426, 10, 3, 254, 0, 0, 0, 0, 5, 4)]
	[InlineData(640, 426, 10, 4, 254, 0, 0, 0, 0, 10, 7)]
	[InlineData(640, 426, 10, 5, 254, 0, 0, 0, 0, 20, 14)]
	[InlineData(640, 426, 10, 6, 254, 0, 0, 0, 0, 40, 27)]
	[InlineData(640, 426, 10, 7, 254, 0, 0, 0, 0, 80, 54)]
	[InlineData(640, 426, 10, 8, 254, 0, 0, 0, 0, 160, 107)]
	[InlineData(640, 426, 10, 9, 254, 0, 0, 0, 0, 255, 213)]
	[InlineData(640, 426, 10, 9, 254, 1, 0, 253, 0, 67, 213)]
	[InlineData(640, 426, 10, 10, 254, 0, 0, 0, 0, 255, 255)]
	[InlineData(640, 426, 10, 10, 254, 1, 0, 253, 0, 256, 255)]
	[InlineData(640, 426, 10, 10, 254, 2, 0, 507, 0, 133, 255)]
	[InlineData(640, 426, 10, 10, 254, 0, 1, 0, 253, 255, 173)]
	[InlineData(640, 426, 10, 10, 254, 1, 1, 253, 253, 256, 173)]
	[InlineData(640, 426, 10, 10, 254, 2, 1, 507, 253, 133, 173)]
	// "7"
	[InlineData(454, 480, 9, 0, 254, 0, 0, 0, 0, 1, 1)]
	[InlineData(454, 480, 9, 1, 254, 0, 0, 0, 0, 2, 2)]
	[InlineData(454, 480, 9, 2, 254, 0, 0, 0, 0, 4, 4)]
	[InlineData(454, 480, 9, 3, 254, 0, 0, 0, 0, 8, 8)]
	[InlineData(454, 480, 9, 4, 254, 0, 0, 0, 0, 15, 15)]
	[InlineData(454, 480, 9, 5, 254, 0, 0, 0, 0, 29, 30)]
	[InlineData(454, 480, 9, 6, 254, 0, 0, 0, 0, 57, 60)]
	[InlineData(454, 480, 9, 7, 254, 0, 0, 0, 0, 114, 120)]
	[InlineData(454, 480, 9, 8, 254, 0, 0, 0, 0, 227, 240)]
	[InlineData(454, 480, 9, 9, 254, 0, 0, 0, 0, 255, 255)]
	[InlineData(454, 480, 9, 9, 254, 1, 0, 253, 0, 201, 255)]
	[InlineData(454, 480, 9, 9, 254, 0, 1, 0, 253, 255, 227)]
	[InlineData(454, 480, 9, 9, 254, 1, 1, 253, 253, 201, 227)]
	public void GetTilePixelBoundsReturnsCorrectBounds(int iW, int iH, int maxLevel, int level, int tileSize, int x, int y, int eX, int eY, int eW, int eH)
	{
		var tileSource = new DeepZoomImageTileSource("<anything>", iW, iH, maxLevel, tileSize, 1, "Fake_files", "jpg");

		var bounds = tileSource.GetTilePixelBounds(level, x, y);

		Assert.Equal(new Rectangle(eX, eY, eW, eH), bounds);
	}

	[Theory]
	// "0"
	[InlineData(640, 426, 10, 0, 254, 0, 0, 1, 1, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 0, 254, 0, 0, .5f, .5f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 0, 254, .5f, .5f, .5f, .5f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 0, 254, .3f, .3f, .3f, .3f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 6, 254, 0, 0, 1, 1, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 6, 254, 0, 0, .5f, .5f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 6, 254, .5f, .5f, .5f, .5f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 6, 254, .3f, .3f, .3f, .3f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 9, 254, 0f, 0f, .3f, 1f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 9, 254, 0f, 0f, .79375f, 1f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 9, 254, .5f, 0f, .2f, 1f, 0, 0, 1, 1)]
	[InlineData(640, 426, 10, 9, 254, .5f, 0f, .5f, 1f, 0, 0, 2, 1)]
	[InlineData(640, 426, 10, 9, 254, .8f, 0f, 0.15f, 1f, 1, 0, 1, 1)]
	public void GetTilesReturnsCorrectBounds(int iW, int iH, int maxLevel, int level, int tileSize, float fX, float fY, float fW, float fH, int tX, int tY, int tW, int tH)
	{
		var tileSource = new DeepZoomImageTileSource("<anything>", iW, iH, maxLevel, tileSize, 0, "Fake_files", "jpg");

		var bounds = tileSource.GetTiles(level, new(fX, fY, fW, fH));

		Assert.Equal(new Rectangle(tX, tY, tW, tH), bounds);
	}

	[Theory]
	// "0"
	[InlineData("0", 640, 426, 10, 0, 1)]
	[InlineData("0", 640, 426, 10, 1, 1)]
	[InlineData("0", 640, 426, 10, 2, 1)]
	[InlineData("0", 640, 426, 10, 3, 1)]
	[InlineData("0", 640, 426, 10, 4, 1)]
	[InlineData("0", 640, 426, 10, 5, 1)]
	[InlineData("0", 640, 426, 10, 6, 1)]
	[InlineData("0", 640, 426, 10, 7, 1)]
	[InlineData("0", 640, 426, 10, 8, 1)]
	[InlineData("0", 640, 426, 10, 9, 2)]
	[InlineData("0", 640, 426, 10, 10, 6)]
	// "7"
	[InlineData("7", 454, 480, 9, 0, 1)]
	[InlineData("7", 454, 480, 9, 1, 1)]
	[InlineData("7", 454, 480, 9, 2, 1)]
	[InlineData("7", 454, 480, 9, 3, 1)]
	[InlineData("7", 454, 480, 9, 4, 1)]
	[InlineData("7", 454, 480, 9, 5, 1)]
	[InlineData("7", 454, 480, 9, 6, 1)]
	[InlineData("7", 454, 480, 9, 7, 1)]
	[InlineData("7", 454, 480, 9, 8, 1)]
	[InlineData("7", 454, 480, 9, 9, 4)]
	public void GetImageTilesReturnsCorrectImages(string itemId, int itemWidth, int itemHeight, int maxLevel, int level, int expectedCount)
	{
		var tileSource = new DeepZoomImageTileSource(itemId, itemWidth, itemHeight, maxLevel, 254, 0, $"TestData/collection-dz_deepzoom/{itemId}_files", "jpg");

		var images = tileSource.GetImageTiles(level);

		Assert.Equal(expectedCount, images.Count);
		Assert.Distinct(images);

		foreach (var img in images)
		{
			Assert.True(File.Exists(img.Uri));
			Assert.Equal(Rectangle.Empty, img.CropRect);
		}
	}
}
