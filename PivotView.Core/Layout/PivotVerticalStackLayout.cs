namespace PivotView.Core.Layout;

public class PivotVerticalStackLayout : PivotLayout
{
    public override void Measure(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
        // 1. Get the aspect ratio of the items
        ItemAspectRatio = GetItemAspectRatio(items);

        // 2.Calculate the smallest/best area to lay out the items
        var count = Math.Max(items.Count, ItemCountOverride);
        // TODO: be a bit smarter if there is a few wide items

        // 3. Calculate the general item size
        ItemHeight = frame.Height / (double)count;
        ItemWidth = ItemHeight * ItemAspectRatio;

        // 4. Calculate the total items area
        LayoutWidth = ItemWidth;
        LayoutHeight = ItemHeight * count;
    }

    public override void Arrange(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];

            var newFrame = new RectangleF(
                (float)(frame.X + ItemMargin),
                (float)(frame.Y + ItemMargin + (ItemHeight * i)),
                (float)(ItemWidth - ItemMargin - ItemMargin),
                (float)(ItemHeight - ItemMargin - ItemMargin));

            item.Frame.Desired = newFrame;
        }
    }
}
