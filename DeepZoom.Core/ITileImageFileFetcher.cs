namespace DeepZoom.Core;

public interface ITileImageFileFetcher
{
	Task<object?> FetchAsync(string uri, CancellationToken cancellationToken = default);
}
