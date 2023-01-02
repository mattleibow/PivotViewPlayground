namespace Pivot.Core.Layout;

public class ExplosionLayoutTransition : OffScreenLayoutTransition
{
	public int OffScreenScale { get; set; } = 2;

	public override RectangleF GetOffScreenFrame(RectangleF onscreenFrame, RectangleF frame)
	{
		// get onscreen frame and center
		var onscreenCenter = new PointF(
			onscreenFrame.X + onscreenFrame.Width / 2,
			onscreenFrame.Y + onscreenFrame.Height / 2);

		// get offscreen frame and buffer size
		var offscreenSize = onscreenFrame.Size * OffScreenScale;
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
		var slopeFromCenter = (double)normOnscreenCenter.Y / normOnscreenCenter.X;
		if (double.IsNaN(slopeFromCenter))
			slopeFromCenter = double.PositiveInfinity;

		// find the offscreen coordinates
		var normX = normOnscreenCenter.X > 0
			? normClipRect.Right + buffer.Width + normOnscreenCenter.X
			: normClipRect.Left - buffer.Width + normOnscreenCenter.X;
		var normY = normOnscreenCenter.Y > 0
			? normClipRect.Bottom + buffer.Height + normOnscreenCenter.Y
			: normClipRect.Top - buffer.Height + normOnscreenCenter.Y;

		// calculate the new normalized position
		var newNormY = slopeFromCenter * normX;
		var newNormOnscreenCenter = Math.Abs(newNormY) > Math.Abs(normY)
			? new PointF((float)(normY / slopeFromCenter), (float)(normY))
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

		return newFrame;
	}
}
