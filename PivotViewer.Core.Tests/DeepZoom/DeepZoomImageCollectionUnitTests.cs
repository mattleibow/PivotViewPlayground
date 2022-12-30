namespace PivotViewer.Core.Tests;

public class DeepZoomImageCollectionUnitTests
{
	[Theory]
	[InlineData("TestData\\collection-dz_deepzoom\\collection-dz.dzc")]
	[InlineData("TestData\\conceptcars-Seadragon-21\\conceptcars.dzc")]
	public async Task CanLoadFromFile(string filename)
	{
		var collection = new DeepZoomCollection(filename);
		await collection.LoadAsync();

		Assert.NotNull(collection);
	}

	[Theory]
	// "0"
	[InlineData("0", 0, 1)]
	[InlineData("0", 1, 1)]
	[InlineData("0", 2, 1)]
	[InlineData("0", 3, 1)]
	[InlineData("0", 4, 1)]
	[InlineData("0", 5, 1)]
	[InlineData("0", 6, 1)]
	[InlineData("0", 7, 1)]
	[InlineData("0", 8, 1)]
	[InlineData("0", 9, 2)]
	[InlineData("0", 10, 6)]
	// "7"
	[InlineData("7", 0, 1)]
	[InlineData("7", 1, 1)]
	[InlineData("7", 2, 1)]
	[InlineData("7", 3, 1)]
	[InlineData("7", 4, 1)]
	[InlineData("7", 5, 1)]
	[InlineData("7", 6, 1)]
	[InlineData("7", 7, 1)]
	[InlineData("7", 8, 1)]
	[InlineData("7", 9, 4)]
	public async Task GetFullImageTilesReturnsCorrectImagesForLevels(string itemId, int level, int expectedCount)
	{
		var expectedBase = Path.GetFullPath($"TestData/collection-dz_deepzoom/{itemId}_files/{level}/");

		var collection = new DeepZoomCollection("TestData\\collection-dz_deepzoom\\collection-dz.dzc");
		await collection.LoadAsync();
		await collection.LoadItemAsync(itemId);

		var images = collection.GetFullImageTiles(itemId, level);

		Assert.Equal(expectedCount, images.Count);
		Assert.Distinct(images);

		foreach (var img in images)
		{
			Assert.StartsWith(expectedBase, Path.GetFullPath(img.Uri));
			Assert.True(File.Exists(img.Uri));
			Assert.Equal(Rectangle.Empty, img.CropRect);
		}
	}

	[Theory]
	[InlineData("0", 0, 0, 0, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("0", 1, 0, 0, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("0", 2, 0, 0, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("0", 3, 0, 0, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("0", 4, 0, 0, 10, 7, "\\4\\0_0.jpg")]
	[InlineData("0", 5, 0, 0, 20, 14, "\\5\\0_0.jpg")]
	[InlineData("0", 6, 0, 0, 40, 27, "\\6\\0_0.jpg")]
	[InlineData("0", 7, 0, 0, 80, 54, "\\7\\0_0.jpg")]
	[InlineData("1", 0, 1, 0, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("1", 1, 2, 0, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("1", 2, 4, 0, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("1", 3, 8, 0, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("1", 4, 16, 0, 10, 8, "\\4\\0_0.jpg")]
	[InlineData("1", 5, 32, 0, 20, 15, "\\5\\0_0.jpg")]
	[InlineData("1", 6, 64, 0, 40, 30, "\\6\\0_0.jpg")]
	[InlineData("1", 7, 128, 0, 80, 60, "\\7\\0_0.jpg")]
	[InlineData("2", 0, 0, 1, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("2", 1, 0, 2, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("2", 2, 0, 4, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("2", 3, 0, 8, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("2", 4, 0, 16, 10, 8, "\\4\\0_0.jpg")]
	[InlineData("2", 5, 0, 32, 20, 15, "\\5\\0_0.jpg")]
	[InlineData("2", 6, 0, 64, 40, 30, "\\6\\0_0.jpg")]
	[InlineData("2", 7, 0, 128, 80, 60, "\\7\\0_0.jpg")]
	[InlineData("7", 0, 0, 0, 5, 5, "\\0\\0_0.jpg")]
	[InlineData("7", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("7", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("7", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("7", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("7", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("7", 6, 0, 0, 5, 5, "\\6\\0_0.jpg")]
	[InlineData("7", 7, 0, 0, 5, 5, "\\7\\0_0.jpg")]
	[InlineData("9", 0, 0, 0, 5, 5, "\\0\\0_0.jpg")]
	[InlineData("9", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("9", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("9", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("9", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("9", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("9", 6, 0, 0, 5, 5, "\\6\\0_0.jpg")]
	[InlineData("9", 7, 0, 0, 5, 5, "\\7\\0_0.jpg")]
	[InlineData("10", 0, 0, 3, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("10", 1, 0, 6, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("10", 2, 0, 12, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("10", 3, 0, 24, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("10", 4, 0, 48, 10, 7, "\\4\\0_0.jpg")]
	[InlineData("10", 5, 0, 96, 20, 14, "\\5\\0_0.jpg")]
	[InlineData("10", 6, 0, 192, 40, 27, "\\6\\0_0.jpg")]
	[InlineData("10", 7, 0, 128, 80, 54, "\\7\\0_1.jpg")]
	[InlineData("31", 0, 0, 0, 5, 5, "\\0\\0_0.jpg")]
	[InlineData("31", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("31", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("31", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("31", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("31", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("31", 6, 0, 0, 5, 5, "\\6\\0_0.jpg")]
	[InlineData("31", 7, 0, 0, 5, 5, "\\7\\0_0.jpg")]
	[InlineData("49", 0, 0, 0, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("49", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("49", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("49", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("49", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("49", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("49", 6, 0, 0, 5, 5, "\\6\\1_1.jpg")]
	[InlineData("49", 7, 0, 0, 5, 5, "\\7\\2_2.jpg")]
	public async Task GetThumbnailImageTileReturnsCorrectImageForLevels(string itemId, int level, int x, int y, int width, int height, string file)
	{
		var expectedBase = Path.GetFullPath($"TestData/collection-dz_deepzoom/collection-dz_files/{level}/");

		var collection = new DeepZoomCollection("TestData\\collection-dz_deepzoom\\collection-dz.dzc");
		await collection.LoadAsync();

		var img = collection.GetThumbnailImageTile(itemId, level);

		Assert.StartsWith(expectedBase, Path.GetFullPath(img.Uri));
		Assert.EndsWith(file, Path.GetFullPath(img.Uri));
		Assert.True(File.Exists(img.Uri));
		Assert.Equal(new Rectangle(x, y, width, height), img.CropRect);
	}
}
