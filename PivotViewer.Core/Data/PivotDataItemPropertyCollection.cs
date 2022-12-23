using System.Diagnostics.CodeAnalysis;

namespace PivotViewer.Core.Data;

public class PivotDataItemPropertyCollection : Dictionary<PivotProperty, PivotPropertyValueCollection>
{
	public PivotPropertyValueCollection this[string propertyName]
	{
		get => this[GetProperty(propertyName)];
		set => this[GetProperty(propertyName)] = value;
	}

	public bool TryGetValue(string propertyName, [MaybeNullWhen(false)] out PivotPropertyValueCollection values)
	{
		values = null;

		if (TryGetProperty(propertyName, out var property))
			return TryGetValue(property, out values);

		return false;
	}

	private PivotProperty GetProperty(string propertyName)
	{
		if (TryGetProperty(propertyName, out var property))
			return property;

		throw new KeyNotFoundException($"Property with name '{propertyName}' does not exist.");
	}

	private bool TryGetProperty(string propertyName, [MaybeNullWhen(false)] out PivotProperty property)
	{
		property = null;

		foreach (var key in Keys)
		{
			if (key.Name == propertyName)
			{
				property = key;
				return true;
			}
		}

		return false;
	}
}
