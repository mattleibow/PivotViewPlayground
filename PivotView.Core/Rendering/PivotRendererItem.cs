namespace PivotView.Core.Rendering;

public class PivotRendererItem
{
    public PivotRendererItem(PivotDataItem dataItem)
    {
        DataItem = dataItem;
    }

    public string? Name => DataItem.Name;

    public PivotDataItem DataItem { get; }

    public AnimatableProperty<RectangleF> Frame { get; } = new(default);
}
