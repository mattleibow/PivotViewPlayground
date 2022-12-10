namespace PivotView.Core.Animation;

public class AnimationStep
{
    private readonly List<AnimationStepItem> stepItems = new();

    public AnimationStep(TimeSpan duration, EasingDelegate easing)
    {
        Duration = duration;
        Easing = easing;
    }

    public TimeSpan Progress { get; private set; }

    public TimeSpan Duration { get; set; }

    public EasingDelegate Easing { get; }

    public Action? OnComplete { get; set; }

    public TimeSpan Update(TimeSpan delta)
    {
        var remaining = Duration - Progress;
        var actualDelta = delta > remaining
            ? remaining
            : delta;

        var progress = Progress + actualDelta;
        var seconds = progress.TotalSeconds;

        var actualSeconds = Easing(seconds);

        foreach (var item in stepItems)
            item.Update(actualSeconds);

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
    public abstract void Update(double progress);
}

public record AnimationStepItem<TValue>(AnimatableProperty<TValue?> Property, TValue? Start, TValue? End) : AnimationStepItem
{
    public override void Update(double progress)
    {
        var lerp = Lerping.Lerps[typeof(TValue)];
        var value = (TValue)lerp(Start, End, progress);
        Property.Current = value;
    }
}

public class AnimationSet
{
    private readonly LinkedList<AnimationStep> steps = new();

    private AnimationStep? currentStep;
    private TimeSpan currentDuration = TimeSpan.Zero;

    public AnimationSet()
    {
    }

    public AnimationStep? Current => currentStep;

    public void Update(TimeSpan delta)
    {
        if (delta < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delta), "The animation cannot go backwards. Ensure the delta value is positive.");

        // get the corrent or next step depending
        if (currentStep is null && steps.First is not null)
        {
            currentStep = steps.First.Value;
            steps.RemoveFirst();
        }

        // if there is nothing to do, bail out
        if (currentStep is null)
            return;

        var remainingDelta = currentStep.Update(delta);

        currentDuration += delta;
    }

    public void Complete()
    {
        // TODO: set all values to their final values

        steps.Clear();

        currentStep = null;
        currentDuration = TimeSpan.Zero;
    }
}

public class AnimatableProperty<T>
{
    public AnimatableProperty(T? current = default)
    {
        Current = current;
    }

    public T? Current { get; set; }

    public T? Desired { get; set; }
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
    public Action<TimeSpan>? Tick { get; set; }
}

public class TestTicker : Ticker
{
    public void PerformTick(TimeSpan delta)
    {
        Tick?.Invoke(delta);
    }
}

