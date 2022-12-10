namespace PivotView.Core.Animation;

public class AnimatableProperty<T>
{
    public AnimatableProperty(T? current = default, T? desired = default)
    {
        Current = current;
        Desired = desired;
    }

    public T? Current { get; set; }

    public T? Desired { get; set; }
}

