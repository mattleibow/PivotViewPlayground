namespace PivotView.Core.Tests.Animation;

public class AnimationStepUnitTests
{
    [Theory]
    [InlineData(0.0f, 1.0f)]
    [InlineData(0.3f, 0.7f)]
    public void AddingAStepDoesNotUpdateProperty(float current, float desired)
    {
        var property = new AnimatableProperty<float>(current, desired);

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        Assert.Equal(current, property.Current);
        Assert.Equal(desired, property.Desired);
    }

    [Fact]
    public void SingleSmallerUpdateMovesForward()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        var remaining = step.Update(Time.OneHundredMilli);

        Assert.Equal(Time.Zero, remaining);
        Assert.Equal(Time.OneHundredMilli, step.Progress);

        Assert.Equal(0.1f, property.Current);
        Assert.Equal(1.0f, property.Desired);
    }

    [Fact]
    public void MultipleSmallerUpdatesMovesForward()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        var remaining1 = step.Update(Time.FourHundredMilli);
        Assert.Equal(Time.Zero, remaining1);
        Assert.Equal(0.4f, property.Current);
        Assert.Equal(Time.FourHundredMilli, step.Progress);

        var remaining2 = step.Update(Time.FourHundredMilli);
        Assert.Equal(Time.Zero, remaining2);
        Assert.Equal(0.8f, property.Current);
        Assert.Equal(Time.EightHundredMilli, step.Progress);
    }

    [Fact]
    public void ManySmallerUpdatesPreventsMovePast()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        step.Update(Time.FourHundredMilli);
        step.Update(Time.FourHundredMilli);

        var remaining = step.Update(Time.FourHundredMilli);

        Assert.Equal(TimeSpan.FromMilliseconds(200), remaining);
        Assert.Equal(1.0f, property.Current);
        Assert.Equal(Time.OneSec, step.Progress);
    }

    [Fact]
    public void SingleLargeUpdatesPreventsMovePast()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        var remaining = step.Update(Time.OneAndHalfSec);

        Assert.Equal(Time.HalfSec, remaining);
        Assert.Equal(1.0f, property.Current);
        Assert.Equal(Time.OneSec, step.Progress);
    }

    [Fact]
    public void ExactMatchingStepFiresOnCompleteDelegate()
    {
        var isCompleted = 0;

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.OnComplete = () => isCompleted++;

        step.Update(Time.OneSec);

        Assert.Equal(1, isCompleted);
    }

    [Fact]
    public void SmallerStepDoesNotFireOnCompleteDelegate()
    {
        var isCompleted = 0;

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.OnComplete = () => isCompleted++;

        step.Update(Time.EightHundredMilli);

        Assert.Equal(0, isCompleted);
    }

    [Fact]
    public void StepFiresOnCompleteDelegate()
    {
        var isCompleted = 0;

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.OnComplete = () => isCompleted++;

        step.Update(Time.EightHundredMilli);

        Assert.Equal(0, isCompleted);

        step.Update(Time.EightHundredMilli);

        Assert.Equal(1, isCompleted);
    }

    [Fact]
    public void SingleLargeStepFiresOnCompleteDelegate()
    {
        var isCompleted = 0;

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.OnComplete = () => isCompleted++;

        step.Update(Time.OneAndHalfSec);

        Assert.Equal(1, isCompleted);
    }

    [Fact]
    public void StepFiresOnCompleteDelegateOnlyOnce()
    {
        var isCompleted = 0;

        var step = new AnimationStep(Time.OneSec, Easing.Linear);
        step.OnComplete = () => isCompleted++;

        step.Update(Time.EightHundredMilli);
        step.Update(Time.EightHundredMilli);
        step.Update(Time.EightHundredMilli);

        Assert.Equal(1, isCompleted);
    }
}
