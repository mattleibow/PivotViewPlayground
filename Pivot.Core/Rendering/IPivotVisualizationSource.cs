namespace Pivot.Rendering;

public interface IPivotVisualizationSource
{
	RectangleF RenderFrame { get; set; }

	IReadOnlyList<PivotVisualizationItem> Items { get; }
}
