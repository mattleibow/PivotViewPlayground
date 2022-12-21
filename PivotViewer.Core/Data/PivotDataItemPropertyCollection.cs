namespace PivotViewer.Core.Data;

public class PivotDataItemPropertyCollection : Dictionary<PivotProperty, PivotPropertyValueCollection>
{
	public PivotPropertyValueCollection this[string propertyName]
	{
		get => this[GetProperty(propertyName)];
		set => this[GetProperty(propertyName)] = value;
	}

	private PivotProperty GetProperty(string propertyName)
	{
		foreach (var key in Keys)
		{
			if (key.Name == propertyName)
				return key;
		}

		throw new KeyNotFoundException($"Property with name '{propertyName}' does not exist.");
	}
}
