namespace PivotViewer.Core.Tests;

public class ActionAnimationStepUnitTests
{
	[Fact]
	public void UpdatesOnlyOnce()
	{
		var run = 0;

		var step = new ActionAnimationStep();
		step.Action = () => run++;

		Assert.Equal(0, run);

		step.Update(Time.OneSec);

		Assert.Equal(1, run);
		Assert.True(step.IsComplete);

		step.Update(Time.OneSec);

		Assert.Equal(1, run);
		Assert.True(step.IsComplete);
	}
}
