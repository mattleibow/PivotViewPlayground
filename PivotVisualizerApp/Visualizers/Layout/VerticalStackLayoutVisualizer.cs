namespace PivotVisualizerApp.Visualizers.Layout;

public class VerticalStackLayoutVisualizer : LayoutVisualizer<Pivot.Core.Layout.VerticalStackLayout>
{
	public VerticalStackLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
		: base("Vertical Stack", new Pivot.Core.Layout.VerticalStackLayout(), items)
	{
	}
}
