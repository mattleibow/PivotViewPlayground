using PivotViewer.Core.VisualizerApp.Visualizers.Rendering;

namespace PivotViewer.Core.VisualizerApp;

public partial class RendererPage : ContentPage
{
	private readonly string[] newIds;
	private readonly PivotRenderer renderer = new();

	private string? itemsText;
	private string[]? visibleIds;

	private bool isVisible;
	private long lastUpdated;

	public RendererPage()
	{
		InitializeComponent();

		try
		{
			var datasource = CxmlPivotDataSource.FromFile("C:\\Projects\\PivotViewPlayground\\PivotViewer.Core.Tests\\TestData\\conceptcars.cxml");

			newIds = datasource.Items!.Select(i => i.Id!).ToArray();

			renderer.DataSource = datasource;
		}
		catch
		{
			newIds = Enumerable.Range(1, 100).Select(i => i.ToString()).ToArray();

			renderer.DataSource = new PivotDataSource
			{
				Items = newIds.Select(NewItem).ToArray(),
			};
		}

		ItemsText = string.Join(Environment.NewLine, newIds);

		Visualizer = new RendererVisualizer("Default", renderer);

		BindingContext = this;
	}

	public RendererVisualizer Visualizer { get; }

	public string? ItemsText
	{
		get => itemsText;
		set
		{
			itemsText = value ?? string.Empty;
			visibleIds = itemsText.Split(new[] { '\r', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

			renderer.Filter = (id) => visibleIds.Contains(id);
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		isVisible = true;

		lastUpdated = Environment.TickCount64;
		Dispatcher.StartTimer(TimeSpan.FromSeconds(1 / 60.0), () =>
		{
			var time = Environment.TickCount64;

			renderer.Animation?.Update(TimeSpan.FromMilliseconds(time - lastUpdated));
			Visualizer.InvalidateDrawing();

			lastUpdated = time;

			return isVisible;
		});
	}

	protected override void OnDisappearing()
	{
		isVisible = false;

		base.OnDisappearing();
	}

	private static PivotDataItem NewItem(string id) =>
		new()
		{
			Id = id,
			ImageWidth = 100,
			ImageHeight = 100
		};
}
