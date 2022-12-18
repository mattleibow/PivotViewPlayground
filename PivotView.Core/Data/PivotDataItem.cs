using System.Diagnostics;

namespace PivotView.Core.Data;

[DebuggerDisplay("{Id}")]
public class PivotDataItem
{
    public string? Id { get; set; }

    public float ImageWidth { get; set; } = 0;

    public float ImageHeight { get; set; } = 0;

    public IList<PivotDataProperty>? Properties { get; set; } = new List<PivotDataProperty>();
}
