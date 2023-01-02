namespace Pivot.Core.Data;

public interface ICxmlFileFetcher
{
	Task<Stream> FetchAsync(string uri, CancellationToken cancellationToken = default);
}
