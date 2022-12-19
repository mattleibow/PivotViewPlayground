using System.Collections;
using System.Diagnostics;

namespace PivotView.Core.Animation;

[DebuggerDisplay("Property: Name = {Name}, IsComplete = {IsComplete}")]
public class PropertyAnimationStep : AnimationStep, IEnumerable<PropertyAnimationStepItem>
{
	private readonly LinkedList<PropertyAnimationStepItem> stepItems = new();
	private readonly TimeSpan actualDuration;

	public PropertyAnimationStep(TimeSpan duration)
		: this(duration, Animation.Easing.Linear, TimeSpan.Zero, TimeSpan.Zero)
	{
	}

	public PropertyAnimationStep(TimeSpan duration, EasingDelegate easing)
		: this(duration, easing, TimeSpan.Zero, TimeSpan.Zero)
	{
	}

	public PropertyAnimationStep(TimeSpan duration, EasingDelegate easing, TimeSpan minDelay, TimeSpan maxDelay)
		: base(duration)
	{
		actualDuration = duration;

		Easing = easing;
		MinimumDelay = minDelay;
		MaximumDelay = maxDelay;
	}

	public EasingDelegate Easing { get; }

	public TimeSpan MinimumDelay { get; }

	public TimeSpan MaximumDelay { get; }

	public int Count => stepItems.Count;

	protected override void UpdateProgress()
	{
		foreach (var item in stepItems)
		{
			var percent = item.GetProgress(Progress);
			var actualPercent = Easing(percent);
			item.SetProgress(actualPercent);
		}
	}

	public void Add<TValue>(AnimatableProperty<TValue?> property) =>
		Add(property, property.Current, property.Desired, GetRandomDelay());

	public void Add<TValue>(AnimatableProperty<TValue?> property, TValue? end) =>
		Add(property, property.Current, end, GetRandomDelay());

	public void Add<TValue>(AnimatableProperty<TValue?> property, TValue? start, TValue? end) =>
		Add(property, start, end, GetRandomDelay());

	public void Add<TValue>(AnimatableProperty<TValue?> property, TValue? start, TValue? end, TimeSpan delay)
	{
		if (delay < MinimumDelay || delay > MaximumDelay)
			throw new ArgumentOutOfRangeException(nameof(delay), delay, $"Delay must be between MinimumDelay {MinimumDelay} and MaximumDelay {MaximumDelay}, inclusive.");

		// increase the total duration
		var totalDuration = actualDuration + delay;
		if (totalDuration > Duration)
			Duration = totalDuration;

		var stepItem = new PropertyAnimationStepItem<TValue>(property, start, end, actualDuration, delay);
		stepItems.AddLast(stepItem);
	}

	private TimeSpan GetRandomDelay()
	{
		var min = MinimumDelay.Ticks;
		var max = MaximumDelay.Ticks;

		var rnd = Random.Shared.NextInt64(min, max);

		return TimeSpan.FromTicks(rnd);
	}

	public IEnumerator<PropertyAnimationStepItem> GetEnumerator() =>
		stepItems.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		((IEnumerable)stepItems).GetEnumerator();
}
