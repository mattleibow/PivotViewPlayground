using PivotView.Core.Layout;
using PivotView.Core.Rendering;
using System.Collections.ObjectModel;

namespace PivotView.Core.VisualizerApp.Visualizers;

public class LayoutVisualizer : Visualizer
{
    private RectF lastScreenRect;

    public LayoutVisualizer(string name, PivotLayout layout, ObservableCollection<PivotRendererItem> items)
        : base(name + " Layout")
    {
        Layout = layout;
        Items = items;

        items.CollectionChanged += (s, e) => InvalidateLayout();
    }

    public PivotLayout Layout { get; }

    public ObservableCollection<PivotRendererItem> Items { get; }
    
    [Switch("Show layout lines")]
    public bool IsLayoutLinesVisible { get; set; } = false;

    [Switch("Show screen boundary lines")]
    public bool IsScreenLinesVisible { get; set; } = true;

    [Switch("Show items")]
    public bool IsItemsVisible { get; set; } = true;

    [Slider("Item margin", 0, 20)]
    public double ItemMargin
    {
        get => Layout.ItemMargin;
        set => Layout.ItemMargin = value;
    }

    public void InvalidateLayout()
    {
        lastScreenRect = RectF.Zero;
    }

    public override void Draw(ICanvas canvas, RectF bounds)
    {
        base.Draw(canvas, bounds);

        var w4 = bounds.Width / 4;
        var h4 = bounds.Height / 4;
        var screenRect = new RectF(w4, h4, w4 * 2, h4 * 2);

        // draw items
        if (IsItemsVisible)
        {
            UpdateLayout(screenRect);

            DrawItems(canvas, screenRect);
        }

        // draw layout lines
        if (IsLayoutLinesVisible)
            DrawLayoutLines(canvas, screenRect);

        // draw "screen bounds"
        if (IsScreenLinesVisible)
        {
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 1;
            canvas.DrawRectangle(screenRect);
        }
    }

    protected virtual void DrawItems(ICanvas canvas, RectF bounds)
    {
        canvas.FillColor = Colors.LightGoldenrodYellow;
        canvas.StrokeColor = Colors.Gray;
        canvas.StrokeSize = 1;

        foreach (var item in Items)
        {
            var itemRect = item.Frame.Desired.ToRect();

            canvas.FillRectangle(itemRect);
            canvas.DrawRectangle(itemRect);
        }
    }

    protected virtual void DrawLayoutLines(ICanvas canvas, RectF bounds)
    {
    }

    protected virtual void UpdateLayout(RectF screenRect)
    {
        if (lastScreenRect == screenRect)
            return;
        lastScreenRect = screenRect;

        Layout?.LayoutItems(Items, screenRect.ToSystemRectangleF());
    }

    protected override void OnVisualizerPropertyValueChanged()
    {
        InvalidateLayout();

        base.OnVisualizerPropertyValueChanged();
    }
}

public class LayoutVisualizer<T> : LayoutVisualizer
    where T : PivotLayout
{
    public LayoutVisualizer(string name, T layout, ObservableCollection<PivotRendererItem> items)
        : base(name, layout, items)
    {
    }

    public new T Layout => (T)base.Layout;
}
