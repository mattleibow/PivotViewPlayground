using PivotView.Core.Layout;
using PivotView.Core.Rendering;
using System.Collections.ObjectModel;
using System.Drawing;

namespace PivotView.Core.VisualizerApp.Visualizers.Layout;

public class VerticalStackLayoutVisualizer : LayoutVisualizer<PivotVerticalStackLayout>
{
    public VerticalStackLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
        : base("Vertical Stack", new PivotVerticalStackLayout(), items)
    {
    }
}
