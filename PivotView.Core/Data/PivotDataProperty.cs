using System.Diagnostics;

namespace PivotView.Core.Data;

[DebuggerDisplay("{Name} = {Value}")]
public class PivotDataProperty
{
	public PivotDataProperty(string name, object value)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Value = value ?? throw new ArgumentNullException(nameof(name));
	}

	public string Name { get; }

	public object Value { get; }
}
