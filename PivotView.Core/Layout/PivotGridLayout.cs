namespace PivotView.Core.Layout;

public class PivotGridLayout : PivotLayout
{
    public int Columns { get; protected set; }

    public int Rows { get; protected set; }

    public override void Measure(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
        // 1. Get the aspect ratio of the items
        ItemAspectRatio = GetItemAspectRatio(items);

        // 2.Calculate the smallest/best area to lay out the items
        var count = Math.Max(items.Count, ItemCountOverride);
        // start of with one tall columns with all the items
        var columns = 1;
        var rows = count;
        // calculate the max size of each item to fit
        var maxItemHeight = (double)frame.Height / rows;
        var maxItemWidth = ItemAspectRatio * maxItemHeight;
        // loop while there is enough space to add one more column
        while (frame.Width - (columns + 1) * maxItemWidth >= 0.0 && columns < count)
        {
            // add another column
            columns++;
            rows = (int)Math.Ceiling((double)count / columns);
            // calculate the max size of each item to fit
            maxItemHeight = (double)frame.Height / rows;
            maxItemWidth = ItemAspectRatio * maxItemHeight;
        }

        // 3. Calculate the general item size
        ItemHeight = frame.Height / (double)rows;
        ItemWidth = ItemHeight * ItemAspectRatio;
        if (ItemWidth * columns > frame.Width)
        {
            ItemWidth = (double)frame.Width / columns;
            ItemHeight = ItemWidth / ItemAspectRatio;
        }

        // 4. Calculate the number of rows/cols
        Columns = Math.Min(columns, items.Count);
        Rows = (int)Math.Ceiling(items.Count / (double)Columns);

        // 5. Calculate the total items area
        LayoutWidth = ItemWidth * Columns;
        LayoutHeight = ItemHeight * Rows;
    }

    public override void Arrange(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
        foreach (var item in items)
        {
        }
    }
}
