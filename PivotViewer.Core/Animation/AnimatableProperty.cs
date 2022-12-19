namespace PivotViewer.Core.Animation;

public class AnimatableProperty<T>
{
	private T? desired;
	private T? current;

	public AnimatableProperty(T? current = default, T? desired = default)
	{
		Current = current;
		Desired = desired;
	}

	public T? Current
	{
		get => current;
		set
		{
			current = value;
			RecalculateIsCurrentDesired();
		}
	}

	public T? Desired
	{
		get => desired;
		set
		{
			desired = value;
			RecalculateIsCurrentDesired();
		}
	}

	public bool IsCurrentDesired { get; private set; }

	private void RecalculateIsCurrentDesired() =>
		IsCurrentDesired = (Current is null && Desired is null) || Desired?.Equals(Current) == true;
}

