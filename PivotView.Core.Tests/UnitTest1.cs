using System.Drawing;

namespace PivotView.Core.Tests;

public class UnitTest1
{
    // - Reset zoom, deselect, stop all other animations
    // - Hide UI, run filters
    // - Calculate locations of removed items (out of view) / explode old items out
    // - Calculate new locations of current / relayout current and leave space for new ones
    // - Calculate locations of new items / implode new ones

    [Fact]
    public void DataSourceCurrentItems()
    {
        var renderer = new NewPivotRenderer
        {
            DataSource = new NewPivotDataSource(new[] { "A", "B", "C", "D" })
        };

        var items = renderer.CurrentItems;
        Assert.NotEmpty(items);
        Assert.Equal(4, items.Count);

        Assert.Equal("A", items[0].Name);
        Assert.Equal("B", items[1].Name);
        Assert.Equal("C", items[2].Name);
        Assert.Equal("D", items[3].Name);
    }

    [Fact]
    public void DataSourceAllItems()
    {
        var renderer = new NewPivotRenderer
        {
            DataSource = new NewPivotDataSource(new[] { "A", "B", "C", "D" })
        };

        var items = renderer.Items;
        Assert.NotEmpty(items);
        Assert.Equal(4, items.Count);

        Assert.Equal("A", items[0].Name);
        Assert.Equal("B", items[1].Name);
        Assert.Equal("C", items[2].Name);
        Assert.Equal("D", items[3].Name);
    }

    [Fact]
    public void Filter()
    {
        var filter = new[] { "A", "B", "C" };
        var renderer = new NewPivotRenderer
        {
            Filter = (item) => filter.Contains(item),
            DataSource = new NewPivotDataSource(new[] { "A", "B", "C", "D" })
        };

        var items = renderer.CurrentItems;
        Assert.NotEmpty(items);
        Assert.Equal(3, items.Count);

        Assert.Equal("A", items[0].Name);
        Assert.Equal("B", items[1].Name);
        Assert.Equal("C", items[2].Name);
    }

    [Fact]
    public void VerticalStackLayout()
    {
        var renderer = new NewPivotRenderer
        {
            DataSource = new NewPivotDataSource(new[] { "A", "B", "C" }),
            Layout = new PivotVerticalStackLayout { ItemSpacing = 15 },
            Frame = new RectangleF(0, 0, 120, 120)
        };
        renderer.ResetLayout();

        var items = renderer.CurrentItems;
        Assert.NotEmpty(items);
        Assert.Equal(3, items.Count);

        Assert.Equal("A", items[0].Name);
        Assert.Equal("B", items[1].Name);
        Assert.Equal("C", items[2].Name);

        Assert.Equal(new(0, 0, 120, 30), items[0].Frame.Desired);
        Assert.Equal(new(0, 45, 120, 30), items[1].Frame.Desired);
        Assert.Equal(new(0, 90, 120, 30), items[2].Frame.Desired);
    }

    [Fact]
    public void UpdatingFilterUpdatesDoesNothingToAllItems()
    {
        var filterInitial = new[] { "A", "B", "C" };
        var filterUpdate = new[] { "B", "C", "D" };

        var renderer = new NewPivotRenderer
        {
            Filter = (item) => filterInitial.Contains(item),
            DataSource = new NewPivotDataSource(new[] { "A", "B", "C", "D" }),
        };

        renderer.Filter = (item) => filterUpdate.Contains(item);

        var items = renderer.Items;
        Assert.NotEmpty(items);
        Assert.Equal(4, items.Count);

        Assert.Equal("A", items[0].Name);
        Assert.Equal("B", items[1].Name);
        Assert.Equal("C", items[2].Name);
        Assert.Equal("D", items[3].Name);
    }

    [Fact]
    public void UpdatingFilterUpdatesCurrentItems()
    {
        var filterInitial = new[] { "A", "B", "C" };
        var filterUpdate = new[] { "B", "C", "D" };

        var renderer = new NewPivotRenderer
        {
            Filter = (item) => filterInitial.Contains(item),
            DataSource = new NewPivotDataSource(new[] { "A", "B", "C", "D" }),
        };

        renderer.Filter = (item) => filterUpdate.Contains(item);

        var items = renderer.CurrentItems;
        Assert.NotEmpty(items);
        Assert.Equal(3, items.Count);

        Assert.Equal("B", items[0].Name);
        Assert.Equal("C", items[1].Name);
        Assert.Equal("D", items[2].Name);
    }

    [Fact]
    public void PreAnimationHasCorrectValues()
    {
        var filterInitial = new[] { "A", "B", "C" };
        var filterUpdate = new[] { "B", "C", "D" };

        var renderer = new NewPivotRenderer
        {
            Filter = (item) => filterInitial.Contains(item),
            DataSource = new NewPivotDataSource(new[] { "A", "B", "C", "D" }),
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
