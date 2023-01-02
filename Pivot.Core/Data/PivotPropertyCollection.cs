using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Pivot.Core.Data;

public class PivotPropertyCollection : IReadOnlyList<PivotProperty>
{
	private readonly List<PivotProperty> properties = new();
	private readonly Dictionary<string, PivotProperty> propertiesByName = new();

	public PivotProperty this[int index] => properties[index];

	public PivotProperty this[string name] => propertiesByName[name];

	public int Count => properties.Count;

	public void Add(PivotProperty item)
	{
		properties.Add(item);
		propertiesByName.Add(item.Name, item);
	}

	public void Remove(PivotProperty item)
	{
		properties.Remove(item);
		propertiesByName.Remove(item.Name);
	}

	public void Clear()
	{
		properties.Clear();
		propertiesByName.Clear();
	}

	public bool TryGet(string propertyName, [MaybeNullWhen(false)] out PivotProperty item) =>
		propertiesByName.TryGetValue(propertyName, out item);

	public IEnumerator<PivotProperty> GetEnumerator() =>
		properties.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
