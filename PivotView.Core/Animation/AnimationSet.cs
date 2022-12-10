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

