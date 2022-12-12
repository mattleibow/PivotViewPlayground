namespace PivotView.Core.Animation;

public abstract record AnimationStepItem()
{
    public abstract void SetProgress(double progress);
}

public record AnimationStepItem<TValue>(AnimatableProperty<TValue?> Property, TValue? Start, TValue? End) : AnimationStepItem
{
    public override void SetProgress(double progress)
    {
        var lerp = Lerping.Lerps[typeof(TValue)];
        var value = (TValue)lerp(Start, End, progress);
        Property.Current = value;
    }
}
