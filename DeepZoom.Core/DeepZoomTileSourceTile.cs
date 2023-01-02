namespace DeepZoom.Core;

public class DeepZoomTileSourceTile
{
	public DeepZoomTileSourceTile(string uri, Rectangle cropRect = default, Point coordinates = default)
	{
		Uri = uri;
		CropRect = cropRect;
		Coordinates = coordinates;
	}

	public string Uri { get; }

	public Rectangle CropRect { get; }

	public Point Coordinates { get; }
}
