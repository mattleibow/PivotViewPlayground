using System.Collections;

namespace PivotVisualizerApp;

public partial class PivotViewerPage : ContentPage
{
	public PivotViewerPage()
	{
		InitializeComponent();

		BindingContext = this;
	}

	public ObservableCollection<TestItem> Items { get; } =
		new()
		{
			new TestItem { FullName = "Matthew Leibowitz", Statement = "I is v. cool!", Tags = { "A", "B", "C" } },
			new TestItem { FullName = "Indry Horman", Statement = "yeay!", Tags = { "C" } },
		};
}

public class TestItem
{
	public string FullName { get; set; }

	public string Statement { get; set; }

	public List<string> Tags { get; } = new();
}
