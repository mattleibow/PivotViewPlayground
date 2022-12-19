namespace PivotViewer.Core.Tests;

public partial class PivotRendererUnitTests
{
    public class Filter
    {
        [Fact]
        public void FilterRemovesItemsFromCurrentItems()
        {
            var filter = new[] { "A", "B", "C" };
            var renderer = new PivotRenderer
            {
                Filter = (item) => filter.Contains(item),
                DataSource = CreateDataSource(new[] { "A", "B", "C", "D" })
            };

            var items = renderer.CurrentItems;
            Assert.NotEmpty(items);
            Assert.Equal(3, items.Count);

            Assert.Equal("A", items[0].Id);
            Assert.Equal("B", items[1].Id);
            Assert.Equal("C", items[2].Id);
        }

        [Fact]
        public void UpdatingFilterUpdatesDoesNothingToAllItems()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "B", "C", "D" };

            var renderer = new PivotRenderer
            {
                Filter = (item) => filterInitial.Contains(item),
                DataSource = CreateDataSource(new[] { "A", "B", "C", "D" }),
            };

            // update filter
            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.Items;
            Assert.NotEmpty(items);
            Assert.Equal(4, items.Count);

            Assert.Equal("A", items[0].Id);
            Assert.Equal("B", items[1].Id);
            Assert.Equal("C", items[2].Id);
            Assert.Equal("D", items[3].Id);
        }

        [Fact]
        public void UpdatingFilterUpdatesCurrentItems()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "B", "C", "D" };

            var renderer = new PivotRenderer
            {
                Filter = (item) => filterInitial.Contains(item),
                DataSource = CreateDataSource(new[] { "A", "B", "C", "D" }),
            };

            // update filter
            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.CurrentItems;
            Assert.NotEmpty(items);
            Assert.Equal(3, items.Count);

            Assert.Equal("B", items[0].Id);
            Assert.Equal("C", items[1].Id);
            Assert.Equal("D", items[2].Id);
        }
    }
}
