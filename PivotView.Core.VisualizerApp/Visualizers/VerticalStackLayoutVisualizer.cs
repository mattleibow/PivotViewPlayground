using PivotView.Core.Layout;
using PivotView.Core.Rendering;
using System.Collections.ObjectModel;

namespace PivotView.Core.VisualizerApp.Visualizers;

public class VerticalStackLayoutVisualizer : LayoutVisualizer<PivotVerticalStackLayout>
{
    public VerticalStackLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
        : base("Vertical Stack", new PivotVerticalStackLayout(), items)
    {
    }

    [Slider("Item spacing", 0, 20)]
    public float ItemSpacing
    {
        get => Layout.ItemSpacing;
        set => Layout.ItemSpacing = value;
    }
}
