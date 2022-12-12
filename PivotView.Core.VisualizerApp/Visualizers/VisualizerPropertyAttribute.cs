namespace PivotView.Core.VisualizerApp.Visualizers;

public class VisualizerPropertyAttribute : Attribute
{
    private const string AttributeSuffix = "Attribute";

    public VisualizerPropertyAttribute(string label)
    {
        Label = label;
    }

    public string Label { get; }

    public string Key
    {
        get
        {
            var name = GetType().Name;
            if (name.EndsWith(AttributeSuffix, StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - AttributeSuffix.Length);
            return name;
        }
    }
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
