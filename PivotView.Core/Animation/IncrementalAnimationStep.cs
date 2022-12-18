using System.Collections;
using System.Diagnostics;

namespace PivotView.Core.Animation;

[DebuggerDisplay("Incremental: Name = {Name}, IsComplete = {IsComplete}")]
public class IncrementalAnimationStep : IAnimationStep, IEnumerable<AnimationStepItem>
{
    private readonly List<AnimationStepItem> stepItems = new();

    public IncrementalAnimationStep(TimeSpan duration)
        : this(duration, Animation.Easing.Linear)
    {
    }

    public IncrementalAnimationStep(TimeSpan duration, EasingDelegate easing)
    {
        Duration = duration;
        Easing = easing;
    }

    public string? Name { get; set; }

    public TimeSpan Duration { get; }

    public EasingDelegate Easing { get; }

    public bool IsInstantaneous => false;

    public int Count => stepItems.Count;

    public TimeSpan Progress { get; private set; }

    public bool IsComplete { get; private set; }

    public TimeSpan Update(TimeSpan delta)
    {
        // we do not support backwards at this time
        if (delta < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delta), "The animation cannot go backwards. Ensure the delta value is positive.");

        // we are at the end
        if (IsComplete)
            return delta;

        // skip if not making any progress 
        if (delta == TimeSpan.Zero)
            return TimeSpan.Zero;

        // jump to the end if max value was specified
        if (delta == TimeSpan.MaxValue)
            delta = Duration - Progress;

        var remaining = Duration - Progress;
        var actualDelta = delta > remaining
            ? remaining
            : delta;

        Progress += actualDelta;

        var seconds = Progress.TotalSeconds / Duration.TotalSeconds;
        var actualSeconds = Easing(seconds);

        foreach (var item in stepItems)
            item.SetProgress(actualSeconds);

        if (Progress >= Duration)
            IsComplete = true;

        return delta - actualDelta;
    }

    public void Complete() =>
        Update(TimeSpan.MaxValue);

    public void Add<TValue>(AnimatableProperty<TValue?> property) =>
        Add(property, property.Current, property.Desired);

    public void Add<TValue>(AnimatableProperty<TValue?> property, TValue? value) =>
        Add(property, property.Current, value);

    public void Add<TValue>(AnimatableProperty<TValue?> property, TValue? start, TValue? value)
    {
        var stepItem = new AnimationStepItem<TValue>(property, start, value);
        stepItems.Add(stepItem);
    }

    public IEnumerator<AnimationStepItem> GetEnumerator() =>
        stepItems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)stepItems).GetEnumerator();
}
