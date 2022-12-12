namespace PivotView.Core.Animation;

public class AnimationSet
{
    private readonly LinkedList<AnimationStep> steps = new();

    private AnimationStep? currentStep;
    private TimeSpan currentDuration = TimeSpan.Zero;

    public AnimationSet()
    {
    }

    public AnimationStep? Current => currentStep;

    public int Count => steps.Count + (currentStep is null ? 0 : 1);

    public void Add(AnimationStep step)
    {
        steps.AddLast(step);
    }

    public void Update(TimeSpan delta)
    {
        if (delta < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delta), "The animation cannot go backwards. Ensure the delta value is positive.");

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
            if (currentStep is null || remainingDelta == TimeSpan.Zero)
                return;

            // step the animation
            remainingDelta = currentStep.Update(remainingDelta);

            // prepare to load the next step
            if (currentStep.IsComplete)
                currentStep = null;
        }

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

