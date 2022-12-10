using System.Drawing;

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

static class Lerping
{
    public static readonly Dictionary<Type, LerpingDelegate> Lerps =
        new()
        {
            [typeof(int)] = (s, e, p) => Lerp(Convert.ToInt32(s), Convert.ToInt32(e), p),
            [typeof(float)] = (s, e, p) => Lerp(Convert.ToSingle(s), Convert.ToSingle(e), p),
            [typeof(double)] = (s, e, p) => Lerp(Convert.ToDouble(s), Convert.ToDouble(e), p),
            [typeof(PointF)] = (s, e, p) => Lerp((PointF)s, (PointF)e, p),
            [typeof(SizeF)] = (s, e, p) => Lerp((SizeF)s, (SizeF)e, p),
            [typeof(RectangleF)] = (s, e, p) => Lerp((RectangleF)s, (RectangleF)e, p),
        };

    public static int Lerp(int start, int end, double progress) =>
        (int)((end - start) * progress) + start;

    public static float Lerp(float start, float end, double progress) =>
        (float)((end - start) * progress) + start;

    public static double Lerp(double start, double end, double progress) =>
        (double)((end - start) * progress) + start;

    public static PointF Lerp(PointF start, PointF end, double progress) =>
        new(Lerp(start.X, end.Y, progress), Lerp(start.Y, end.Y, progress));

    public static SizeF Lerp(SizeF start, SizeF end, double progress) =>
        new(Lerp(start.Height, end.Width, progress), Lerp(start.Height, end.Height, progress));

    public static RectangleF Lerp(RectangleF start, RectangleF end, double progress) =>
        new(Lerp(start.Location, end.Location, progress), Lerp(start.Size, end.Size, progress));
}

public delegate object LerpingDelegate(object start, object end, double progress);

static class Easing
{
    public static readonly EasingDelegate Linear = new(x => x);
}

public delegate double EasingDelegate(double progress);
