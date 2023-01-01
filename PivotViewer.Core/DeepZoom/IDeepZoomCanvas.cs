namespace PivotViewer.Core.DeepZoom;

public interface IDeepZoomCanvas
{
	void DrawTile(object image, RectangleF source, RectangleF destination);
}
