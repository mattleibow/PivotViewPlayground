namespace Pivot.Rendering;

public interface IPivotVisualizationDebugCanvas : IPivotVisualizationCanvas
{
	PivotVisualizationRendererDebugOptions? DebugOptions { get; }
}
