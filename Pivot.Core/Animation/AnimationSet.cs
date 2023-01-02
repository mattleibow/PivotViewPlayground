using System.Collections;

namespace Pivot.Core.Animation;

public interface IAnimationSet
{
	int Count { get; }

	IEnumerable<IAnimationStep> Steps { get; }

	IAnimationStep? Current { get; }

	bool IsComplete { get; }

	void Update(TimeSpan delta);

	void Complete();
}

public class AnimationSet : IAnimationSet, IEnumerable<IAnimationStep>
{
	private readonly LinkedList<IAnimationStep> steps = new();

	private IAnimationStep? currentStep;

	public AnimationSet()
	{
	}

	public IAnimationStep? Current => currentStep;

	public int Count => steps.Count + (currentStep is null ? 0 : 1);

	public bool IsComplete => steps.Count == 0 && currentStep is null;

	public void Add(IAnimationStep step)
	{
		steps.AddLast(step);
	}

	public void Update(TimeSpan delta)
	{
		// we do not support backwards at this time
		if (delta < TimeSpan.Zero)
			throw new ArgumentOutOfRangeException(nameof(delta), "The animation cannot go backwards. Ensure the delta value is positive.");

		// skip if not making any progress 
		if (delta == TimeSpan.Zero)
			return;

		// jump to the end if max value was specified
		if (delta == TimeSpan.MaxValue)
		{
			Complete();
			return;
		}

		var remainingDelta = delta;
		while (remainingDelta >= TimeSpan.Zero)
		{
			// get the current or next step depending
			if (currentStep is null && steps.First is not null)
			{
				currentStep = steps.First.Value;
				steps.RemoveFirst();
			}

			// if there is nothing to do, bail out
			if (currentStep is null)
				return;

			// run the next step if it is instantaneous
			if (!currentStep.IsInstantaneous && remainingDelta == TimeSpan.Zero)
				return;

			// step the animation
			remainingDelta = currentStep.Update(remainingDelta);

			// prepare to load the next step
			if (currentStep.IsComplete)
				currentStep = null;
		}
	}

	public void Complete()
	{
		foreach (var step in steps)
			step.Complete();

		steps.Clear();

		currentStep = null;
	}

	public IEnumerator<IAnimationStep> GetEnumerator() =>
		steps.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		((IEnumerable)steps).GetEnumerator();

	IEnumerable<IAnimationStep> IAnimationSet.Steps => steps;
}

