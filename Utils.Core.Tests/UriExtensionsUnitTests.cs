namespace Utils.Core.Tests;

public class UriExtensionsUnitTests
{
	[Theory]
	[InlineData("TestData\\collection-dz_deepzoom\\collection-dz.dzc", "TestData/collection-dz_deepzoom")]
	[InlineData("TestData\\collection-dz_deepzoom\\37.dzi", "TestData/collection-dz_deepzoom")]
	[InlineData("C:\\TestData\\collection-dz_deepzoom\\37.dzi", "C:/TestData/collection-dz_deepzoom")]
	[InlineData("file:///C:/TestData/collection-dz_deepzoom/dz.dzc", "file:///C:/TestData/collection-dz_deepzoom")]
	[InlineData("file:///C:/TestData/collection-dz_deepzoom/37.dzi", "file:///C:/TestData/collection-dz_deepzoom")]
	[InlineData("https://test-data.org/collection-dz_deepzoom/37.dzi", "https://test-data.org/collection-dz_deepzoom")]
	public void GetBaseUriIsCorrect(string sourceUri, string baseUri)
	{
		var uri = UriExtensions.GetBaseUri(sourceUri);

		Assert.Equal(baseUri, uri);
	}

	[Theory]
	[InlineData("TestData\\collection-dz_deepzoom\\collection-dz.dzc", "TestData/collection-dz_deepzoom/collection-dz")]
	[InlineData("TestData\\collection-dz_deepzoom\\37.dzi", "TestData/collection-dz_deepzoom/37")]
	[InlineData("C:\\TestData\\collection-dz_deepzoom\\37.dzi", "C:/TestData/collection-dz_deepzoom/37")]
	[InlineData("file:///C:/TestData/collection-dz_deepzoom/dz.dzc", "file:///C:/TestData/collection-dz_deepzoom/dz")]
	[InlineData("file:///C:/TestData/collection-dz_deepzoom/37.dzi", "file:///C:/TestData/collection-dz_deepzoom/37")]
	[InlineData("https://test-data.org/collection-dz_deepzoom/37.dzi", "https://test-data.org/collection-dz_deepzoom/37")]
	public void GetTilesBaseUriIsCorrect(string sourceUri, string tilesUri)
	{
		var uri = UriExtensions.GetUriWithoutExtension(sourceUri);

		Assert.Equal(tilesUri, uri);
	}
}
