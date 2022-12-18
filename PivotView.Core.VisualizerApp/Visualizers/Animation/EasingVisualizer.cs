using PivotView.Core.Animation;
using PivotView.Core.Data;
using PivotView.Core.Rendering;
using System.Drawing;

namespace PivotView.Core.VisualizerApp.Visualizers.Animation;

public class EasingVisualizer : AnimationVisualizer
{
    public EasingVisualizer(string name, EasingDelegate func)
        : base($"{name} Easing", new List<PivotRendererItem>(new[] { CreateAnimationItem() }))
    {
        EasingFunction = func;
    }

    public EasingDelegate EasingFunction { get; }

    protected override void PrepareItems(RectF bounds)
    {
        var p = Progress / 100.0;

        var xOffset = bounds.Width * p;
        var yOffset = bounds.Height * EasingFunction(p);

        var rect = new RectangleF(
            (float)(bounds.X - 5 + xOffset),
            (float)(bounds.Bottom - 5 - yOffset),
            10,
            10);

        Items[0].Frame.Current = rect;
        Items[0].Frame.Desired = rect;
    }

    private static PivotRendererItem CreateAnimationItem()
    {
        var dataItem = new PivotDataItem
        {
            ImageWidth = 10,
            ImageHeight = 10
        };

        var renderItem = new PivotRendererItem(dataItem);

        var rect = new RectangleF(0, 0, 10, 10);

        renderItem.Frame.Current = rect;
        renderItem.Frame.Desired = rect;

        return renderItem;
    }
}
