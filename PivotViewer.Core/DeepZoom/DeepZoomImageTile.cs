namespace PivotViewer.Core.DeepZoom;

public class DeepZoomImageTile
{
	public DeepZoomImageTile(string uri, Rectangle cropRect = default, Point coordinates = default)
	{
		Uri = uri;
		CropRect = cropRect;
		Coordinates = coordinates;
	}

	public string Uri { get; }

	public Rectangle CropRect { get; }

	public Point Coordinates { get; }
}
