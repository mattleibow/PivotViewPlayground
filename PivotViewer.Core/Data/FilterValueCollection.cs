using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PivotViewer.Core.Data;

[DebuggerDisplay("{DebuggerValue}")]
public class FilterValueCollection : IReadOnlyList<FilterValue>
{
	private readonly List<FilterValue> values = new();
	private readonly Dictionary<IComparable, FilterValue> valuesByValue = new();

	public FilterValue this[int index] => values[index];

	public int Count => values.Count;

	internal string? DebuggerValue =>
		Count switch
		{
			0 => null,
			1 => $"{values[0].Value}",
			_ => $"{values[0].Value},..."
		};

	public void Add(FilterValue item)
	{
		values.Add(item);
		valuesByValue.Add(item.Value, item);
	}

	public void Remove(FilterValue item)
	{
		values.Remove(item);
		valuesByValue.Remove(item.Value);
	}

	public void Clear()
	{
		values.Clear();
		valuesByValue.Clear();
	}

	public FilterValue Get(IComparable value) =>
		valuesByValue[value];

	public bool Has(IComparable value) =>
		valuesByValue.ContainsKey(value);

	public bool TryGet(IComparable value, [MaybeNullWhen(false)] out FilterValue current) =>
		valuesByValue.TryGetValue(value, out current);

	public IEnumerator<FilterValue> GetEnumerator() =>
		values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
