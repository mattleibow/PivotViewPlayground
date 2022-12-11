namespace PivotView.Core.Tests;

public partial class PivotLayoutUnitTests
{
    public class VerticalStack
    {
        [Fact]
        public void LayoutIsCorrect()
        {
            var frame = new RectangleF(0, 0, 120, 120);
            var layout = new PivotVerticalStackLayout { ItemSpacing = 15 };

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(0, 0, 120, 30), items[0].Frame.Desired);
            Assert.Equal(new(0, 45, 120, 30), items[1].Frame.Desired);
            Assert.Equal(new(0, 90, 120, 30), items[2].Frame.Desired);
        }

        [Fact]
        public void OffsetLayoutIsCorrect()
        {
            var frame = new RectangleF(100, 100, 120, 120);
            var layout = new PivotVerticalStackLayout { ItemSpacing = 15 };

            var items = CreateItemList(new[] { "A", "B", "C" });

            layout.LayoutItems(items, frame);

            Assert.Equal(new(100, 100, 120, 30), items[0].Frame.Desired);
            Assert.Equal(new(100, 145, 120, 30), items[1].Frame.Desired);
            Assert.Equal(new(100, 190, 120, 30), items[2].Frame.Desired);
        }
    }
}
