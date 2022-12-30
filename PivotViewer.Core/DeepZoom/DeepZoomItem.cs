namespace PivotViewer.Core.DeepZoom;

public class DeepZoomItem
{
	private string? sourceUri;

	public string? Id { get; set; }

	public int N { get; set; }

	public string? SourceUri
	{
		get => sourceUri;
		set
		{
			sourceUri = value;
			TileBaseUri = DeepZoomExtensions.GetTileBaseUri(value);
		}
	}

	public string? TileBaseUri { get; private set; }

	public int Width { get; set; }

	public int Height { get; set; }

	/// <summary>
	/// Width / Height
	/// </summary>
	public float AspectRatio =>
		Width == 0 || Height == 0
			? 1.0f
			: (float)Width / Height;

	public RectangleF Viewport { get; set; }

	public DeepZoomItemImage? Image { get; set; }
}

public class DeepZoomItemImage
{
	private string? sourceUri;

	public string? SourceUri
	{
		get => sourceUri;
		set
		{
			sourceUri = value;
			TileBaseUri = DeepZoomExtensions.GetTileBaseUri(value);
		}
	}

	public string? TileBaseUri { get; private set; }

	public string? TileFileFormat { get; set; }

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

	public int TileSize { get; set; }

	public int TileOverlap { get; set; }
}
