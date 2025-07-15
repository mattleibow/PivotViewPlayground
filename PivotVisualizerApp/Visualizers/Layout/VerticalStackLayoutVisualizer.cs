namespace PivotVisualizerApp.Visualizers.Layout;

public class VerticalStackLayoutVisualizer : LayoutVisualizer<Pivot.Layout.VerticalStackLayout>
{
	public VerticalStackLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
		: base("Vertical Stack", new Pivot.Layout.VerticalStackLayout(), items)
	{
	}
}
