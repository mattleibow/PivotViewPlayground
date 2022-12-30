namespace PivotViewer.Core.DeepZoom;

public class DefaultImageLoader : IImageLoader
{
	private readonly IFileLoader fileLoader;

	public DefaultImageLoader(IFileLoader? fileLoader = default)
	{
		this.fileLoader = fileLoader ?? new DefaultFileLoader();
	}

	public async Task<object?> LoadAsync(string uri, CancellationToken cancellationToken = default)
	{
		var stream = await fileLoader.LoadAsync(uri, cancellationToken);
		return stream;
	}
}
