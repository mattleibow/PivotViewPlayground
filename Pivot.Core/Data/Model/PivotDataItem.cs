using System.Diagnostics;

namespace Pivot.Data.Model;

/// <summary>
/// Represents a single 
/// </summary>
[DebuggerDisplay("{Id}")]
public class PivotDataItem
{
	public PivotDataItem()
	{
	}

	public string? Id { get; set; }

	public float ImageWidth { get; set; } = 0;

	public float ImageHeight { get; set; } = 0;

	public PivotDataItemPropertyCollection? Properties { get; set; } = new();
}
