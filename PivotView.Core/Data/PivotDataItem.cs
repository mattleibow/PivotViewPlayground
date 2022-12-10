using System.Diagnostics;

namespace PivotView.Core.Data;

[DebuggerDisplay("{Name}")]
public class PivotDataItem
{
    public string? Name { get; set; }

    public float ImageWidth { get; set; } = 0;

    public float ImageHeight { get; set; } = 0;

    public IList<PivotDataProperty>? Properties { get; set; } = new List<PivotDataProperty>();
}
