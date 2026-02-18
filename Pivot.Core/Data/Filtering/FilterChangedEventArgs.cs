namespace Pivot.Data.Filtering;

public class FilterChangedEventArgs : EventArgs
{
	public FilterChangedEventArgs(FilterChangedAction action, FilterProperty property, IComparable value)
	{
		Action = action;
		Property = property;
		Values = new[] { value };
	}

	public FilterChangedEventArgs(FilterChangedAction action, FilterProperty property, IEnumerable<IComparable> values)
	{
		Action = action;
		Property = property;
		Values = values.ToArray();
	}

	public FilterChangedAction Action { get; }

	public FilterProperty Property { get; }

	public string PropertyName => Property.Name;

	public IList<IComparable> Values { get; }
}
