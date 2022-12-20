namespace PivotViewer.Core.Tests;

public class CxmlPivotDataSourceUnitTests
{
	[Theory]
	[InlineData("conceptcars.cxml", true)] // Full data here: https://github.com/mattleibow/seajax.github.io
	[InlineData("msdnmagazine.cxml", false)]
	[InlineData("ski_resorts.cxml", false)]
	[InlineData("venues.cxml", false)]
	[InlineData("buxton.cxml", false)]
	public void CanLoadCxmlFiles(string filename, bool loadDeepZoom)
	{
		var cxml = CxmlPivotDataSource.FromFile("TestData/" + filename, loadDeepZoom);
		Assert.NotNull(cxml);
	}
}
