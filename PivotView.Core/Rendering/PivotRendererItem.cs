namespace PivotView.Core.Rendering;

public class PivotRendererItem
{
    public PivotRendererItem(PivotDataItem dataItem)
    {
        DataItem = dataItem;
    }

    public string? Name => DataItem.Name;

    public PivotDataItem DataItem { get; }

    /// <summary>
    /// Width / Height
    /// </summary>
    public double AspectRatio =>
        DataItem.ImageWidth == 0 || DataItem.ImageHeight == 0
            ? 1.0f
            : (double)DataItem.ImageWidth / DataItem.ImageHeight;

    public AnimatableProperty<RectangleF> Frame { get; } = new(default);
}
