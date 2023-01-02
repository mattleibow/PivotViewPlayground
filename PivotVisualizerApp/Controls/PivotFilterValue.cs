namespace PivotVisualizerApp.Controls;

public class PivotFilterValue : BindableObject
{
	public static readonly BindableProperty IsAppliedProperty = BindableProperty.Create(
		nameof(IsApplied), typeof(bool), typeof(PivotFilterValue), false,
		propertyChanged: OnIsAppliedChanged);

	public static readonly BindableProperty CountProperty = BindableProperty.Create(
		nameof(Count), typeof(int), typeof(PivotFilterValue), 0);

	public static readonly BindableProperty ValueProperty = BindableProperty.Create(
		nameof(Value), typeof(object), typeof(PivotFilterValue), null);

	private readonly FilterManager filter;
	private readonly FilterProperty filterProperty;
	private readonly FilterValue filterValue;

	private bool inSync;

	internal PivotFilterValue(FilterManager filter, FilterProperty property, FilterValue value)
	{
		this.filter = filter;
		filterProperty = property;
		filterValue = value;

		Value = value.Value;
		Sync(value);
	}

	public object Value
	{
		get => GetValue(ValueProperty);
		private set => SetValue(ValueProperty, value);
	}

	public int Count
	{
		get => (int)GetValue(CountProperty);
		private set => SetValue(CountProperty, value);
	}

	public bool IsApplied
	{
		get => (bool)GetValue(IsAppliedProperty);
		set => SetValue(IsAppliedProperty, value);
	}

	internal void Sync(FilterValue source)
	{
		inSync = true;

		IsApplied = GetIsApplied(source);
		Count = source.Count;

		inSync = false;
	}

	private bool GetIsApplied(FilterValue value) =>
		filter.AppliedFilters.TryGet(value.Property.Name, out var prop) && prop.HasValue(value.Value);

	private static void OnIsAppliedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not PivotFilterValue pfv || newValue is not bool applied)
			return;

		if (pfv.inSync)
			return;

		if (applied)
			pfv.filter.AppliedFilters.ApplyValue(pfv.filterValue);
		else
			pfv.filter.AppliedFilters.UnapplyValue(pfv.filterValue);
	}
}
