#if DEEPZOOM_CORE
namespace DeepZoom.Core;
#elif PIVOT_CORE
namespace Pivot.Core;
#else
namespace Utils.Core;
#endif

internal static class UriExtensions
{
	public static string? GetUriWithoutExtension(string? sourceUri)
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

		return string.Join("/", parts);
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
}
