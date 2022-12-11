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
        public void PerfectMultRowMultiColMeasureIsCorrect()
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
        public void MultRowMultiColMeasureIsCorrect()
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
        public void LayoutIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new PivotGridLayout();

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(0, 0, 120, 30), items[0].Frame.Desired);
            Assert.Equal(new(0, 45, 120, 30), items[1].Frame.Desired);
            Assert.Equal(new(0, 90, 120, 30), items[2].Frame.Desired);
        }
    }
}
