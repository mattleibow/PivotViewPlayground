namespace DeepZoom.Core;

internal static class DeepZoomExtensions
{
	public static string? GetTileBaseUri(string? sourceUri) =>
		UriExtensions.GetUriWithoutExtension(sourceUri) + "_files";

	public static string? GetBaseUri(string? sourceUri) =>
		UriExtensions.GetBaseUri(sourceUri);
}
