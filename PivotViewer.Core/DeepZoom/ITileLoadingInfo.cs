namespace PivotViewer.Core.DeepZoom;

public interface ITileLoadingInfo
{
	string Uri { get; }

	TileLoadState State { get; }

	object? Image { get; }
}
