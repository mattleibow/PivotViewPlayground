namespace PivotView.Core.Layout;

abstract public class PivotLayout
{
    public void LayoutItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
        OnLayoutItems(items, frame);
    }

    protected abstract void OnLayoutItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame);
}
