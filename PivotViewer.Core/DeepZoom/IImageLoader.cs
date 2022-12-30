namespace PivotViewer.Core.DeepZoom;

public interface IImageLoader
{
	Task<object?> LoadAsync(string uri, CancellationToken cancellationToken = default);
}
