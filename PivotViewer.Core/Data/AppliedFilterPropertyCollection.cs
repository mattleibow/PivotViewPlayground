namespace PivotViewer.Core.Data;

public class AppliedFilterPropertyCollection : FilterPropertyCollection
{
	public void ApplyValue(FilterValue filterValue) =>
		ApplyOrIncrementValue(filterValue, 1);

	public void UnapplyValue(FilterValue filterValue) =>
		ApplyOrIncrementValue(filterValue, -1);

	public void ApplyOrIncrementValue(FilterValue filterValue, int change = 1) =>
		ApplyOrIncrementValue(filterValue.Property.PivotProperty, filterValue.Value, change);

	private void ApplyOrIncrementValue(PivotProperty property, IComparable value, int change = 1)
	{
		if (TryGet(property.Name, out var applied))
		{
			applied.IncrementValue(value, change);
		}
		else
		{
			applied = new FilterProperty(property);
			applied.IncrementValue(value, change);
			Add(applied);
		}
	}
}
