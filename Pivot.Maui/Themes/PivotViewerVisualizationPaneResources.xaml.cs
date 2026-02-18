namespace Pivot.Controls.Themes;

public partial class PivotViewerVisualizationPaneResources : ResourceDictionary
{
	private static bool registered;

	public PivotViewerVisualizationPaneResources()
	{
		InitializeComponent();
	}

	internal static void EnsureRegistered() =>
		Utils.EnsureResourcesRegistered<PivotViewerVisualizationPaneResources>(ref registered);
}
