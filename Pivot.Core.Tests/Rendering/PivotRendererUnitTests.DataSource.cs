namespace Pivot.Core.Tests;

public partial class PivotRendererUnitTests
{
	public class DataSource
	{
		[Fact]
		public void CurrentItemsIsCorrect()
		{
			var renderer = new PivotRenderer
			{
				DataSource = CreateDataSource(new[] { "A", "B", "C", "D" })
			};

			var items = renderer.CurrentItems;
			Assert.NotEmpty(items);
			Assert.Equal(4, items.Count);

			Assert.Equal("A", items[0].Id);
			Assert.Equal("B", items[1].Id);
			Assert.Equal("C", items[2].Id);
			Assert.Equal("D", items[3].Id);
		}

		[Fact]
		public void AllItemsIsCorrect()
		{
			var renderer = new PivotRenderer
			{
				DataSource = CreateDataSource(new[] { "A", "B", "C", "D" })
			};

			var items = renderer.Items;
			Assert.NotEmpty(items);
			Assert.Equal(4, items.Count);

			Assert.Equal("A", items[0].Id);
			Assert.Equal("B", items[1].Id);
			Assert.Equal("C", items[2].Id);
			Assert.Equal("D", items[3].Id);
		}
	}
}
