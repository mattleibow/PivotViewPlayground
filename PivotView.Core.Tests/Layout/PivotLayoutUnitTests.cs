namespace PivotView.Core.Tests;

public partial class PivotLayoutUnitTests
{
    private static List<PivotRendererItem> CreateItemList(params string[] items) =>
        items.Select(i => new PivotRendererItem(new PivotDataItem { Name = i })).ToList();
}
