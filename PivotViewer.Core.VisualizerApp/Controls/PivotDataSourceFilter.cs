using System.Text;

namespace PivotViewer.Core.VisualizerApp.Controls;

public class PivotDataSourceFilter : BindableObject
{
	// TODO: make readonly
	public static readonly BindableProperty PropertiesProperty = BindableProperty.Create(
		nameof(Properties), typeof(ObservableCollection<PivotFilterProperty>), typeof(PivotDataSourceFilter), null);

	// TODO: make readonly
	public static readonly BindableProperty FilterProperty = BindableProperty.Create(
		nameof(Filter), typeof(ObservableCollection<PivotFilterProperty>), typeof(PivotDataSourceFilter), new ObservableCollection<PivotFilterProperty>());

	// TODO: make readonly
	public static readonly BindableProperty FilterStringProperty = BindableProperty.Create(
		nameof(FilterString), typeof(string), typeof(PivotDataSourceFilter), null);

	private PivotDataSource datasource;
	private FilterManager filter;

	public PivotDataSourceFilter(PivotDataSource datasource)
	{
		this.datasource = datasource;

		filter = new FilterManager(datasource);
		filter.RebuildIndexes();

		Properties = new(filter.AvailableFilters.Select(CreateProperty));

		filter.FilterUpdated += OnFilterUpdated;
	}

	public string FilterString
	{
		get => (string)GetValue(FilterStringProperty);
		private set => SetValue(FilterStringProperty, value);
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

	public event EventHandler? FilterUpdated;

	private PivotFilterProperty CreateProperty(FilterProperty property) =>
		new(filter, property);

	private void OnFilterUpdated(object? sender, EventArgs e)
	{
		FilterString = GetFilterString(filter.AppliedFilters);

		SyncFilterCollections(filter.AppliedFilters, Filter);

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

	private void SyncFilterCollections(FilterPropertyCollection source, ObservableCollection<PivotFilterProperty> destination)
	{
		foreach (var src in source)
		{
			var dest = destination.FirstOrDefault(d => src.Name == d.Name);

			if (dest is null)
				destination.Add(CreateProperty(src));
			else
				dest.SyncFilterCollections(src.Values, dest.Values);
		}

		for (var i = destination.Count - 1; i >= 0; i--)
		{
			if (source.All(s => s.Name != destination[i].Name))
			{
				destination.RemoveAt(i);
			}
		}
	}
}
