namespace PivotVisualizerApp.Controls.Themes;

public partial class PivotViewerResources : ResourceDictionary
{
	private static bool registered;

	public PivotViewerResources()
	{
		InitializeComponent();
	}

	internal static void EnsureRegistered() =>
		Utils.EnsureResourcesRegistered<PivotViewerResources>(ref registered);
}
