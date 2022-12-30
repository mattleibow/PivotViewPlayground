namespace PivotViewer.Core.DeepZoom;

public interface IFileLoader
{
	Task<Stream> LoadAsync(string uri, CancellationToken cancellationToken = default);
}
