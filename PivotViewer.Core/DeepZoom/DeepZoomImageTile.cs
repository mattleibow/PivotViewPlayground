namespace PivotViewer.Core.DeepZoom;

public class DeepZoomImageTile
{
	public DeepZoomImageTile(string uri, Rectangle cropRect = default)
	{
		Uri = uri;
		CropRect = cropRect;
	}

	public string Uri { get; }

	public Rectangle CropRect { get; }
}
