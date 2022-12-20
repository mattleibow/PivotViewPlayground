using System.Diagnostics;

namespace PivotViewer.Core.Data;

[DebuggerDisplay("{Type}: {Name}")]
public class PivotDataProperty
{
	public string? Name { get; set; }
	
	public string? Format { get; set; }

	public string? Description { get; set; }

	public PivotDataPropertyType Type { get; set; }
}

public enum PivotDataPropertyType
{
	Text,
	Number,
	DateTime,
	Boolean,
}
