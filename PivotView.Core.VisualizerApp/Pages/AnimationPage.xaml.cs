using PivotView.Core.VisualizerApp.Visualizers.Animation;
using Easing = PivotView.Core.Animation.Easing;

namespace PivotView.Core.VisualizerApp;

public partial class AnimationPage : ContentPage
{
	private AnimationVisualizer? current;
	private bool isVisible;

	public AnimationPage()
	{
		InitializeComponent();

		Visualizers =
			new()
			{
				new EasingVisualizer("Linear", Easing.Linear),

				new EasingVisualizer("Sin In", Easing.SinIn),
				new EasingVisualizer("Sin Out", Easing.SinOut),
				new EasingVisualizer("Sin InOut", Easing.SinInOut),

				new EasingVisualizer("Spring In", Easing.SpringIn),
				new EasingVisualizer("Spring Out", Easing.SpringOut),

				new EasingVisualizer("Cubic In", Easing.CubicIn),
				new EasingVisualizer("Cubic Out", Easing.CubicOut),
				new EasingVisualizer("Cubic InOut", Easing.CubicInOut),

				new LerpingVisualizer()
			};

		Current = Visualizers.FirstOrDefault();

		BindingContext = this;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		isVisible = true;
		Dispatcher.StartTimer(TimeSpan.FromSeconds(1.0 / 60.0), () =>
		{
			Current?.TimerTick();
			return isVisible;
		});
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		isVisible = false;
	}

	public ObservableCollection<PivotRendererItem> Items { get; } =
		new()
		{
			NewItem("Item 1"),
			NewItem("Item 2"),
			NewItem("Item 3"),
			NewItem("Item 4"),
		};

	public ObservableCollection<AnimationVisualizer> Visualizers { get; }

	public AnimationVisualizer? Current
	{
		get => current;
		set
		{
			current = value;
			OnPropertyChanged();
		}
	}

	private static PivotRendererItem NewItem(string name) =>
		new(new() { Id = name });
}
