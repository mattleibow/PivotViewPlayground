using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace PivotViewer.Core.Data;

public class FilterPropertyCollection : IReadOnlyList<FilterProperty>
{
	private readonly List<FilterProperty> properties = new();
	private readonly Dictionary<string, FilterProperty> propertiesByName = new();

	public FilterProperty this[int index] => properties[index];

	public FilterProperty this[string name] => propertiesByName[name];

	public int Count => properties.Count;

	public void Add(FilterProperty item)
	{
		properties.Add(item);
		propertiesByName.Add(item.Name, item);
	}

	public void Remove(FilterProperty item)
	{
		properties.Remove(item);
		propertiesByName.Remove(item.Name);
	}

	public void Clear()
	{
		properties.Clear();
		propertiesByName.Clear();
	}

	public bool TryGet(string propertyName, [MaybeNullWhen(false)] out FilterProperty applied) =>
		propertiesByName.TryGetValue(propertyName, out applied);

	public IEnumerator<FilterProperty> GetEnumerator() =>
		properties.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
