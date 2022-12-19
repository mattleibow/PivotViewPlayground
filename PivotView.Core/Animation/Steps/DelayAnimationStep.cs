using System.Diagnostics;

namespace PivotView.Core.Animation;

[DebuggerDisplay("Delay: Name = {Name}, IsComplete = {IsComplete}")]
public class DelayAnimationStep : AnimationStep
{
	public DelayAnimationStep(TimeSpan duration)
		: base(duration)
	{
	}

	protected override void UpdateProgress()
	{
		// no-op as we just delay things
	}
}
