using Pivot.Rendering;

namespace Pivot.Controls;

public interface IMauiGraphicsPivotVisualizationCanvas : IPivotVisualizationCanvas
{
	ICanvas Canvas { get; }
}
