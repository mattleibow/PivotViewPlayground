using System.Diagnostics;

namespace Pivot.Data.Model;

/// <summary>
/// Describes a property of a <see cref="PivotDataItem"/>.
/// </summary>
[DebuggerDisplay("{Type}: {Name}")]
public class PivotProperty
{
	public PivotProperty(string name)
	{
		Name = name;
	}

	public string Name { get; set; }

	public string? Format { get; set; }

	public string? Description { get; set; }

	public PivotPropertyType Type { get; set; }

	public bool IsFilterVisible { get; set; } = true;

	public bool IsMetaDataVisible { get; set; } = true;

	public bool IsSearchVisible { get; set; } = true;

	public bool IsVisible => IsFilterVisible || IsMetaDataVisible || IsSearchVisible;
}
