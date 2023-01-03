namespace Pivot.Core.Data;

public class FilterManager
{
	private readonly FilterPropertyCollection allFilters = new();
	private readonly AppliedFilterPropertyCollection appliedFilters = new();
	private readonly AppliedFilterPropertyCollection availableFilters = new();
	private readonly List<PivotDataItem> filteredItems = new();
	private readonly List<PivotDataItem> removedItems = new();

	public FilterManager()
	{
		appliedFilters.FilterChanged += OnAppliedFilterChanged;
	}

	public FilterManager(PivotDataSource dataSource)
		: this()
	{
		DataSource = dataSource;
	}

	public PivotDataSource? DataSource { get; set; }

	public FilterPropertyCollection AllFilters => allFilters;

	public AppliedFilterPropertyCollection AppliedFilters => appliedFilters;

	public AppliedFilterPropertyCollection AvailableFilters => availableFilters;

	public IReadOnlyList<PivotDataItem> FilteredItems => filteredItems;

	public IReadOnlyList<PivotDataItem> RemovedItems => removedItems;

	public event EventHandler? FilterUpdated;

	public void RebuildIndexes()
	{
		ClearIndexes();

		BuildIndexes();

		FilterUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void ClearIndexes()
	{
		allFilters.Clear();
		appliedFilters.Clear();
		availableFilters.Clear();
		filteredItems.Clear();
		removedItems.Clear();
	}

	private void BuildIndexes()
	{
		if (DataSource is null)
			return;

		// add the properties
		foreach (var property in DataSource.Properties)
		{
			if (!property.IsFilterVisible)
				continue;

			var filterProp = new FilterProperty(property);
			allFilters.Add(filterProp);
		}

		// calculate all the available values
		foreach (var item in DataSource.Items)
		{
			foreach (var propPair in item.Properties)
			{
				if (!propPair.Key.IsFilterVisible)
					continue;

				if (!allFilters.TryGet(propPair.Key.Name, out var filterProp))
					continue;

				foreach (var val in propPair.Value)
				{
					filterProp.IncrementValue(val, 1);
				}
			}
		}

		// copy all the values to remaining values
		foreach (var available in allFilters)
		{
			var remaining = new FilterProperty(available.PivotProperty);
			availableFilters.Add(remaining);

			foreach (var val in available.Values)
			{
				remaining.IncrementValue(val.Value, val.Count);
			}
		}

		removedItems.EnsureCapacity(DataSource.Items.Count);

		filteredItems.EnsureCapacity(DataSource.Items.Count);
		filteredItems.AddRange(DataSource.Items);
	}

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
			// loop through each value as OR
			if (!IsAvailable(item, filter))
				return false;
		}

		return true;
	}

	private static bool IsAvailable(PivotDataItem item, FilterProperty filter)
	{
		// the item MUST have the filtered property
		if (!item.Properties.TryGetValue(filter.PivotProperty, out var propertyValues))
			return false;

		// loop through each value as OR
		return IsIntersection(propertyValues, filter.Values);
	}

	private static bool IsAvailable(PivotDataItem item, string propertyName, IComparable value)
	{
		// the item MUST have the filtered property
		if (!item.Properties.TryGetValue(propertyName, out var propertyValues))
			return false;

		// loop through each value as OR
		return IsMatch(propertyValues, value);
	}

	private static bool IsAvailable(PivotDataItem item, string propertyName, IEnumerable<IComparable> values)
	{
		// the item MUST have the filtered property
		if (!item.Properties.TryGetValue(propertyName, out var propertyValues))
			return false;

		// loop through each value as OR
		return IsIntersection(propertyValues, values);
	}

	private static bool IsIntersection(IEnumerable<IComparable> itemPropertyValues, IEnumerable<FilterValue> filterValues)
	{
		foreach (var filterValue in filterValues)
		{
			if (IsMatch(itemPropertyValues, filterValue.Value))
				return true;
		}

		return false;
	}

	private static bool IsIntersection(IEnumerable<IComparable> itemPropertyValues, IEnumerable<IComparable> values)
	{
		foreach (var value in values)
		{
			if (IsMatch(itemPropertyValues, value))
				return true;
		}

		return false;
	}

	private static bool IsMatch(IEnumerable<IComparable> itemPropertyValues, IComparable value)
	{
		foreach (var propertyValue in itemPropertyValues)
		{
			if (value.CompareTo(propertyValue) == 0)
				return true;
		}

		return false;
	}

	private void OnAppliedFilterChanged(object? sender, FilterChangedEventArgs e)
	{
		if (e.Action == FilterChangedAction.AddProperty)
		{
			// this is adding the first value of a NEW property and
			// thus means the filter is getting MORE restrictive

			for (var i = filteredItems.Count - 1; i >= 0; i--)
			{
				var item = filteredItems[i];

				if (!IsAvailable(item, e.PropertyName, e.Values))
				{
					removedItems.Add(item);
					filteredItems.RemoveAt(i);
				}
			}
		}
		else if (e.Action == FilterChangedAction.RemoveProperty)
		{
			// this is removing the LAST value of an old property and
			// thus means the filter is getting LESS restrictive

			for (var i = removedItems.Count - 1; i >= 0; i--)
			{
				var item = removedItems[i];

				if (IsAvailable(item))
				{
					filteredItems.Add(item);
					removedItems.RemoveAt(i);
				}
			}
		}
		else if (e.Action == FilterChangedAction.AddValue)
		{
			// this is adding ANOTHER value of an old property and
			// thus means the filter is getting LESS restrictive

			for (var i = removedItems.Count - 1; i >= 0; i--)
			{
				var item = removedItems[i];

				if (IsAvailable(item))
				{
					filteredItems.Add(item);
					removedItems.RemoveAt(i);
				}
			}
		}
		else if (e.Action == FilterChangedAction.RemoveValue)
		{
			// this is removing ANOTHER value of an old property and
			// thus means the filter is getting MORE restrictive

			for (var i = filteredItems.Count - 1; i >= 0; i--)
			{
				var item = filteredItems[i];

				if (!IsAvailable(item, e.Property))
				{
					removedItems.Add(item);
					filteredItems.RemoveAt(i);
				}
			}
		}

		// TODO: improve filter performance

		availableFilters.Clear();

		foreach (var item in DataSource.Items)
		{
			foreach (var property in item.Properties)
			{
				if (!property.Key.IsFilterVisible)
					continue;

				var available = true;

				foreach (var filter in appliedFilters)
				{
					if (property.Key != filter.PivotProperty && !IsAvailable(item, filter))
					{
						available = false;
						break;
					}
				}

				if (available)
				{
					foreach (var v in property.Value)
					{
						availableFilters.ApplyValue(property.Key, v);
					}
				}
			}
		}

		FilterUpdated?.Invoke(this, e);
	}
}
