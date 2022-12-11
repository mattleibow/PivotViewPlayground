namespace PivotView.Core.VisualizerApp.Visualizers;

public class VisualizerPropertyAttribute : Attribute
{
    public VisualizerPropertyAttribute(string label)
    {
        Label = label;
    }

    public string Label { get; }
}

public class SliderAttribute : VisualizerPropertyAttribute
{
    public SliderAttribute(string label, float min, float max)
        : base(label)
    {
        Min = min;
        Max = max;
    }

    public float Min { get; }

    public float Max { get; }
}

public class SwitchAttribute : VisualizerPropertyAttribute
{
    public SwitchAttribute(string label)
        : base(label)
    {
    }
}
