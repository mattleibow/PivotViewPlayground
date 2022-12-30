namespace PivotViewer.Core.Tests;

public class DeepZoomExtensionsUnitTests
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
		var uri = DeepZoomExtensions.GetBaseUri(sourceUri);

		Assert.Equal(baseUri, uri);
	}

	[Theory]
	[InlineData("TestData\\collection-dz_deepzoom\\collection-dz.dzc", "TestData/collection-dz_deepzoom/collection-dz_files")]
	[InlineData("TestData\\collection-dz_deepzoom\\37.dzi", "TestData/collection-dz_deepzoom/37_files")]
	[InlineData("C:\\TestData\\collection-dz_deepzoom\\37.dzi", "C:/TestData/collection-dz_deepzoom/37_files")]
	[InlineData("file:///C:/TestData/collection-dz_deepzoom/dz.dzc", "file:///C:/TestData/collection-dz_deepzoom/dz_files")]
	[InlineData("file:///C:/TestData/collection-dz_deepzoom/37.dzi", "file:///C:/TestData/collection-dz_deepzoom/37_files")]
	[InlineData("https://test-data.org/collection-dz_deepzoom/37.dzi", "https://test-data.org/collection-dz_deepzoom/37_files")]
	public void GetTilesBaseUriIsCorrect(string sourceUri, string tilesUri)
	{
		var uri = DeepZoomExtensions.GetTileBaseUri(sourceUri);

		Assert.Equal(tilesUri, uri);
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(5, 0)]
	[InlineData(0, 5)]
	[InlineData(5, 5)]
	[InlineData(5, 3)]
	[InlineData(10, 5)]
	[InlineData(5, 10)]
	[InlineData(50, 100)]
	[InlineData(100, 100)]
	[InlineData(500, 500)]
	[InlineData(1000, 1000)]
	[InlineData(2000, 1000)]
	[InlineData(1000, 2000)]
	[InlineData(2000, 2000)]
	[InlineData(ushort.MaxValue, 0)]
	[InlineData(ushort.MaxValue, ushort.MaxValue >> 1)]
	[InlineData(0, ushort.MaxValue >> 1)]
	[InlineData(ushort.MaxValue >> 1, 0)]
	public void EncodeDecodeMortonNumberMaintainsValues(int x, int y)
	{
		var morton = DeepZoomExtensions.GetMortonNumber(x, y);
		var point = DeepZoomExtensions.GetMortonPoint(morton);

		Assert.Equal(new Point(x, y), point);
	}

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
	[InlineData(50, 4, 5)]
	[InlineData(150, 6, 9)]
	[InlineData(500, 30, 12)]
	public void DecodeMortonNumberMaintainsValues(int morton, int x, int y)
	{
		var point = DeepZoomExtensions.GetMortonPoint(morton);

		Assert.Equal(new Point(x, y), point);
	}

	[Theory]
	[InlineData(3, 1, 7)]
	[InlineData(1, 1, 3)]
	public void EncodeMortonNumberMaintainsValues(int x, int y, int morton)
	{
		var actual = DeepZoomExtensions.GetMortonNumber(x, y);

		Assert.Equal(morton, actual);
	}
}
