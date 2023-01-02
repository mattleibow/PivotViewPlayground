namespace Pivot.Core.Layout;

public abstract class OffScreenLayoutTransition : PivotLayoutTransition
{
	public override void ArrangeItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame, PivotLayoutTransitionType transitionType)
	{
		foreach (var item in items)
		{
			var frameProp = item.Frame;

			var onscreenFrame = transitionType == PivotLayoutTransitionType.Enter
				? frameProp.Desired
				: frameProp.Current;

			var newFrame = GetOffScreenFrame(onscreenFrame, frame);

			if (transitionType == PivotLayoutTransitionType.Enter)
				frameProp.Current = newFrame;
			else
				frameProp.Desired = newFrame;
		}
	}

	public abstract RectangleF GetOffScreenFrame(RectangleF onscreenFrame, RectangleF frame);
}
