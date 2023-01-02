namespace DeepZoom.Core;

public interface ITileDrawingCanvas
{
	void DrawTile(object image, RectangleF source, RectangleF destination);
}
