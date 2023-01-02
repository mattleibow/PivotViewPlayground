namespace DeepZoom.Core;

public class Tile
{
	public int Level { get; set; }

	public int X { get; set; }

	public int Y { get; set; }

	public string Uri { get; set; }

	public Rectangle CropRect { get; set; }
	
	public RectangleF Bounds { get; set; }
}
