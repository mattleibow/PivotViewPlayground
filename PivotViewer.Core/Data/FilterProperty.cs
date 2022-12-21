using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PivotViewer.Core.Data;

[DebuggerDisplay("{Name}={Values}")]
public class FilterProperty
{
	private readonly FilterValueCollection values = new();

	public FilterProperty(PivotProperty property)
	{
		PivotProperty = property;
	}

	public PivotProperty PivotProperty { get; }

	public string Name => PivotProperty.Name;

	public int Count => values.Count;

	public FilterValueCollection Values => values;

	public void ResetValues()
	{
		values.Clear();
	}

	public FilterValue GetFilterValue(IComparable value) =>
		Values.Get(value);

	public bool TryGetFilterValue(IComparable value, [MaybeNullWhen(false)] out FilterValue filterValue) =>
		Values.TryGet(value, out filterValue);

	public bool HasValue(IComparable value) =>
		Values.Has(value);

	public void AddValue(IComparable value) =>
		IncrementValue(value, 1);

	public void RemoveValue(IComparable value) =>
		IncrementValue(value, -1);

	public void IncrementValue(IComparable value, int change = 1)
	{
		if (values.TryGet(value, out var current))
		{
			var newCount = current.Count + change;
			if (newCount < 0)
				throw new InvalidOperationException($"Unable to decrease '{value}' count from '{current}' by '{change}'.");

			if (newCount > 0)
				current.Count = newCount;
			else
				values.Remove(current);
		}
		else if (change > 0)
		{
			values.Add(new FilterValue(this, value) { Count = change });
		}
		else if (change < 0)
		{
			throw new InvalidOperationException($"Unable to initialize '{value}' to '{change}'.");
		}
	}
}
