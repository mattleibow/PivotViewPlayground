using Pivot.Animation.Steps;

namespace Pivot.Animation;

public interface IAnimationSet
{
	int Count { get; }

	IEnumerable<IAnimationStep> Steps { get; }

	IAnimationStep? Current { get; }

	bool IsComplete { get; }

	void Update(TimeSpan delta);

	void Complete();
}

