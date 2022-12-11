﻿namespace PivotView.Core.Layout;

abstract public class PivotLayout
{
    public int ItemCountOverride { get; set; }

    public double ItemAspectRatioOverride { get; set; }

    public double ItemMargin { get; set; }

    /// <summary>
    /// Width/Height
    /// </summary>
    public double ItemAspectRatio { get; protected set; } = 1.0;

    public double ItemWidth { get; protected set; }

    public double ItemHeight { get; protected set; }

    public double LayoutWidth { get; protected set; }

    public double LayoutHeight { get; protected set; }

    public void LayoutItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
        Measure(items, frame);
        Arrange(items, frame);
    }

    public abstract void Measure(IReadOnlyList<PivotRendererItem> items, RectangleF frame);

    public abstract void Arrange(IReadOnlyList<PivotRendererItem> items, RectangleF frame);

    protected double GetItemAspectRatio(IReadOnlyList<PivotRendererItem> items)
    {
        if (ItemAspectRatioOverride != 0.0)
            return ItemAspectRatioOverride;

        // just use the first item for now
        if (items.Count > 0)
            return items[0].AspectRatio;

        return 1.0;
    }
}
