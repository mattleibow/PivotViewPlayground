using System.Drawing;

namespace PivotView.Core.Tests;

abstract class PivotLayout
{
    public void LayoutItems(IReadOnlyList<PlaceholderItem> items, RectangleF frame)
    {
        OnLayoutItems(items, frame);
    }

    protected abstract void OnLayoutItems(IReadOnlyList<PlaceholderItem> items, RectangleF frame);
}

class PivotGridLayout : PivotLayout
{
    protected override void OnLayoutItems(IReadOnlyList<PlaceholderItem> items, RectangleF frame)
    {
        foreach (var item in items)
        { }
    }
}

class PivotOutOfFrameLayout : PivotLayout
{
    private bool isAdding;

    public PivotOutOfFrameLayout(bool isAdding)
    {
        this.isAdding = isAdding;
    }

    protected override void OnLayoutItems(IReadOnlyList<PlaceholderItem> items, RectangleF frame)
    {
        foreach (var item in items)
        {
            var frameProp = item.Frame;

            var current = isAdding
                ? frameProp.Desired
                : frameProp.Current;

            var newFrame = new RectangleF(frame.Width + 50, current.Y, current.Width, current.Height);

            if (isAdding)
                frameProp.Current = newFrame;
            else
                frameProp.Desired = newFrame;
        }
    }
}

class PivotVerticalStackLayout : PivotLayout
{
    public float ItemSpacing { get; set; }

    protected override void OnLayoutItems(IReadOnlyList<PlaceholderItem> items, RectangleF frame)
    {
        var totalSpacing = (items.Count - 1) * ItemSpacing;
        var availableSpace = frame.Height - totalSpacing;

        var itemHeight = availableSpace / items.Count;

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];

            var newFrame = new RectangleF(0, i * (itemHeight + ItemSpacing), frame.Width, itemHeight);

            item.Frame.Desired = newFrame;
        }
    }
}
