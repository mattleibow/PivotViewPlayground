namespace PivotView.Core.Layout;

public class PivotOutOfFrameLayout : PivotLayout
{
    private bool isAdding;

    public PivotOutOfFrameLayout(bool isAdding)
    {
        this.isAdding = isAdding;
    }

    protected override void OnLayoutItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
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
