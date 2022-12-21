using System.Collections;
using System.Diagnostics;

namespace PivotViewer.Core.Data;

[DebuggerDisplay("{DebuggerValue}")]
public class PivotPropertyValueCollection : IReadOnlyList<IComparable>
{
	private readonly IList<IComparable> values;

	public PivotPropertyValueCollection(IComparable value)
	{
		values = new List<IComparable>() { value };
	}

	public PivotPropertyValueCollection(params IComparable[] values)
	{
		this.values = new List<IComparable>(values);
	}

	public PivotPropertyValueCollection(IEnumerable<IComparable> values)
	{
		this.values = new List<IComparable>(values);
	}

	public IComparable this[int index] => values[index];

	public int Count => values.Count;

	public void Add(IComparable value) =>
		values.Add(value);

	internal string? DebuggerValue =>
		Count switch
		{
			0 => null,
			1 => $"{values[0]}",
			_ => $"{values[0]}, ..."
		};

	public IEnumerator<IComparable> GetEnumerator() =>
		values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
