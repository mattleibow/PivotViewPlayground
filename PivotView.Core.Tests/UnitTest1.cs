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

    [Theory]
    [InlineData(-0.1, -0.1)]
    [InlineData(0.0, 0.0)]
    [InlineData(0.1, 0.1)]
    [InlineData(0.5, 0.5)]
    [InlineData(0.9, 0.9)]
    [InlineData(1.0, 1.0)]
    [InlineData(1.1, 1.1)]
    public void LinearEasing(double progress, double expected)
    {
        var actual = Easing.Linear(progress);
        Assert.Equal(expected, actual);
    }

    [Theory]
    // double
    [InlineData(0.0, 1.0, 0.0, 0.0)]
    [InlineData(0.0, 1.0, 0.3, 0.3)]
    [InlineData(0.0, 1.0, 0.5, 0.5)]
    [InlineData(0.0, 1.0, 0.7, 0.7)]
    [InlineData(0.0, 1.0, 1.0, 1.0)]
    // float
    [InlineData(0.0f, 1.0f, 0.0, 0.0f)]
    [InlineData(0.0f, 1.0f, 0.3, 0.3f)]
    [InlineData(0.0f, 1.0f, 0.5, 0.5f)]
    [InlineData(0.0f, 1.0f, 0.7, 0.7f)]
    [InlineData(0.0f, 1.0f, 1.0, 1.0f)]
    public void LerpingLerpsSimplePrimatives(object start, object end, double progress, object expected)
    {
        var startType = start.GetType();
        var endType = end.GetType();

        Assert.Equal(startType, endType);

        var actual = Lerping.Lerps[startType](start, end, progress);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 0.0, new[] { 0f, 0f, 100f, 100f })]
    [InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 0.5, new[] { 5f, 5f, 75f, 75f })]
    [InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 1.0, new[] { 10f, 10f, 50f, 50f })]
    public void LerpingLerpsRectF(float[] s, float[] e, double progress, float[] exp)
    {
        var start = new RectangleF(s[0], s[1], s[2], s[3]);
        var end = new RectangleF(e[0], e[1], e[2], e[3]);
        var expected = new RectangleF(exp[0], exp[1], exp[2], exp[3]);

        var actual = Lerping.Lerps[typeof(RectangleF)](start, end, progress);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Anim()
    {
        var ticker = new TestTicker();
    }
}

class Animator
{
    private readonly Ticker ticker;

    public Animator(Ticker ticker)
    {
    }

    public void Tick()
    {
    }
}

class Ticker
{
    public Action<TimeSpan>? Tick { get; set; }
}

class TestTicker : Ticker
{
    public void PerformTick(TimeSpan delta)
    {
        Tick?.Invoke(delta);
    }
}
