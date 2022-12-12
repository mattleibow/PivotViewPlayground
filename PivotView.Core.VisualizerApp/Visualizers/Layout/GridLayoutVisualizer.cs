using PivotView.Core.Layout;
using PivotView.Core.Rendering;
using System.Collections.ObjectModel;

namespace PivotView.Core.VisualizerApp.Visualizers.Layout;

public class GridLayoutVisualizer : LayoutVisualizer<PivotGridLayout>
{
    public GridLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
        : base("Grid", new PivotGridLayout(), items)
    {
    }

    [Switch("Stack from the bottom")]
    public bool IsBottomUp
    {
        get => Layout.Origin == PivotGridLayout.LayoutOrigin.Bottom;
        set => Layout.Origin = value ? PivotGridLayout.LayoutOrigin.Bottom : PivotGridLayout.LayoutOrigin.Top;
    }
}
