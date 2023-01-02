using PivotVisualizerApp.Visualizers;
using PivotVisualizerApp.Visualizers.DeepZoom;

namespace PivotVisualizerApp;

public partial class DeepZoomPage : ContentPage
{
	private Visualizer? current;

	public DeepZoomPage()
	{
		InitializeComponent();

		var uri = $"{MauiProgram.TestDataPath}collection-dz_deepzoom\\";

		Visualizers =
			new()
			{
				new DeepZoomImageVisualizer("Image (0)", new DeepZoomImageTileSource("0", 640, 426, 10, 254, 1, $"{uri}0_files", "jpg")),
				new DeepZoomImageVisualizer("Image (7)", new DeepZoomImageTileSource("7", 454, 480, 9, 254, 1, $"{uri}7_files", "jpg")),
				new DeepZoomImageVisualizer("Collection (0)", new DeepZoomCollectionTileSource("0", 0, 640, 426, 7, 256, $"{uri}collection-dz_files", "jpg")),
				new DeepZoomImageVisualizer("Collection (7)", new DeepZoomCollectionTileSource("7", 7, 454, 480, 7, 256, $"{uri}collection-dz_files", "jpg")),
				new DeepZoomTileManagerVisualizer("Image (0)", new DeepZoomImageTileSource("0", 640, 426, 10, 254, 1, $"{uri}0_files", "jpg")),
				new DeepZoomTileManagerVisualizer("Image (7)", new DeepZoomImageTileSource("7", 454, 480, 9, 254, 1, $"{uri}7_files", "jpg")),
				new DeepZoomTileManagerVisualizer("Collection (0)",new DeepZoomCollectionTileSource("0", 0, 640, 426, 7, 256, $"{uri}collection-dz_files", "jpg")),
				new DeepZoomTileManagerVisualizer("Collection (7)",new DeepZoomCollectionTileSource("7", 7, 454, 480, 7, 256, $"{uri}collection-dz_files", "jpg")),
			};

		Current = Visualizers.FirstOrDefault();

		BindingContext = this;
	}

	public ObservableCollection<Visualizer> Visualizers { get; }

	public Visualizer? Current
	{
		get => current;
		set
		{
			current = value;
			OnPropertyChanged();
		}
	}
}
