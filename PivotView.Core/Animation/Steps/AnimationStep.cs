using System.Diagnostics;

namespace PivotView.Core.Animation;

[DebuggerDisplay("Step: Name = {Name}, IsComplete = {IsComplete}")]
public abstract class AnimationStep : IAnimationStep
{
	public AnimationStep(TimeSpan duration)
	{
		Duration = duration;
	}

	public string? Name { get; set; }

	public TimeSpan Duration { get; protected set; }

	public bool IsInstantaneous => false;

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

		UpdateProgress();

		if (Progress >= Duration)
			IsComplete = true;

		return delta - actualDelta;
	}

	public void Complete() =>
		Update(TimeSpan.MaxValue);

	protected abstract void UpdateProgress();
}
