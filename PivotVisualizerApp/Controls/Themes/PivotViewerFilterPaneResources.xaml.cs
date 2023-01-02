namespace PivotVisualizerApp.Controls.Themes;

public partial class PivotViewerFilterPaneResources : ResourceDictionary
{
	private static bool registered;

	public PivotViewerFilterPaneResources()
	{
		InitializeComponent();
	}

	internal static void EnsureRegistered() =>
		Utils.EnsureResourcesRegistered<PivotViewerFilterPaneResources>(ref registered);
}
