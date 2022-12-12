using PivotView.Core.Rendering;
using System.Collections.ObjectModel;

namespace PivotView.Core.VisualizerApp.Visualizers;

public class ItemsVisualizer : Visualizer
{
    public ItemsVisualizer(string name, ObservableCollection<PivotRendererItem> items)
        : base(name)
    {
        Items = items;
    }

    public ObservableCollection<PivotRendererItem> Items { get; }

    [Switch("Show screen boundary lines")]
    public bool IsScreenLinesVisible { get; set; } = true;

    [Switch("Show items")]
    public bool IsItemsVisible { get; set; } = true;

    [Switch("Show desired locations")]
    public bool IsDesiredLocations { get; set; } = true;

    public override void Draw(ICanvas canvas, RectF bounds)
    {
        base.Draw(canvas, bounds);

        var w4 = bounds.Width / 4;
        var h4 = bounds.Height / 4;
        var screenRect = new RectF(w4, h4, w4 * 2, h4 * 2);

        // draw items
        if (IsItemsVisible)
        {
            PrepareItems(screenRect);
            DrawItems(canvas, screenRect);
        }

        // draw "screen bounds"
        if (IsScreenLinesVisible)
        {
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 1;
            canvas.DrawRectangle(screenRect);
        }
    }

    protected virtual void PrepareItems(RectF bounds)
    {
    }

    protected virtual void DrawItems(ICanvas canvas, RectF bounds)
    {
        canvas.FillColor = Colors.LightGoldenrodYellow;
        canvas.StrokeColor = Colors.Gray;
        canvas.StrokeSize = 1;

        foreach (var item in Items)
        {
            var r = IsDesiredLocations
                ? item.Frame.Desired
                : item.Frame.Current;
            var itemRect = r.ToRect();

            canvas.FillRectangle(itemRect);
            canvas.DrawRectangle(itemRect);
            canvas.DrawString(item.Name, itemRect, HorizontalAlignment.Center, VerticalAlignment.Center);
        }
    }
}
