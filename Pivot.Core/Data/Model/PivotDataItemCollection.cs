using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Pivot.Data.Model;

/// <summary>
/// This 
/// </summary>
public class PivotDataItemCollection : IReadOnlyList<PivotDataItem>
{
	private readonly List<PivotDataItem> dataItems = new();
	private readonly Dictionary<string, PivotDataItem> dataItemsById = new();

	public PivotDataItem this[int index] => dataItems[index];

	public PivotDataItem this[string id] => dataItemsById[id];

	public int Count => dataItems.Count;

	public void Add(PivotDataItem item)
	{
		dataItems.Add(item);
		dataItemsById.Add(item.Id, item);
	}

	public void AddRange(IEnumerable<PivotDataItem> items)
	{
		dataItems.AddRange(items);
		foreach (var item in items)
		{
			dataItemsById.Add(item.Id, item);
		}
	}

	public void Remove(PivotDataItem item)
	{
		dataItems.Remove(item);
		dataItemsById.Remove(item.Id);
	}

	public void Clear()
	{
		dataItems.Clear();
		dataItemsById.Clear();
	}

	public bool TryGet(string id, [MaybeNullWhen(false)] out PivotDataItem item) =>
		dataItemsById.TryGetValue(id, out item);

	public IEnumerator<PivotDataItem> GetEnumerator() =>
		dataItems.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
