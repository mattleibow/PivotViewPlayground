namespace PivotView.Core.Tests;

public partial class PivotLayoutUnitTests
{
    public class Grid
    {
        [Fact]
        public void SingleItemMeasureIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A" });

            layout.Measure(items, frame);

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
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A", "B" });

            layout.Measure(items, frame);

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
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.Measure(items, frame);

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
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A", "B", "C", "D", "E", "F" });

            layout.Measure(items, frame);

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
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A", "B", "C", "D", "E", "F", "G" });

            layout.Measure(items, frame);

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
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(0, 0, 120, 120), items[0].Frame.Desired);
        }

        [Fact]
        public void SingleRowArrangeIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A", "B" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(0, 0, 60, 60), items[0].Frame.Desired);
            Assert.Equal(new(60, 0, 60, 60), items[1].Frame.Desired);
        }

        [Fact]
        public void ArrangeIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new PivotGridLayout();

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
            var layout = new PivotGridLayout
            {
                Origin = PivotGridLayout.LayoutOrigin.Bottom
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
            var layout = new PivotGridLayout();

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
            var layout = new PivotGridLayout();

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
            var layout = new PivotGridLayout();

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
            var layout = new PivotGridLayout
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
            var layout = new PivotGridLayout
            {
                Origin = PivotGridLayout.LayoutOrigin.Bottom
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
            var layout = new PivotGridLayout
            {
                ItemMargin = 5,
                Origin = PivotGridLayout.LayoutOrigin.Bottom
            };

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(105, 105, 50, 50), items[0].Frame.Desired);
            Assert.Equal(new(105, 165, 50, 50), items[1].Frame.Desired);
            Assert.Equal(new(165, 165, 50, 50), items[2].Frame.Desired);
        }
    }
}
