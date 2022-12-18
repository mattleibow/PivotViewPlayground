using PivotView.Core.Layout;
using PivotView.Core.Rendering;
using System.Collections.ObjectModel;
using System.Drawing;

namespace PivotView.Core.VisualizerApp.Visualizers.Layout;

public class VerticalStackLayoutVisualizer : LayoutVisualizer<Core.Layout.VerticalStackLayout>
{
    public VerticalStackLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
        : base("Vertical Stack", new Core.Layout.VerticalStackLayout(), items)
    {
    }
}
