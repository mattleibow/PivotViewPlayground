using PivotView.Core.Layout;
using PivotView.Core.Rendering;
using System.Collections.ObjectModel;

namespace PivotView.Core.VisualizerApp.Visualizers;

public class GridLayoutVisualizer : LayoutVisualizer<PivotGridLayout>
{
    public GridLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
        : base("Grid", new PivotGridLayout(), items)
    {
    }
}
