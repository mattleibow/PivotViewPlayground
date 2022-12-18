using PivotView.Core.Rendering;
using System.Drawing;

namespace PivotView.Core.VisualizerApp.Visualizers.Rendering;

public class RendererVisualizer : ItemsVisualizer
{
    public RendererVisualizer(string name, PivotRenderer renderer)
        : base(name + " Renderer", renderer.VisibleItems)
    {
        IsDesiredLocations = false;
        ScreenScale = 1;

        Renderer = renderer;

        renderer.ItemsChanged += OnRendererItemsChanged;
    }

    public PivotRenderer Renderer { get; }

    protected override void PrepareItems(RectF bounds)
    {
        base.PrepareItems(bounds);

        Renderer.Frame = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
    }

    private void OnRendererItemsChanged(object? sender, EventArgs e)
    {
        InvalidateDrawing();
    }
}
