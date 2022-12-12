namespace PivotView.Core.Tests.Animation;

public class AnimationSetUnitTests
{
    [Fact]
    public void SmallerStepDoesNotFireOnCompleteDelegate()
    {
        var step1Complete = 0;

        var set = new AnimationSet();

        var step1 = new AnimationStep(Time.OneSec);
        step1.OnComplete = () => step1Complete++;
        set.Add(step1);

        set.Update(Time.EightHundredMilli);
        Assert.Equal(0, step1Complete);

        set.Update(Time.EightHundredMilli);
        Assert.Equal(1, step1Complete);
    }

    [Fact]
    public void SteppingTracksCorrectCurrent()
    {
        var set = new AnimationSet();

        var step1 = new AnimationStep(Time.OneSec);
        set.Add(step1);

        var step2 = new AnimationStep(Time.OneSec);
        set.Add(step2);

        Assert.Null(set.Current);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(step1, set.Current);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(step1, set.Current);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(step2, set.Current);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(step2, set.Current);

        set.Update(Time.FourHundredMilli);
        Assert.Null(set.Current);
    }

    [Fact]
    public void SteppingLosesAnimationSteps()
    {
        var set = new AnimationSet();
        Assert.Equal(0, set.Count);

        set.Add(new AnimationStep(Time.OneSec));
        Assert.Equal(1, set.Count);

        set.Add(new AnimationStep(Time.OneSec));
        Assert.Equal(2, set.Count);
        
        set.Update(Time.FourHundredMilli);
        Assert.Equal(2, set.Count);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(2, set.Count);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(1, set.Count);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(1, set.Count);

        set.Update(Time.FourHundredMilli);
        Assert.Equal(0, set.Count);
    }

    [Fact]
    public void StepProgressesAnimationSet()
    {
        var step1Complete = 0;
        var step2Complete = 0;

        var set = new AnimationSet();

        var step1 = new AnimationStep(Time.OneSec);
        step1.OnComplete = () => step1Complete++;
        set.Add(step1);

        var step2 = new AnimationStep(Time.OneSec);
        step2.OnComplete = () => step2Complete++;
        set.Add(step2);

        Assert.Null(set.Current);

        set.Update(Time.EightHundredMilli);
        Assert.Equal(0, step1Complete);
        Assert.Equal(0, step2Complete);

        set.Update(Time.EightHundredMilli);
        Assert.Equal(1, step1Complete);
        Assert.Equal(0, step2Complete);

        set.Update(Time.EightHundredMilli);
        Assert.Equal(1, step1Complete);
        Assert.Equal(1, step2Complete);
    }
}
