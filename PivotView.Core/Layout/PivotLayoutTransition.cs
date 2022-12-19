namespace PivotView.Core.Layout;

public abstract class PivotLayoutTransition
{
	public abstract void ArrangeItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame, PivotLayoutTransitionType transitionType);
}

public enum PivotLayoutTransitionType
{
	Enter,
	Exit,
}
