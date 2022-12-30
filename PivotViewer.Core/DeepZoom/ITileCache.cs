namespace PivotViewer.Core.DeepZoom;

public interface ITileCache<T>
{
	T? Add(T tile);

	void Remove(T tile);

	void Refresh(T tile);
}
