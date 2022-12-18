namespace PivotView.Core.Tests;

public class IncrementalAnimationStepUnitTests
{
    [Theory]
    [InlineData(0.0f, 1.0f)]
    [InlineData(0.3f, 0.7f)]
    public void AddingAStepDoesNotUpdateProperty(float current, float desired)
    {
        var property = new AnimatableProperty<float>(current, desired);

        var step = new IncrementalAnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        Assert.Equal(current, property.Current);
        Assert.Equal(desired, property.Desired);
    }

    [Theory]
    [InlineData(1000, 1000, 1f)]
    [InlineData(1000, 500, 0.5f)]
    [InlineData(500, 500, 01f)]
    [InlineData(500, 250, 0.5f)]
    public void SingleSmallerUpdateMovesForward2(int duration, int stepSize, float expected)
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new IncrementalAnimationStep(TimeSpan.FromMilliseconds(duration), Easing.Linear);
        step.Add(property);

        var delta = TimeSpan.FromMilliseconds(stepSize);
        var remaining = step.Update(delta);

        Assert.Equal(Time.Zero, remaining);
        Assert.Equal(delta, step.Progress);

        Assert.Equal(expected, property.Current);
        Assert.Equal(1.0f, property.Desired);
    }

    [Fact]
    public void SingleSmallerUpdateMovesForward()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new IncrementalAnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        var remaining = step.Update(Time.OneHundredMilli);

        Assert.Equal(Time.Zero, remaining);
        Assert.Equal(Time.OneHundredMilli, step.Progress);
        Assert.False(step.IsComplete);

        Assert.Equal(0.1f, property.Current);
        Assert.Equal(1.0f, property.Desired);
    }
    
    [Fact]
    public void MultipleSmallerUpdatesMovesForward()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new IncrementalAnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        var remaining1 = step.Update(Time.FourHundredMilli);
        Assert.Equal(Time.Zero, remaining1);
        Assert.Equal(0.4f, property.Current);
        Assert.Equal(Time.FourHundredMilli, step.Progress);
        Assert.False(step.IsComplete);

        var remaining2 = step.Update(Time.FourHundredMilli);
        Assert.Equal(Time.Zero, remaining2);
        Assert.Equal(0.8f, property.Current);
        Assert.Equal(Time.EightHundredMilli, step.Progress);
        Assert.False(step.IsComplete);
    }

    [Fact]
    public void ManySmallerUpdatesPreventsMovePast()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new IncrementalAnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        step.Update(Time.FourHundredMilli);
        step.Update(Time.FourHundredMilli);

        var remaining = step.Update(Time.FourHundredMilli);

        Assert.Equal(TimeSpan.FromMilliseconds(200), remaining);
        Assert.Equal(1.0f, property.Current);
        Assert.Equal(Time.OneSec, step.Progress);
        Assert.True(step.IsComplete);
    }

    [Fact]
    public void SingleLargeUpdatesPreventsMovePast()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new IncrementalAnimationStep(Time.OneSec, Easing.Linear);
        step.Add(property);

        var remaining = step.Update(Time.OneAndHalfSec);

        Assert.Equal(Time.HalfSec, remaining);
        Assert.Equal(1.0f, property.Current);
        Assert.Equal(Time.OneSec, step.Progress);
        Assert.True(step.IsComplete);
    }

    [Fact]
    public void ExactMatchingStepFiresOnCompleteDelegate()
    {
        var step = new IncrementalAnimationStep(Time.OneSec, Easing.Linear);

        step.Update(Time.OneSec);

        Assert.True(step.IsComplete);
    }
}
