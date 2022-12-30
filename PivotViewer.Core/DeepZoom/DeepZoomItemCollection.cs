using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace PivotViewer.Core.Data;

public class DeepZoomItemCollection : IReadOnlyList<DeepZoomItem>
{
	private readonly List<DeepZoomItem> items = new();
	private readonly Dictionary<string, DeepZoomItem> itemsById = new();

	public DeepZoomItem this[int index] => items[index];

	public DeepZoomItem this[string id] => itemsById[id];

	public int Count => items.Count;

	public void Add(DeepZoomItem item)
	{
		items.Add(item);
		itemsById.Add(item.Id, item);
	}

	public void Remove(DeepZoomItem item)
	{
		items.Remove(item);
		itemsById.Remove(item.Id);
	}

	public void Clear()
	{
		items.Clear();
		itemsById.Clear();
	}

	public bool TryGet(string id, [MaybeNullWhen(false)] out DeepZoomItem item) =>
		itemsById.TryGetValue(id, out item);

	public IEnumerator<DeepZoomItem> GetEnumerator() =>
		items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
