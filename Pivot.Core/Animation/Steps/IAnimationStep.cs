namespace Pivot.Core.Animation;

public interface IAnimationStep
{
	string? Name { get; }

	bool IsInstantaneous { get; }

	bool IsComplete { get; }

	TimeSpan Update(TimeSpan delta);

	void Complete();
}
