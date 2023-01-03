namespace PivotVisualizerApp.Controls.ViewModels;

public class FilterValueViewModel : BindableObject
{
	private readonly FilterManager filter;
	private readonly FilterProperty filterProperty;
	private readonly FilterValue filterValue;

	private bool inSync;

	private bool isApplied;
	private int count;

	internal FilterValueViewModel(FilterManager filter, FilterProperty property, FilterValue value)
	{
		this.filter = filter;
		filterProperty = property;
		filterValue = value;

		Value = value.Value;

		Sync(value);
	}

	public object Value { get; }

	public int Count
	{
		get => count;
		private set
		{
			count = value;
			OnPropertyChanged();
		}
	}

	public bool IsApplied
	{
		get => isApplied;
		set
		{
			isApplied = value;

			if (!inSync)
			{
				if (isApplied)
					filter.AppliedFilters.ApplyValue(filterValue);
				else
					filter.AppliedFilters.UnapplyValue(filterValue);
			}

			OnPropertyChanged();
		}
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
}
