namespace PivotViewer.Core.VisualizerApp.Controls;

public class PivotFilterValue : BindableObject
{
	public static readonly BindableProperty IsAppliedProperty = BindableProperty.Create(
		nameof(IsApplied), typeof(bool), typeof(PivotFilterValue), false,
		propertyChanged: OnIsAppliedChanged);

	private readonly FilterManager filter;
	private readonly FilterProperty filterProperty;
	private readonly FilterValue filterValue;

	public PivotFilterValue(FilterManager filter, FilterProperty property, FilterValue value)
	{
		this.filter = filter;
		filterProperty = property;
		filterValue = value;
	}

	public object Value => filterValue.Value;

	public int Count => filterProperty.Count;

	public bool IsApplied
	{
		get => (bool)GetValue(IsAppliedProperty);
		set => SetValue(IsAppliedProperty, value);
	}

	private static void OnIsAppliedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not PivotFilterValue pfv || newValue is not bool applied)
			return;

		if (applied)
			pfv.filter.AppliedFilters.ApplyValue(pfv.filterValue);
		else
			pfv.filter.AppliedFilters.UnapplyValue(pfv.filterValue);
	}
}
