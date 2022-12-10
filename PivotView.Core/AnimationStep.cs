using System.Drawing;

namespace PivotView.Core;

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

static public class Lerping
{
    public static readonly Dictionary<Type, LerpingDelegate> Lerps =
        new()
        {
            [typeof(int)] = (s, e, p) => Lerp(Convert.ToInt32(s), Convert.ToInt32(e), p),
            [typeof(float)] = (s, e, p) => Lerp(Convert.ToSingle(s), Convert.ToSingle(e), p),
            [typeof(double)] = (s, e, p) => Lerp(Convert.ToDouble(s), Convert.ToDouble(e), p),
            [typeof(PointF)] = (s, e, p) => Lerp(ToPointF(s), ToPointF(e), p),
            [typeof(SizeF)] = (s, e, p) => Lerp(ToSizeF(s), ToSizeF(e), p),
            [typeof(RectangleF)] = (s, e, p) => Lerp(ToRectangleF(s), ToRectangleF(e), p),
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

    private static PointF ToPointF(object? s) =>
        s is null ? PointF.Empty : (PointF)s;

    private static SizeF ToSizeF(object? s) =>
        s is null ? SizeF.Empty : (SizeF)s;

    private static RectangleF ToRectangleF(object? s) =>
        s is null ? RectangleF.Empty : (RectangleF)s;
}

public delegate object LerpingDelegate(object? start, object? end, double progress);

static public class Easing
{
    public static readonly EasingDelegate Linear = new(x => x);
}

public delegate double EasingDelegate(double progress);
