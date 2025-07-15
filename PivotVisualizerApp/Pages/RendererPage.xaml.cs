using Pivot.Data.Model;
using Pivot.Data.Sources.Cxml;
using PivotVisualizerApp.Visualizers.Rendering;

namespace PivotVisualizerApp;

public partial class RendererPage : ContentPage
{
	private readonly PivotVisualizationController renderer = new();

	private readonly List<PivotDataItem> allItems = new();

	private string? itemsText;

	private bool isVisible;
	private long lastUpdated;

	public RendererPage()
	{
		InitializeComponent();

		Visualizer = new RendererVisualizer("Default", renderer);

		BindingContext = this;

		_ = LoadCollectionAsync();
	}

	public RendererVisualizer Visualizer { get; }

	public string? ItemsText
	{
		get => itemsText;
		set
		{
			itemsText = value ?? string.Empty;

			var filter = itemsText.Split(new[] { '\r', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			renderer.Filter = allItems.Where(i => filter.Contains(i.Id)).ToList();

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

	private async Task LoadCollectionAsync()
	{
		//try
		//{
		//	var datasource = new CxmlPivotDataSource($"{MauiProgram.TestDataPath}conceptcars.cxml");
		//	await datasource.LoadAsync();

		//	newIds = datasource.Items.Select(i => i.Id!).ToArray();

		//	renderer.DataSource = datasource;
		//}
		//catch
		//{

		var newIds = Enumerable.Range(1, 100).Select(i => i.ToString());

		allItems.Clear();
		allItems.AddRange(newIds.Select(NewItem));

		var datasource = new PivotDataSource();
		datasource.Items.AddRange(allItems);
		renderer.DataSource = datasource;

		//}

		ItemsText = string.Join(Environment.NewLine, allItems.Select(i => i.Id));
	}

	private static PivotDataItem NewItem(string id) =>
		new()
		{
			Id = id,
			ImageWidth = 100,
			ImageHeight = 100
		};
}
