namespace PivotView.Core.Animation;

public delegate double EasingDelegate(double progress);

static public class Easing
{
    public static readonly EasingDelegate Linear = new(x => x);
}
