namespace PivotView.Core.VisualizerApp.Visualizers.Layout;

public class GridLayoutVisualizer : LayoutVisualizer<GridLayout>
{
	public GridLayoutVisualizer(ObservableCollection<PivotRendererItem> items)
		: base("Grid", new GridLayout(), items)
	{
	}

	[Switch("Stack from the bottom")]
	public bool IsBottomUp
	{
		get => Layout.Origin == GridLayout.LayoutOrigin.Bottom;
		set => Layout.Origin = value ? GridLayout.LayoutOrigin.Bottom : GridLayout.LayoutOrigin.Top;
	}
}
