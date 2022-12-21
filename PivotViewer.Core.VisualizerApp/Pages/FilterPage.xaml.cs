using System.Text;

namespace PivotViewer.Core.VisualizerApp;

public partial class FilterPage : ContentPage
{
	private readonly PivotDataSource datasource;

	public FilterPage()
	{
		InitializeComponent();

		datasource = CxmlPivotDataSource.FromFile("C:\\Projects\\PivotViewPlayground\\PivotViewer.Core.Tests\\TestData\\conceptcars.cxml");

		Filter = new PivotDataSourceFilter(datasource);

		Filter.FilterUpdated += (sender, e) =>
		{
			// TODO: update FilteredItems
		};

		BindingContext = this;
	}

	public PivotDataSourceFilter Filter { get; }

	public ObservableCollection<PivotDataItem> FilteredItems { get; } = new();
}

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

public class PivotFilterProperty : BindableObject
{
	// TODO: make readonly
	public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
		nameof(Values), typeof(ObservableCollection<PivotFilterValue>), typeof(PivotFilterProperty), null);

	private readonly FilterManager filter;
	private readonly FilterProperty property;

	public PivotFilterProperty(FilterManager filter, FilterProperty property)
	{
		this.filter = filter;
		this.property = property;

		Values = new(property.Values.Select(CreateValue));
	}

	public string Name => property.Name;

	public int Count => property.Count;

	public ObservableCollection<PivotFilterValue> Values
	{
		get => (ObservableCollection<PivotFilterValue>)GetValue(ValuesProperty);
		private set => SetValue(ValuesProperty, value);
	}

	private PivotFilterValue CreateValue(FilterValue value) =>
		new(filter, property, value);

	internal void SyncFilterCollections(FilterValueCollection source, ObservableCollection<PivotFilterValue> destination)
	{
		foreach (var src in source)
		{
			if (destination.All(d => src.Value.CompareTo(d.Value) != 0))
				destination.Add(CreateValue(src));
		}

		for (var i = destination.Count - 1; i >= 0; i--)
		{
			if (source.All(s => s.Value.CompareTo(destination[i].Value) != 0))
			{
				destination.RemoveAt(i);
			}
		}
	}
}

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
