namespace PivotView.Core.Tests;

public partial class PivotLayoutUnitTests
{
    public class VerticalStack
    {
        [Fact]
        public void MeasureIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new VerticalStackLayout();

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.MeasureItems(items, frame);

            Assert.Equal(1, layout.ItemAspectRatio);
            Assert.Equal(40, layout.ItemHeight);
            Assert.Equal(40, layout.ItemWidth);
            Assert.Equal(120, layout.LayoutHeight);
            Assert.Equal(40, layout.LayoutWidth);
        }

        [Fact]
        public void LayoutIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new VerticalStackLayout();

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(0, 0, 40, 40), items[0].Frame.Desired);
            Assert.Equal(new(0, 40, 40, 40), items[1].Frame.Desired);
            Assert.Equal(new(0, 80, 40, 40), items[2].Frame.Desired);
        }

        [Fact]
        public void LayoutWithItemMarginIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new VerticalStackLayout { ItemMargin = 5 };

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(5, 5, 30, 30), items[0].Frame.Desired);
            Assert.Equal(new(5, 45, 30, 30), items[1].Frame.Desired);
            Assert.Equal(new(5, 85, 30, 30), items[2].Frame.Desired);
        }

        [Fact]
        public void OffsetLayoutIsCorrect()
        {
            var frame = new RectangleF(100, 100, 120, 120);
            var layout = new VerticalStackLayout();

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(100, 100, 40, 40), items[0].Frame.Desired);
            Assert.Equal(new(100, 140, 40, 40), items[1].Frame.Desired);
            Assert.Equal(new(100, 180, 40, 40), items[2].Frame.Desired);
        }

        [Fact]
        public void OffsetLayoutWithItemMarginIsCorrect()
        {
            var frame = new RectangleF(100, 100, 120, 120);
            var layout = new VerticalStackLayout { ItemMargin = 5 };

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(105, 105, 30, 30), items[0].Frame.Desired);
            Assert.Equal(new(105, 145, 30, 30), items[1].Frame.Desired);
            Assert.Equal(new(105, 185, 30, 30), items[2].Frame.Desired);
        }
    }
}
