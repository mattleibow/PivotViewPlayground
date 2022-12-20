using System.Collections;
using System.Diagnostics;

namespace PivotViewer.Core.Data;

[DebuggerDisplay("{DebuggerValue}")]
public class PivotDataPropertyValue : IEnumerable<IComparable>
{
	private readonly IList<IComparable> values;

	public PivotDataPropertyValue(IList<IComparable> values)
	{
		this.values = values ?? throw new ArgumentNullException(nameof(values));
	}

	public PivotDataPropertyValue(IComparable value)
	{
		values = new List<IComparable>() { value };
	}

	public IComparable this[int index] => values[index];

	public int Count => values.Count;

	public void Add(IComparable value) =>
		values.Add(value);

	internal string? DebuggerValue =>
		Count switch
		{
			0 => null,
			1 => values[0]?.ToString(),
			_ => $"{values[0]}, ..."
		};

	public IEnumerator<IComparable> GetEnumerator() =>
		values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
