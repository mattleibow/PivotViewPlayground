using System.Reflection;

namespace PivotView.Core.VisualizerApp.Visualizers;

public class Visualizer : BindableObject, IDrawable
{
    private readonly List<VisualizerProperty> properties = new();

    public Visualizer(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public IReadOnlyList<VisualizerProperty> Properties => GetOrBuildProperties();

    public void InvalidateDrawing() =>
        OnPropertyChanged(nameof(IDrawable));

    public virtual void Draw(ICanvas canvas, RectF bounds)
    {
        // clear
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(bounds);
    }

    protected virtual void OnVisualizerPropertyValueChanged()
    {
        InvalidateDrawing();
    }

    private IReadOnlyList<VisualizerProperty> GetOrBuildProperties()
    {
        if (properties.Count == 0)
        {
            var props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in props)
            {
                if (property.GetCustomAttribute<VisualizerPropertyAttribute>() is not null)
                {
                    var item = new VisualizerProperty(this, property);
                    item.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(VisualizerProperty.Value))
                            OnVisualizerPropertyValueChanged();
                    };
                    properties.Add(item);
                }
            }
        }

        return properties;
    }
}
