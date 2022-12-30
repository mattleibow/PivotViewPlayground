namespace PivotViewer.Core.DeepZoom;

public static class DeepZoomExtensions
{
	public static string? GetTileBaseUri(string? sourceUri)
	{
		if (sourceUri is null)
			return null;

		sourceUri = sourceUri.TrimEnd('/', '\\');

		var parts = sourceUri.Split('/', '\\');
		var lastI = parts.Length - 1;
		var filename = parts[lastI];
		var lastDotI = filename.LastIndexOf(".");

		if (lastDotI > -1)
			parts[lastI] = filename.Substring(0, lastDotI);

		var baseUri = string.Join("/", parts);

		return baseUri + "_files";
	}

	public static string? GetBaseUri(string? sourceUri)
	{
		if (sourceUri is null)
			return null;

		sourceUri = sourceUri.TrimEnd('/', '\\');

		var parts = sourceUri.Split('/', '\\');
		var lastI = parts.Length - 1;

		return string.Join("/", parts, 0, lastI);
	}

	public static int GetMortonNumber(int x, int y)
	{
		var result = 0;

		for (var i = 0; i < 16; i++)
		{
			result |=
				((x & (1 << i)) << i) |
				((y & (1 << i)) << (i + 1));
		}

		return result;
	}

	public static Point GetMortonPoint(int morton)
	{
		var x = 0;
		var y = 0;

		for (var i = 0; i < 16; i++)
		{
			x |= (morton & (1 << (2 * i))) >> i;
			y |= (morton & (1 << (2 * i + 1))) >> (i + 1);
		}

		return new Point(x, y);
	}
}
