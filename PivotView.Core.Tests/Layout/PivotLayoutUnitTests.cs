namespace PivotView.Core.Tests;

public partial class PivotLayoutUnitTests
{
    private static List<PivotRendererItem> CreateItemList(params string[] items) =>
        items.Select(i => new PivotRendererItem(CreateDataItem(i))).ToList();

    private static PivotDataItem CreateDataItem(string name, float width = 100, float height = 100) =>
        new()
        {
            Id = name,
            ImageWidth = width,
            ImageHeight = height,
        };
}
