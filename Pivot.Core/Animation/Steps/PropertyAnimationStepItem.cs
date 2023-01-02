namespace Pivot.Core.Animation;

public abstract record PropertyAnimationStepItem(TimeSpan Duration, TimeSpan Delay)
{
	public abstract void SetProgress(double progress);

	public double GetProgress(TimeSpan progress)
	{
		progress -= Delay;

		if (progress < TimeSpan.Zero)
			progress = TimeSpan.Zero;
		if (progress > Duration)
			progress = Duration;

		return progress / Duration;
	}
}

public record PropertyAnimationStepItem<TValue>(AnimatableProperty<TValue?> Property, TValue? Start, TValue? End, TimeSpan Duration, TimeSpan Delay)
	: PropertyAnimationStepItem(Duration, Delay)
{
	public PropertyAnimationStepItem(AnimatableProperty<TValue?> property, TValue? start, TValue? end)
		: this(property, start, end, TimeSpan.Zero, TimeSpan.Zero)
	{
	}

	public override void SetProgress(double progress)
	{
		var lerp = Lerping.Lerps[typeof(TValue)];
		var value = (TValue)lerp(Start, End, progress);
		Property.Current = value;
	}
}
