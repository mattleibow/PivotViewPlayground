namespace PivotView.Core.Layout;

public class PivotOutOfFrameLayout : PivotLayout
{
    private const int OffscreenScale = 2;

    private bool isAdding;

    public PivotOutOfFrameLayout(bool isAdding)
    {
        this.isAdding = isAdding;
    }

    public override void Measure(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
    }

    public override void Arrange(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
    {
        foreach (var item in items)
        {
            var frameProp = item.Frame;

            // get onscreen frame and center
            var onscreenFrame = isAdding
                ? frameProp.Desired
                : frameProp.Current;
            var onscreenCenter = new PointF(
                onscreenFrame.X + onscreenFrame.Width / 2,
                onscreenFrame.Y + onscreenFrame.Height / 2);

            // get offscreen frame and buffer size
            var offscreenSize = onscreenFrame.Size * OffscreenScale;
            var buffer = new SizeF(
                offscreenSize.Width / 2,
                offscreenSize.Height / 2);

            // get center of "view"
            var clipRect = frame; // TODO: handle pan and zoom
            var clipCenter = new PointF(
                clipRect.X + clipRect.Width / 2,
                clipRect.Y + clipRect.Height / 2);
            var normClipRect = new RectangleF(
                clipRect.X - clipCenter.X,
                clipRect.Y - clipCenter.Y,
                clipRect.Width,
                clipRect.Height);

            // normalize the onscreen center
            var normOnscreenCenter = new PointF(
                onscreenCenter.X - clipCenter.X,
                onscreenCenter.Y - clipCenter.Y);

            // calculate the slope from the center to the onscreen position
            var direction = (double)normOnscreenCenter.Y / normOnscreenCenter.X;
            if (double.IsNaN(direction))
                direction = double.PositiveInfinity;

            // find the offscreen coordinates
            var normX = normOnscreenCenter.X > 0
                ? normClipRect.Right + buffer.Width + normOnscreenCenter.X
                : normClipRect.Left - buffer.Width + normOnscreenCenter.X;
            var normY = normOnscreenCenter.Y > 0
                ? normClipRect.Bottom + buffer.Height + normOnscreenCenter.Y
                : normClipRect.Top - buffer.Height + normOnscreenCenter.Y;
            
            // calculate the new normalized position
            var newNormY = direction * normX;
            var newNormOnscreenCenter = Math.Abs(newNormY) > Math.Abs(normY)
                ? new PointF((float)(normY / direction), (float)(normY))
                : new PointF((float)(normX), (float)(newNormY));

            // denormalize
            var denormOffscreenCenter = new PointF(
                newNormOnscreenCenter.X + clipCenter.X,
                newNormOnscreenCenter.Y + clipCenter.Y);

            // calculate final offscreen frame
            var newFrame = new RectangleF(
                denormOffscreenCenter.X - offscreenSize.Width / 2,
                denormOffscreenCenter.Y - offscreenSize.Height / 2,
                offscreenSize.Width,
                offscreenSize.Height);

            // set destination frame
            if (isAdding)
                frameProp.Current = newFrame;
            else
                frameProp.Desired = newFrame;
        }
    }
}
