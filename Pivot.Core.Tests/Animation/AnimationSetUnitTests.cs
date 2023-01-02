namespace Pivot.Core.Tests;

public class AnimationSetUnitTests
{
	[Fact]
	public void SmallerStepDoesNotFireOnCompleteDelegate()
	{
		var set = new AnimationSet();

		var step1 = new PropertyAnimationStep(Time.OneSec);
		set.Add(step1);

		set.Update(Time.EightHundredMilli);
		Assert.False(step1.IsComplete);

		set.Update(Time.EightHundredMilli);
		Assert.True(step1.IsComplete);
	}

	[Fact]
	public void SteppingTracksCorrectCurrent()
	{
		var set = new AnimationSet();

		var step1 = new PropertyAnimationStep(Time.OneSec);
		set.Add(step1);

		var step2 = new PropertyAnimationStep(Time.OneSec);
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

		set.Add(new PropertyAnimationStep(Time.OneSec));
		Assert.Equal(1, set.Count);

		set.Add(new PropertyAnimationStep(Time.OneSec));
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
		var set = new AnimationSet();

		var step1 = new PropertyAnimationStep(Time.OneSec);
		set.Add(step1);

		var step2 = new PropertyAnimationStep(Time.OneSec);
		set.Add(step2);

		Assert.Null(set.Current);

		set.Update(Time.EightHundredMilli);
		Assert.False(step1.IsComplete);
		Assert.False(step2.IsComplete);
		Assert.False(set.IsComplete);

		set.Update(Time.EightHundredMilli);
		Assert.True(step1.IsComplete);
		Assert.False(step2.IsComplete);
		Assert.False(set.IsComplete);

		set.Update(Time.EightHundredMilli);
		Assert.True(step1.IsComplete);
		Assert.True(step2.IsComplete);
		Assert.True(set.IsComplete);
	}

	[Fact]
	public void InstantaneousStepsAreCollapsedWithExactTime()
	{
		var set = new AnimationSet();

		var step1 = new PropertyAnimationStep(Time.OneSec);
		set.Add(step1);

		var step2 = new ActionAnimationStep();
		set.Add(step2);

		Assert.Null(set.Current);

		set.Update(Time.OneSec);
		Assert.True(step1.IsComplete);
		Assert.True(step2.IsComplete);
		Assert.True(set.IsComplete);
	}

	[Fact]
	public void AllInstantaneousStepsAreCollapsedWithExactTime()
	{
		var set = new AnimationSet();

		set.Add(new PropertyAnimationStep(Time.OneSec));

		set.Add(new ActionAnimationStep());
		set.Add(new ActionAnimationStep());
		set.Add(new ActionAnimationStep());

		Assert.Null(set.Current);

		set.Update(Time.OneSec);
		Assert.True(set.IsComplete);
	}

	[Fact]
	public void InstantaneousStepsAreCollapsedWithSteppedTime()
	{
		var set = new AnimationSet();

		var step1 = new PropertyAnimationStep(Time.OneSec);
		set.Add(step1);

		var step2 = new ActionAnimationStep();
		set.Add(step2);

		Assert.Null(set.Current);

		set.Update(Time.EightHundredMilli);
		Assert.False(step1.IsComplete);
		Assert.False(step2.IsComplete);
		Assert.False(set.IsComplete);

		set.Update(Time.EightHundredMilli);
		Assert.True(step1.IsComplete);
		Assert.True(step2.IsComplete);
		Assert.True(set.IsComplete);
	}
}
