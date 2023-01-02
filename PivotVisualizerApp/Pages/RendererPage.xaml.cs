using PivotVisualizerApp.Visualizers.Rendering;

namespace PivotVisualizerApp;

public partial class RendererPage : ContentPage
{
	private readonly PivotRenderer renderer = new();

	private string[] newIds;
	private string? itemsText;
	private string[]? visibleIds;

	private bool isVisible;
	private long lastUpdated;

	public RendererPage()
	{
		InitializeComponent();

		Visualizer = new RendererVisualizer("Default", renderer);

		BindingContext = this;

		LoadCollectionAsync();
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

			OnPropertyChanged();
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

	private async void LoadCollectionAsync()
	{
		try
		{
			var datasource = new CxmlPivotDataSource($"{MauiProgram.TestDataPath}conceptcars.cxml");
			await datasource.LoadAsync();

			newIds = datasource.Items.Select(i => i.Id!).ToArray();

			renderer.DataSource = datasource;
		}
		catch
		{
			newIds = Enumerable.Range(1, 100).Select(i => i.ToString()).ToArray();

			var datasource = new PivotDataSource();
			foreach (var id in newIds)
				datasource.Items.Add(NewItem(id));

			renderer.DataSource = datasource;
		}

		ItemsText = string.Join(Environment.NewLine, newIds);
	}

	private static PivotDataItem NewItem(string id) =>
		new()
		{
			Id = id,
			ImageWidth = 100,
			ImageHeight = 100
		};
}
