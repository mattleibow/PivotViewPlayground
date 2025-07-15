namespace Pivot.Data.Sources.Cxml;

public interface ICxmlFileFetcher
{
	Task<Stream> FetchAsync(string uri, CancellationToken cancellationToken = default);
}
