using System.Reflection;

namespace PivotViewer.Core.VisualizerApp.Visualizers;

public class VisualizerProperty : BindableObject
{
	private string? error;

	public VisualizerProperty(Visualizer target, PropertyInfo property)
	{
		Target = target;
		Property = property;
		Attribute = property.GetCustomAttribute<VisualizerPropertyAttribute>() ?? throw new ArgumentException("Propery did not have a [VisualizerProperty] attribute.");
		DefaultValue = Value;
	}

	public Visualizer Target { get; }

	public PropertyInfo Property { get; }

	public VisualizerPropertyAttribute Attribute { get; }

	public string Name => Property.Name;

	public bool HasError => Error is not null;

	public string? Error
	{
		get => error;
		set
		{
			error = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(HasError));
		}
	}

	public object? Value
	{
		get => Property.GetValue(Target);
		set
		{
			try
			{
				var newVal = Convert.ChangeType(value, Property.PropertyType);
				if (Value != newVal)
				{
					Property.SetValue(Target, newVal);
					OnPropertyChanged();
				}

				Error = null;
			}
			catch (Exception ex)
			{
				Error = ex.Message;
			}
		}
	}

	public object? DefaultValue { get; }
}
