using System.Diagnostics;

namespace PivotView.Core.Animation;

[DebuggerDisplay("Action: Name = {Name}, IsComplete = {IsComplete}")]
public class ActionAnimationStep : IAnimationStep
{
    public ActionAnimationStep()
    {
    }

    public ActionAnimationStep(Action action)
        : this()
    {
        Action = action;
    }

    public string? Name { get; set; }

    public Action? Action { get; set; }

    public bool IsInstantaneous => true;

    public bool IsComplete { get; private set; }

    public void Complete()
    {
        if (IsComplete)
            return;

        Action?.Invoke();

        IsComplete = true;
    }

    public TimeSpan Update(TimeSpan delta)
    {
        Complete();
        return delta;
    }
}
