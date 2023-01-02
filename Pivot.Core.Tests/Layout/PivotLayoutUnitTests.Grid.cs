using Xunit.Abstractions;

namespace Pivot.Core.Tests;

public partial class PivotLayoutUnitTests
{
	public class Grid
	{
		[Fact]
		public void SingleItemMeasureIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A" });

			layout.MeasureItems(items, frame);

			Assert.Equal(1, layout.Rows);
			Assert.Equal(1, layout.Columns);

			Assert.Equal(1, layout.ItemAspectRatio);
			Assert.Equal(120, layout.ItemHeight);
			Assert.Equal(120, layout.ItemWidth);
			Assert.Equal(120, layout.LayoutHeight);
			Assert.Equal(120, layout.LayoutWidth);
		}

		[Fact]
		public void SingleRowMeasureIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B" });

			layout.MeasureItems(items, frame);

			Assert.Equal(1, layout.Rows);
			Assert.Equal(2, layout.Columns);

			Assert.Equal(1, layout.ItemAspectRatio);
			Assert.Equal(60, layout.ItemHeight);
			Assert.Equal(60, layout.ItemWidth);
			Assert.Equal(60, layout.LayoutHeight);
			Assert.Equal(120, layout.LayoutWidth);
		}

		[Fact]
		public void MeasureIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B", "C" });

			layout.MeasureItems(items, frame);

			Assert.Equal(2, layout.Rows);
			Assert.Equal(2, layout.Columns);

			Assert.Equal(1, layout.ItemAspectRatio);
			Assert.Equal(60, layout.ItemHeight);
			Assert.Equal(60, layout.ItemWidth);
			Assert.Equal(120, layout.LayoutHeight);
			Assert.Equal(120, layout.LayoutWidth);
		}

		[Fact]
		public void PerfectMultiRowMultiColMeasureIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B", "C", "D", "E", "F" });

			layout.MeasureItems(items, frame);

			Assert.Equal(2, layout.Rows);
			Assert.Equal(3, layout.Columns);

			Assert.Equal(1, layout.ItemAspectRatio);
			Assert.Equal(40, layout.ItemHeight);
			Assert.Equal(40, layout.ItemWidth);
			Assert.Equal(80, layout.LayoutHeight);
			Assert.Equal(120, layout.LayoutWidth);
		}

		[Fact]
		public void MultiRowMultiColMeasureIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B", "C", "D", "E", "F", "G" });

			layout.MeasureItems(items, frame);

			Assert.Equal(3, layout.Rows);
			Assert.Equal(3, layout.Columns);

			Assert.Equal(1, layout.ItemAspectRatio);
			Assert.Equal(40, layout.ItemHeight);
			Assert.Equal(40, layout.ItemWidth);
			Assert.Equal(120, layout.LayoutHeight);
			Assert.Equal(120, layout.LayoutWidth);
		}

		[Fact]
		public void SingleItemArrangeIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(0, 0, 120, 120), items[0].Frame.Desired);
		}

		[Fact]
		public void SingleRowArrangeIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(0, 0, 60, 60), items[0].Frame.Desired);
			Assert.Equal(new(60, 0, 60, 60), items[1].Frame.Desired);
		}

		[Fact]
		public void ArrangeIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B", "C" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(0, 0, 60, 60), items[0].Frame.Desired);
			Assert.Equal(new(60, 0, 60, 60), items[1].Frame.Desired);
			Assert.Equal(new(0, 60, 60, 60), items[2].Frame.Desired);
		}

		[Fact]
		public void ArrangeFromBottomIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout
			{
				Origin = GridLayout.LayoutOrigin.Bottom
			};

			var items = CreateItemList(new[] { "A", "B", "C" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(0, 0, 60, 60), items[0].Frame.Desired);
			Assert.Equal(new(0, 60, 60, 60), items[1].Frame.Desired);
			Assert.Equal(new(60, 60, 60, 60), items[2].Frame.Desired);
		}

		[Fact]
		public void PerfectMultiRowMultiColArrangeIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B", "C", "D", "E", "F" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(0, 0, 40, 40), items[0].Frame.Desired);
			Assert.Equal(new(40, 0, 40, 40), items[1].Frame.Desired);
			Assert.Equal(new(80, 0, 40, 40), items[2].Frame.Desired);
			Assert.Equal(new(0, 40, 40, 40), items[3].Frame.Desired);
			Assert.Equal(new(40, 40, 40, 40), items[4].Frame.Desired);
			Assert.Equal(new(80, 40, 40, 40), items[5].Frame.Desired);
		}

		[Fact]
		public void MultiRowMultiColArrangeIsCorrect()
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B", "C", "D", "E", "F", "G" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(0, 0, 40, 40), items[0].Frame.Desired);
			Assert.Equal(new(40, 0, 40, 40), items[1].Frame.Desired);
			Assert.Equal(new(80, 0, 40, 40), items[2].Frame.Desired);
			Assert.Equal(new(0, 40, 40, 40), items[3].Frame.Desired);
			Assert.Equal(new(40, 40, 40, 40), items[4].Frame.Desired);
			Assert.Equal(new(80, 40, 40, 40), items[5].Frame.Desired);
			Assert.Equal(new(0, 80, 40, 40), items[6].Frame.Desired);
		}

		[Fact]
		public void OffsetLayoutIsCorrect()
		{
			var frame = new RectangleF(100, 100, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A", "B", "C" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(100, 100, 60, 60), items[0].Frame.Desired);
			Assert.Equal(new(160, 100, 60, 60), items[1].Frame.Desired);
			Assert.Equal(new(100, 160, 60, 60), items[2].Frame.Desired);
		}

		[Fact]
		public void OffsetLayoutWithItemMarginIsCorrect()
		{
			var frame = new RectangleF(100, 100, 120, 120);
			var layout = new GridLayout
			{
				ItemMargin = 5
			};

			var items = CreateItemList(new[] { "A", "B", "C" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(105, 105, 50, 50), items[0].Frame.Desired);
			Assert.Equal(new(165, 105, 50, 50), items[1].Frame.Desired);
			Assert.Equal(new(105, 165, 50, 50), items[2].Frame.Desired);
		}

		[Fact]
		public void OffsetLayoutFromBottomIsCorrect()
		{
			var frame = new RectangleF(100, 100, 120, 120);
			var layout = new GridLayout
			{
				Origin = GridLayout.LayoutOrigin.Bottom
			};

			var items = CreateItemList(new[] { "A", "B", "C" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(100, 100, 60, 60), items[0].Frame.Desired);
			Assert.Equal(new(100, 160, 60, 60), items[1].Frame.Desired);
			Assert.Equal(new(160, 160, 60, 60), items[2].Frame.Desired);
		}

		[Fact]
		public void OffsetLayoutFromBottomWithItemMarginIsCorrect()
		{
			var frame = new RectangleF(100, 100, 120, 120);
			var layout = new GridLayout
			{
				ItemMargin = 5,
				Origin = GridLayout.LayoutOrigin.Bottom
			};

			var items = CreateItemList(new[] { "A", "B", "C" });

			layout.LayoutItems(items, frame);

			Assert.Equal(new(105, 105, 50, 50), items[0].Frame.Desired);
			Assert.Equal(new(105, 165, 50, 50), items[1].Frame.Desired);
			Assert.Equal(new(165, 165, 50, 50), items[2].Frame.Desired);
		}

		[Theory]
		[InlineData(GridLayout.SearchDirection.Left)]
		[InlineData(GridLayout.SearchDirection.Right)]
		[InlineData(GridLayout.SearchDirection.Up)]
		[InlineData(GridLayout.SearchDirection.Down)]
		public void SingleItemGetNextItem(GridLayout.SearchDirection direction)
		{
			var frame = new RectangleF(0, 0, 120, 120);
			var layout = new GridLayout();

			var items = CreateItemList(new[] { "A" });

			layout.LayoutItems(items, frame);

			var next = layout.GetNextItem(items[0], direction);

			Assert.Null(next);
		}

		[Theory]
		// X
		[InlineData(100, 100, 1, GridLayout.SearchDirection.Left, 0, -1)]
		[InlineData(100, 100, 1, GridLayout.SearchDirection.Right, 0, -1)]
		[InlineData(100, 100, 1, GridLayout.SearchDirection.Up, 0, -1)]
		[InlineData(100, 100, 1, GridLayout.SearchDirection.Down, 0, -1)]
		// X X
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Left, 0, -1)]
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Right, 0, 1)]
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Up, 0, -1)]
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Down, 0, 1)]
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Left, 1, 0)]
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Right, 1, -1)]
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Up, 1, 0)]
		[InlineData(200, 100, 2, GridLayout.SearchDirection.Down, 1, -1)]
		// X
		// X
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Left, 0, -1)]
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Right, 0, 1)]
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Up, 0, -1)]
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Down, 0, 1)]
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Left, 1, 0)]
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Right, 1, -1)]
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Up, 1, 0)]
		[InlineData(100, 200, 2, GridLayout.SearchDirection.Down, 1, -1)]
		// X X
		// X
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Left, 0, -1)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Right, 0, 1)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Up, 0, -1)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Down, 0, 2)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Left, 1, 0)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Right, 1, 2)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Up, 1, 2)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Down, 1, -1)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Left, 2, 1)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Right, 2, -1)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Up, 2, 0)]
		[InlineData(100, 100, 3, GridLayout.SearchDirection.Down, 2, 1)]
		// X X X
		// X X X
		// X X X
		[InlineData(100, 100, 9, GridLayout.SearchDirection.Left, 4, 3)]
		[InlineData(100, 100, 9, GridLayout.SearchDirection.Right, 4, 5)]
		[InlineData(100, 100, 9, GridLayout.SearchDirection.Up, 4, 1)]
		[InlineData(100, 100, 9, GridLayout.SearchDirection.Down, 4, 7)]
		public void GetNextItem(int width, int height, int count, GridLayout.SearchDirection direction, int startingIndex, int nextIndex)
		{
			var frame = new RectangleF(0, 0, width, height);
			var layout = new GridLayout();

			var items = CreateItemList(Enumerable.Range(1, count).Select(x => x.ToString()).ToArray());

			layout.LayoutItems(items, frame);

			var next = layout.GetNextItem(items[startingIndex], direction);

			var idx = next is null ? -1 : items.IndexOf(next);

			Assert.Equal(nextIndex, idx);
		}
	}
}
