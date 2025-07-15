using Pivot.Controls;

namespace PivotVisualizerApp.Visualizers.Rendering;

public class RendererVisualizer : Visualizer
{
	private MauiGraphicsPivotVisualizationCanvas mauiCanvas = new();

	public RendererVisualizer(string name, PivotVisualizationController controller)
		: base(name + " Renderer")
	{
		Controller = controller;
		Renderer = new MauiGraphicsPivotVisualizationRenderer(controller);

		controller.ItemsChanged += OnRendererItemsChanged;
	}

	public PivotVisualizationController Controller { get; }

	public MauiGraphicsPivotVisualizationRenderer Renderer { get; }

	[Slider("Render scale", 0.1, 1)]
	public double RenderScale
	{
		get => mauiCanvas.DebugOptions.RenderScale;
		set => mauiCanvas.DebugOptions.RenderScale = (float)value;
	}

	[Switch("Show screen boundary lines")]
	public bool DrawScreenBounds
	{
		get => mauiCanvas.DebugOptions.DrawRenderFrameBoundary;
		set => mauiCanvas.DebugOptions.DrawRenderFrameBoundary = value;
	}

	[Switch("Show items")]
	public bool DrawItems
	{
		get => mauiCanvas.DebugOptions.DrawItems;
		set => mauiCanvas.DebugOptions.DrawItems = value;
	}

	[Switch("Show debug items")]
	public bool DrawDebugItems
	{
		get => mauiCanvas.DebugOptions.DrawDebugItems;
		set => mauiCanvas.DebugOptions.DrawDebugItems = value;
	}

	[Slider("Minimum animation delay (ms)", 0, 1000)]
	public double MinimumAnimationDelay
	{
		get => Controller.MinimumAnimationDelay.TotalMilliseconds;
		set => Controller.MinimumAnimationDelay = TimeSpan.FromMilliseconds(value);
	}

	[Slider("Maximum animation delay (ms)", 0, 1000)]
	public double MaximumAnimationDelay
	{
		get => Controller.MaximumAnimationDelay.TotalMilliseconds;
		set => Controller.MaximumAnimationDelay = TimeSpan.FromMilliseconds(value);
	}

	public override void Draw(ICanvas canvas, RectF bounds)
	{
		mauiCanvas.Canvas = canvas;

		Renderer.Draw(mauiCanvas, bounds.ToSystemRectangleF());
	}

	private void OnRendererItemsChanged(object? sender, EventArgs e)
	{
		InvalidateDrawing();
	}
}
