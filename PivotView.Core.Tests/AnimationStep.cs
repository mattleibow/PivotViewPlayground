namespace PivotView.Core.Tests;

class AnimationStep
{
    private readonly List<AnimationStepItem> stepItems = new List<AnimationStepItem>();

    public AnimationStep(TimeSpan duration)
    {
        Duration = duration;
    }

    public TimeSpan Duration { get; set; }

    public Action? OnComplete { get; set; }

    public TimeSpan Update(TimeSpan delta)
    {
        return TimeSpan.Zero;
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

abstract record AnimationStepItem();

record AnimationStepItem<TValue>(AnimatableProperty<TValue?> property, TValue? start, TValue? end) : AnimationStepItem;

class AnimationSet
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

class AnimatableProperty<T>
{
    public AnimatableProperty(T? current = default)
    {
        Current = current;
    }

    public T? Current { get; set; }

    public T? Desired { get; set; }
}
