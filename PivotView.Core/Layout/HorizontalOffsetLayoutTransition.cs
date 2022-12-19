namespace PivotView.Core.Layout;

public class HorizontalOffsetLayoutTransition : OffScreenLayoutTransition
{
	public int OffScreenOffset { get; set; } = 50;

	public override RectangleF GetOffScreenFrame(RectangleF onscreenFrame, RectangleF frame) =>
		new(frame.Right + OffScreenOffset, onscreenFrame.Y, onscreenFrame.Width, onscreenFrame.Height);
}
