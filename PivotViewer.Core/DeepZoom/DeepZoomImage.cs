namespace PivotViewer.Core.DeepZoom;

public class DeepZoomImage
{
	private string? sourceUri;
	private string? tilesBaseUri;

	public string? Id { get; set; }

	public int N { get; set; }

	public string? SourceUri
	{
		get => sourceUri;
		set
		{
			sourceUri = value;
			tilesBaseUri = DeepZoomImageCollection.GetTilesBaseUri(value);
		}
	}

	public string? TilesBaseUri => tilesBaseUri;

	public int Width { get; set; }

	public int Height { get; set; }

	/// <summary>
	/// Width / Height
	/// </summary>
	public float AspectRatio =>
		Width == 0 || Height == 0
			? 1.0f
			: (float)Width / Height;

	public int MaxLevel =>
		(int)Math.Ceiling(Math.Log2(Math.Max(Width, Height)));

	public RectangleF Viewport { get; set; }
}
