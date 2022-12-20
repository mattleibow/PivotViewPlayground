namespace PivotViewer.Core.Tests;

public partial class PivotRendererUnitTests
{
	[Fact]
	public void CreateDataSourceIsCorrect()
	{
		var dataSource = CreateDataSource("A", "B", "C", "D");
		var items = dataSource.Items;

		Assert.NotNull(items);
		Assert.NotEmpty(items);
		Assert.Equal(4, items.Count);

		Assert.Equal("A", items[0].Id);
		Assert.Equal("B", items[1].Id);
		Assert.Equal("C", items[2].Id);
		Assert.Equal("D", items[3].Id);
	}

	private static PivotDataSource CreateDataSource(params string[] items) =>
		new()
		{
			Items = items.Select(i => new PivotDataItem { Id = i }).ToArray()
		};
}
