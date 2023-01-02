using System.Diagnostics;

namespace Pivot.Core.Data;

[DebuggerDisplay("{Value}")]
public class FilterValue
{
	public FilterValue(FilterProperty category, IComparable value)
	{
		Property = category;
		Value = value;
	}

	public FilterProperty Property { get; }

	public IComparable Value { get; }

	public int Count { get; set; }
}
