namespace Pivot.Core.Tests;

public partial class PivotRendererUnitTests
{
	public class Layout
	{
		[Fact]
		public void VerticalStackLayout()
		{
			var renderer = new PivotRenderer
			{
				DataSource = CreateDataSource(new[] { "A", "B", "C" }),
				Layout = new VerticalStackLayout { ItemMargin = 15 },
				LayoutTransition = new HorizontalOffsetLayoutTransition(),
				Frame = new RectangleF(0, 0, 120, 120)
			};
			renderer.ResetLayout();

			var items = renderer.CurrentItems;
			Assert.NotEmpty(items);
			Assert.Equal(3, items.Count);

			Assert.Equal("A", items[0].Id);
			Assert.Equal("B", items[1].Id);
			Assert.Equal("C", items[2].Id);

			Assert.Equal(new(0, 0, 120, 30), items[0].Frame.Desired);
			Assert.Equal(new(0, 45, 120, 30), items[1].Frame.Desired);
			Assert.Equal(new(0, 90, 120, 30), items[2].Frame.Desired);
		}
	}
}
