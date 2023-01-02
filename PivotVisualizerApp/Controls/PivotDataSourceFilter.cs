using System.Text;

namespace PivotVisualizerApp.Controls;

public class PivotDataSourceFilter : BindableObject
{
	// TODO: make readonly
	public static readonly BindableProperty PropertiesProperty = BindableProperty.Create(
		nameof(Properties), typeof(ObservableCollection<PivotFilterProperty>), typeof(PivotDataSourceFilter),
		defaultValueCreator: (b) => new ObservableCollection<PivotFilterProperty>());

	// TODO: make readonly
	public static readonly BindableProperty FilterProperty = BindableProperty.Create(
		nameof(Filter), typeof(ObservableCollection<PivotFilterProperty>), typeof(PivotDataSourceFilter),
		defaultValueCreator: (b) => new ObservableCollection<PivotFilterProperty>());

	// TODO: make readonly
	public static readonly BindableProperty FilteredItemsProperty = BindableProperty.Create(
		nameof(FilteredItems), typeof(ObservableCollection<PivotDataItem>), typeof(PivotDataSourceFilter),
		defaultValueCreator: (b) => new ObservableCollection<PivotDataItem>());

	private PivotDataSource datasource;
	private FilterManager filter;

	public PivotDataSourceFilter(PivotDataSource datasource)
	{
		this.datasource = datasource;

		filter = new FilterManager(datasource);
		filter.RebuildIndexes();

		OnFilterUpdated(filter, EventArgs.Empty);

		filter.FilterUpdated += OnFilterUpdated;
	}

	public ObservableCollection<PivotFilterProperty> Properties
	{
		get => (ObservableCollection<PivotFilterProperty>)GetValue(PropertiesProperty);
		private set => SetValue(PropertiesProperty, value);
	}

	public ObservableCollection<PivotFilterProperty> Filter
	{
		get => (ObservableCollection<PivotFilterProperty>)GetValue(FilterProperty);
		private set => SetValue(FilterProperty, value);
	}

	public ObservableCollection<PivotDataItem> FilteredItems
	{
		get => (ObservableCollection<PivotDataItem>)GetValue(FilteredItemsProperty);
		private set => SetValue(FilteredItemsProperty, value);
	}

	public event EventHandler? FilterUpdated;

	private PivotFilterProperty CreateProperty(FilterProperty property) =>
		new(filter, property);

	private void OnFilterUpdated(object? sender, EventArgs e)
	{
		Sync(filter.AvailableFilters, Properties);
		Sync(filter.AppliedFilters, Filter);

		Sync(filter.FilteredItems, FilteredItems);

		FilterUpdated?.Invoke(this, EventArgs.Empty);
	}

	private static string GetFilterString(AppliedFilterPropertyCollection appliedFilters)
	{
		var sb = new StringBuilder();

		foreach (var filter in appliedFilters)
		{
			if (sb.Length > 0)
				sb.Append(';');
			sb.Append(filter.Name);
			sb.Append('=');
			var first = true;
			foreach (var value in filter.Values)
			{
				if (!first)
					sb.Append(',');
				sb.Append(value.Value);
				first = false;
			}
		}

		return sb.ToString();
	}

	private void Sync(FilterPropertyCollection source, ObservableCollection<PivotFilterProperty> destination) =>
		CollectionHelpers.Sync(
			source,
			destination,
			(s, d) => s.Name == d.Name,
			CreateProperty,
			(s, d) => d.Sync(s.Values, d.Values));

	private void Sync(IEnumerable<PivotDataItem> source, ObservableCollection<PivotDataItem> destination) =>
		CollectionHelpers.Sync(
			source,
			destination);
}
