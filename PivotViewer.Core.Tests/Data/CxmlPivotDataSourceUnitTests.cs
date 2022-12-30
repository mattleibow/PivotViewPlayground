namespace PivotViewer.Core.Tests;

public class CxmlPivotDataSourceUnitTests
{
	[Theory]
	[InlineData("conceptcars.cxml")] // Full data here: https://github.com/mattleibow/seajax.github.io
	[InlineData("msdnmagazine.cxml")]
	[InlineData("ski_resorts.cxml")]
	[InlineData("venues.cxml")]
	[InlineData("buxton.cxml")]
	public async Task CanLoadCxmlFilesAsync(string filename)
	{
		var cxml = new CxmlPivotDataSource("TestData/" + filename);
		await cxml.LoadAsync();

		Assert.NotNull(cxml);

		Assert.NotNull(cxml.Items);
		Assert.NotEmpty(cxml.Items);

		Assert.NotNull(cxml.ImageBaseUri);
	}
}
