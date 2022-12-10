namespace PivotView.Core.Tests;

public partial class PivotRendererUnitTests
{
    public class Animation
    {
        [Fact]
        public void PreAnimationHasCorrectValues()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "B", "C", "D" };

            var renderer = new PivotRenderer
            {
                Filter = (item) => filterInitial.Contains(item),
                DataSource = CreateDataSource(new[] { "A", "B", "C", "D" }),
                Layout = new PivotVerticalStackLayout { ItemSpacing = 15 },
                Frame = new RectangleF(0, 0, 120, 120)
            };
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.Items;

            Assert.Equal(new(0, 0, 120, 30), items[0].Frame.Current);
            Assert.Equal(new(0, 45, 120, 30), items[1].Frame.Current);
            Assert.Equal(new(0, 90, 120, 30), items[2].Frame.Current);
            Assert.Equal(new(170, 90, 120, 30), items[3].Frame.Current);

            Assert.Equal(new(170, 0, 120, 30), items[0].Frame.Desired);
            Assert.Equal(new(0, 0, 120, 30), items[1].Frame.Desired);
            Assert.Equal(new(0, 45, 120, 30), items[2].Frame.Desired);
            Assert.Equal(new(0, 90, 120, 30), items[3].Frame.Desired);
        }
    }
}
