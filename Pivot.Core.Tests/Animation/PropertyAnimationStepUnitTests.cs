namespace Pivot.Core.Tests;

public class PropertyAnimationStepUnitTests
{
	[Theory]
	[InlineData(0f, 1f, 0.0, 0f)]
	[InlineData(0f, 1f, 0.3, 0.3f)]
	[InlineData(0f, 1f, 0.5, 0.5f)]
	[InlineData(0f, 1f, 0.7, 0.7f)]
	[InlineData(0f, 1f, 1.0, 1.0f)]
	public void CanStepFloat(float start, float end, double progress, float expected)
	{
		var property = new AnimatableProperty<float>(start, end);

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);
		step.Add(property);

		step.Update(TimeSpan.FromSeconds(progress));

		Assert.Equal(expected, property.Current);
	}

	[Theory]
	[InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 0.0, new[] { 0f, 0f, 100f, 100f })]
	[InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 0.5, new[] { 5f, 5f, 75f, 75f })]
	[InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 1.0, new[] { 10f, 10f, 50f, 50f })]
	public void CanStepRectangleF(float[] s, float[] e, double progress, float[] exp)
	{
		var start = new RectangleF(s[0], s[1], s[2], s[3]);
		var end = new RectangleF(e[0], e[1], e[2], e[3]);
		var expected = new RectangleF(exp[0], exp[1], exp[2], exp[3]);

		var property = new AnimatableProperty<RectangleF>(start, end);

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);
		step.Add(property);

		step.Update(TimeSpan.FromSeconds(progress));

		Assert.Equal(expected, property.Current);
	}

	[Theory]
	[InlineData(0.0f, 1.0f)]
	[InlineData(0.3f, 0.7f)]
	public void AddingAStepDoesNotUpdateProperty(float current, float desired)
	{
		var property = new AnimatableProperty<float>(current, desired);

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);
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

		var step = new PropertyAnimationStep(TimeSpan.FromMilliseconds(duration), Easing.Linear);
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

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);
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

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);
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

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);
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

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);
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
		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear);

		step.Update(Time.OneSec);

		Assert.True(step.IsComplete);
	}

	[Fact]
	public void StepWithDelayDelaysSmallStep()
	{
		var property = new AnimatableProperty<float>(0.0f, 1.0f);

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear, Time.Zero, Time.HalfSec);
		step.Add(property, 0, 1, Time.HalfSec);

		var remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.0f, property.Current);
		Assert.Equal(Time.FourHundredMilli, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.3f, property.Current);
		Assert.Equal(Time.FourHundredMilli * 2, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.7f, property.Current);
		Assert.Equal(Time.FourHundredMilli * 3, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.OneHundredMilli, remaining);
		Assert.Equal(1f, property.Current);
		Assert.Equal(Time.OneAndHalfSec, step.Progress);
		Assert.True(step.IsComplete);
	}

	[Fact]
	public void StepWithLargeDelayDelaysSmallStep()
	{
		var property = new AnimatableProperty<float>(0.0f, 1.0f);

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear, Time.Zero, Time.TwoSec);
		step.Add(property, 0, 1, Time.HalfSec);

		var remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.0f, property.Current);
		Assert.Equal(Time.FourHundredMilli, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.3f, property.Current);
		Assert.Equal(Time.FourHundredMilli * 2, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.7f, property.Current);
		Assert.Equal(Time.FourHundredMilli * 3, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.FourHundredMilli);

		Assert.Equal(Time.OneHundredMilli, remaining);
		Assert.Equal(1f, property.Current);
		Assert.Equal(Time.OneAndHalfSec, step.Progress);
		Assert.True(step.IsComplete);
	}

	[Fact]
	public void StepWithDelayDelaysAnimation()
	{
		var property = new AnimatableProperty<float>(0.0f, 1.0f);

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear, Time.Zero, Time.HalfSec);
		step.Add(property, 0, 1, Time.HalfSec);

		var remaining = step.Update(Time.HalfSec);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.0f, property.Current);
		Assert.Equal(Time.HalfSec, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.HalfSec);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(0.5f, property.Current);
		Assert.Equal(Time.OneSec, step.Progress);
		Assert.False(step.IsComplete);

		remaining = step.Update(Time.HalfSec);

		Assert.Equal(Time.Zero, remaining);
		Assert.Equal(1f, property.Current);
		Assert.Equal(Time.OneAndHalfSec, step.Progress);
		Assert.True(step.IsComplete);
	}

	[Fact]
	public void StepItemWithDelayUpdatesStep()
	{
		var property = new AnimatableProperty<float>();

		var step = new PropertyAnimationStep(Time.OneSec, Easing.Linear, Time.Zero, Time.HalfSec);

		Assert.Equal(Time.OneSec, step.Duration);

		step.Add(property, 0, 1, Time.Zero);

		Assert.Equal(Time.OneSec, step.Duration);

		step.Add(property, 0, 1, Time.QuarterSec);

		Assert.Equal(Time.OneSec + Time.QuarterSec, step.Duration);

		step.Add(property, 0, 1, Time.QuarterSec);

		Assert.Equal(Time.OneSec + Time.QuarterSec, step.Duration);

		step.Add(property, 0, 1, Time.ThirdSec);

		Assert.Equal(Time.OneSec + Time.ThirdSec, step.Duration);
	}
}
