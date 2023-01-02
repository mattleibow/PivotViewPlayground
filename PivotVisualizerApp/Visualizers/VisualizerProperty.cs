using System.ComponentModel;
using System.Reflection;

namespace PivotVisualizerApp.Visualizers;

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

	public string? Value
	{
		get
		{
			var val = Property.GetValue(Target);
			var newVal = ConvertToString(val);
			return newVal;
		}
		set
		{
			try
			{
				var newVal = ConvertFromString(value);

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

	public string? DefaultValue { get; }

	private object? ConvertFromString(string? value)
	{
		if (Attribute.Converter is null)
			return Convert.ChangeType(value, Property.PropertyType);

		var converter = (TypeConverter)Activator.CreateInstance(Attribute.Converter)!;
		return converter.ConvertFrom(value);
	}

	private string? ConvertToString(object? value)
	{
		if (Attribute.Converter is null)
			return value?.ToString();

		var converter = (TypeConverter)Activator.CreateInstance(Attribute.Converter)!;
		return converter.ConvertTo(value, typeof(string))?.ToString();
	}
}
