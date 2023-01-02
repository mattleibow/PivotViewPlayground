namespace Pivot.Core.Tests;

public class PropertyAnimationStepItemUnitTests
{
	[Theory]
	[InlineData(0f, 1f, 0.0, 0f)]
	[InlineData(0f, 1f, 0.3, 0.3f)]
	[InlineData(0f, 1f, 0.5, 0.5f)]
	[InlineData(0f, 1f, 0.7, 0.7f)]
	[InlineData(0f, 1f, 1.0, 1.0f)]
	public void CanStepFloat(float start, float end, double progress, float expected)
	{
		var property = new AnimatableProperty<float>(start);

		var item = new PropertyAnimationStepItem<float>(property, start, end);

		item.SetProgress(progress);

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

		var property = new AnimatableProperty<RectangleF>(start);

		var item = new PropertyAnimationStepItem<RectangleF>(property, start, end);

		item.SetProgress(progress);

		Assert.Equal(expected, property.Current);
	}

	[Theory]
	// normal: 1 sec, 0 delay
	[InlineData(0.0, 1.0, 0.0, 0.0)]
	[InlineData(0.3, 1.0, 0.0, 0.3)]
	[InlineData(0.5, 1.0, 0.0, 0.5)]
	[InlineData(0.7, 1.0, 0.0, 0.7)]
	[InlineData(1.0, 1.0, 0.0, 1.0)]
	[InlineData(1.2, 1.0, 0.0, 1.0)]
	// short delay: 1 sec, 0.4 delay
	[InlineData(0.0, 1.0, 0.4, 0.0)]
	[InlineData(0.3, 1.0, 0.4, 0.0)]
	[InlineData(0.5, 1.0, 0.4, 0.1)]
	[InlineData(0.7, 1.0, 0.4, 0.3)]
	[InlineData(1.0, 1.0, 0.4, 0.6)]
	[InlineData(1.2, 1.0, 0.4, 0.8)]
	[InlineData(1.4, 1.0, 0.4, 1.0)]
	[InlineData(1.6, 1.0, 0.4, 1.0)]
	// large delay: 1 sec, 1.4 delay
	[InlineData(0.0, 1.0, 1.4, 0.0)]
	[InlineData(0.5, 1.0, 1.4, 0.0)]
	[InlineData(1.0, 1.0, 1.4, 0.0)]
	[InlineData(1.2, 1.0, 1.4, 0.0)]
	[InlineData(1.4, 1.0, 1.4, 0.0)]
	[InlineData(1.6, 1.0, 1.4, 0.2)]
	[InlineData(2.0, 1.0, 1.4, 0.6)]
	[InlineData(2.5, 1.0, 1.4, 1.0)]
	public void This(double progress, double duration, double delay, double expectedProgress)
	{
		var property = new AnimatableProperty<float>();

		var item = new PropertyAnimationStepItem<float>(property, 0, 1, TimeSpan.FromSeconds(duration), TimeSpan.FromSeconds(delay));

		var actualProgress = item.GetProgress(TimeSpan.FromSeconds(progress));

		// cast to float for rounding
		Assert.Equal((float)expectedProgress, (float)actualProgress);
	}
}
