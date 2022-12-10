namespace PivotView.Core.Layout;

public class PivotVerticalStackLayout : PivotLayout
{
    public float ItemSpacing { get; set; }

    protected override void OnLayoutItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
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
