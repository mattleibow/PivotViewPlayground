using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace PivotViewer.Core.Data;

public class PivotDataItemCollection : IReadOnlyList<PivotDataItem>
{
	private readonly List<PivotDataItem> items = new();
	private readonly Dictionary<string, PivotDataItem> itemsById = new();

	public PivotDataItem this[int index] => items[index];

	public PivotDataItem this[string id] => itemsById[id];

	public int Count => items.Count;

	public void Add(PivotDataItem item)
	{
		items.Add(item);
		itemsById.Add(item.Id, item);
	}

	public void Remove(PivotDataItem item)
	{
		items.Remove(item);
		itemsById.Remove(item.Id);
	}

	public void Clear()
	{
		items.Clear();
		itemsById.Clear();
	}

	public bool TryGet(string id, [MaybeNullWhen(false)] out PivotDataItem item) =>
		itemsById.TryGetValue(id, out item);

	public IEnumerator<PivotDataItem> GetEnumerator() =>
		items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
