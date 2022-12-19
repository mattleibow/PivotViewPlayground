namespace PivotViewer.Core.Tests;

public class AnimatablePropertyUnitTests
{
	[Theory]
	[InlineData(null, null, true)]
	[InlineData(null, 10, false)]
	[InlineData(10, null, false)]
	[InlineData(10, 10, true)]
	[InlineData(null, 10f, false)]
	[InlineData(10f, null, false)]
	[InlineData(10f, 10f, true)]
	public void IsDesiredIsCorrect(object? current, object? desired, bool expected)
	{
		var property = new AnimatableProperty<object?>(current, desired);

		var isDesired = property.IsCurrentDesired;

		Assert.Equal(expected, isDesired);
	}
}
