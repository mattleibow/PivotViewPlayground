namespace PivotViewer.Core.DeepZoom;

public class DefaultFileLoader : IFileLoader
{
	private readonly HttpClient httpClient = new();

	public async Task<Stream> LoadAsync(string uri, CancellationToken cancellationToken = default)
	{
		if (File.Exists(uri))
			return File.OpenRead(uri);

		return await httpClient.GetStreamAsync(uri, cancellationToken);
	}
}
