namespace PivotViewer.Core.Data;

public class FilterManager
{
	private readonly FilterPropertyCollection availableFilters = new();
	private readonly AppliedFilterPropertyCollection appliedFilters = new();

	public FilterManager()
	{
	}

	public FilterManager(PivotDataSource dataSource)
	{
		DataSource = dataSource;
	}

	public PivotDataSource? DataSource { get; set; }

	public FilterPropertyCollection AvailableFilters => availableFilters;
	
	public AppliedFilterPropertyCollection AppliedFilters => appliedFilters;

	public event EventHandler? FilterUpdated;

	public void RebuildIndexes()
	{
		availableFilters.Clear();

		if (DataSource is null)
			return;

		var filterByPivotProperties = new Dictionary<PivotProperty, FilterProperty>();

		foreach (var property in DataSource.Properties)
		{
			if (!property.IsFilterVisible)
				continue;

			var filterProp = new FilterProperty(property);
			availableFilters.Add(filterProp);

			filterByPivotProperties[property] = filterProp;
		}

		foreach (var item in DataSource.Items)
		{
			foreach (var prop in item.Properties)
			{
				if (!filterByPivotProperties.TryGetValue(prop.Key, out var filterProp))
					continue;

				foreach (var val in prop.Value)
				{
					filterProp.IncrementValue(val, 1);
				}
			}
		}
	}

	//public ICollection<IComparable> GetAllValues(PivotProperty property, IEnumerable<PivotDataItem> items, bool includeDuplicates = true)
	//{
	//	ICollection<IComparable> collection = includeDuplicates
	//		? new Collection<IComparable>()
	//		: new HashSet<IComparable>();

	//	foreach (var item in items)
	//	{
	//		if (item.Properties.TryGetValue(property, out var values))
	//		{
	//			foreach (IComparable value in values)
	//			{
	//				collection.Add(value);
	//			}
	//		}
	//	}

	//	return collection;
	//}

	//public void SetFilterValue(FilterValue filterValue)
	//{
	//	var property = filterValue.Category.Property;

	//	if (appliedFilters.TryGetValue(property, out var applied))
	//		applied.ResetValues();
	//	else
	//		appliedFilters[property] = applied = new FilterProperty(property);

	//	applied.SetValue(filterValue.Value, 1);
	//}

	public IEnumerable<PivotDataItem> GetFilteredItems()
	{
		if (DataSource is null)
			yield break;

		foreach (var item in DataSource.Items)
		{
			if (IsAvailable(item))
				yield return item;
		}
	}

	public bool IsAvailable(PivotDataItem item)
	{
		// loop through each filter as AND
		foreach (var filter in appliedFilters)
		{
			// the item MUST have the filtered property
			if (!item.Properties.TryGetValue(filter.PivotProperty, out var propValues))
				return false;

			// loop through each value as OR
			var isMatch = false;
			var filterValues = filter.Values;
			foreach (var fv in filterValues)
			{
				foreach (var pv in propValues)
				{
					if (fv.Value.CompareTo(pv) == 0)
					{
						isMatch = true;
						break;
					}
				}

				if (isMatch)
					break;
			}

			// none of the values intersected
			if (!isMatch)
				return false;
		}

		return true;
	}
}
