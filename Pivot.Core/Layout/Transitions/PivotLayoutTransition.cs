namespace Pivot.Layout.Transitions;

public abstract class PivotLayoutTransition
{
	public abstract void ArrangeItems(IReadOnlyList<PivotVisualizationItem> items, RectangleF frame, PivotLayoutTransitionType transitionType);
}

public enum PivotLayoutTransitionType
{
	Enter,
	Exit,
}
