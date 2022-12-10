namespace PivotView.Core.Tests.Animation;

public class AnimationStepUnitTests
{
    private static TimeSpan ZeroTime = TimeSpan.Zero;

    private static TimeSpan FifthSec = TimeSpan.FromMilliseconds(1000.0 / 5); // 200
    private static TimeSpan QuarterSec = TimeSpan.FromMilliseconds(1000.0 / 4); // 250
    private static TimeSpan ThirdSec = TimeSpan.FromMilliseconds(1000.0 / 3); // 333.333
    private static TimeSpan HalfSec = TimeSpan.FromMilliseconds(1000.0 / 2); // 500
    private static TimeSpan OneSec = TimeSpan.FromSeconds(1);
    private static TimeSpan OneAndHalfSec = TimeSpan.FromSeconds(1.5);
    private static TimeSpan TwoSec = TimeSpan.FromSeconds(1);

    private static TimeSpan OneHundredMilli = TimeSpan.FromMilliseconds(100);
    private static TimeSpan FourHundredMilli = TimeSpan.FromMilliseconds(400);
    private static TimeSpan EightHundredMilli = TimeSpan.FromMilliseconds(800);

    [Theory]
    [InlineData(0.0f, 1.0f)]
    [InlineData(0.3f, 0.7f)]
    public void AddingAStepDoesNotUpdateProperty(float current, float desired)
    {
        var property = new AnimatableProperty<float>(current, desired);

        var step = new AnimationStep(OneSec, Easing.Linear);
        step.Add(property);

        Assert.Equal(current, property.Current);
        Assert.Equal(desired, property.Desired);
    }

    [Fact]
    public void SingleSmallerUpdateMovesForward()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(OneSec, Easing.Linear);
        step.Add(property);

        var remaining = step.Update(OneHundredMilli);

        Assert.Equal(ZeroTime, remaining);
        Assert.Equal(OneHundredMilli, step.Progress);

        Assert.Equal(0.1f, property.Current);
        Assert.Equal(1.0f, property.Desired);
    }

    [Fact]
    public void MultipleSmallerUpdatesMovesForward()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(OneSec, Easing.Linear);
        step.Add(property);

        var remaining1 = step.Update(FourHundredMilli);
        Assert.Equal(ZeroTime, remaining1);
        Assert.Equal(0.4f, property.Current);
        Assert.Equal(FourHundredMilli, step.Progress);

        var remaining2 = step.Update(FourHundredMilli);
        Assert.Equal(ZeroTime, remaining2);
        Assert.Equal(0.8f, property.Current);
        Assert.Equal(EightHundredMilli, step.Progress);
    }

    [Fact]
    public void ManySmallerUpdatesPreventsMovePast()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(OneSec, Easing.Linear);
        step.Add(property);

        step.Update(FourHundredMilli);
        step.Update(FourHundredMilli);

        var remaining = step.Update(FourHundredMilli);

        Assert.Equal(TimeSpan.FromMilliseconds(200), remaining);
        Assert.Equal(1.0f, property.Current);
        Assert.Equal(OneSec, step.Progress);
    }

    [Fact]
    public void SingleLargeUpdatesPreventsMovePast()
    {
        var property = new AnimatableProperty<float>(0.0f, 1.0f);

        var step = new AnimationStep(OneSec, Easing.Linear);
        step.Add(property);

        var remaining = step.Update(OneAndHalfSec);

        Assert.Equal(HalfSec, remaining);
        Assert.Equal(1.0f, property.Current);
        Assert.Equal(OneSec, step.Progress);
    }
}
