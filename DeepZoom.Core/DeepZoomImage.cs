namespace DeepZoom.Core;

public class DeepZoomImage
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
