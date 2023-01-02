namespace DeepZoom.Core;

public interface IDeepZoomFileFetcher
{
	Task<Stream> FetchAsync(string uri, CancellationToken cancellationToken = default);
}
