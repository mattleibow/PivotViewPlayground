namespace Pivot.Core.Tests;

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

	private static PivotDataSource CreateDataSource(params string[] items)
	{
		var datasource = new PivotDataSource();

		foreach (var item in items)
			datasource.Items.Add(new PivotDataItem { Id = item });

		return datasource;
	}
}
