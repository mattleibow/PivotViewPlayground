using Pivot.Controls;

namespace PivotVisualizerApp;

class MauiGraphicsPivotVisualizationCanvas : IMauiGraphicsPivotVisualizationCanvas, IPivotVisualizationDebugCanvas
{
	public ICanvas Canvas { get; set; } = null!;

	public PivotVisualizationRendererDebugOptions DebugOptions { get; } = new();
}
