namespace PivotView.Core.Animation;

public class AnimationStep
{
    private readonly List<AnimationStepItem> stepItems = new();

    public AnimationStep(TimeSpan duration, EasingDelegate easing)
    {
        Duration = duration;
        Easing = easing;
    }

    public TimeSpan Duration { get; }

    public EasingDelegate Easing { get; }

    public Action? OnComplete { get; set; }

    public TimeSpan Progress { get; private set; }

    public TimeSpan Update(TimeSpan delta)
    {
        var remaining = Duration - Progress;
        var actualDelta = delta > remaining
            ? remaining
            : delta;

        Progress += actualDelta;

        var seconds = Progress.TotalSeconds;
        var actualSeconds = Easing(seconds);

        foreach (var item in stepItems)
            item.SetProgress(actualSeconds);

        return delta - actualDelta;
    }

    public void Add<TValue>(AnimatableProperty<TValue?> property) =>
        Add(property, property.Current, property.Desired);

    public void Add<TValue>(AnimatableProperty<TValue?> property, TValue? value) =>
        Add(property, property.Current, value);

    public void Add<TValue>(AnimatableProperty<TValue?> property, TValue? start, TValue? value)
    {
        var stepItem = new AnimationStepItem<TValue>(property, start, value);
        stepItems.Add(stepItem);
    }
}

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

public class Animator
{
    private readonly Ticker ticker;

    public Animator(Ticker ticker)
    {
    }

    public void Tick()
    {
    }
}

public class Ticker
{
    protected virtual void OnTick(TimeSpan delta)
    {
        Tick?.Invoke(delta);
    }

    public Action<TimeSpan>? Tick { get; set; }
}
