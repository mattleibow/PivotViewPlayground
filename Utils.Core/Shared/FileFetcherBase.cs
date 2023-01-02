#if DEEPZOOM_CORE
namespace DeepZoom.Core;
#elif PIVOT_CORE
namespace Pivot.Core;
#else
namespace Utils.Core;
#endif

public abstract class FileFetcherBase
{
	private readonly HttpClient httpClient = new();

	public async Task<Stream> FetchAsync(string uri, CancellationToken cancellationToken = default)
	{
		if (File.Exists(uri))
			return File.OpenRead(uri);

		return await httpClient.GetStreamAsync(uri, cancellationToken);
	}
}
