using System.Diagnostics;

namespace Pivot.Core.Data;

[DebuggerDisplay("{Id}")]
public class PivotDataItem
{
	public string? Id { get; set; }

	public float ImageWidth { get; set; } = 0;

	public float ImageHeight { get; set; } = 0;

	public PivotDataItemPropertyCollection? Properties { get; set; } = new();
}
