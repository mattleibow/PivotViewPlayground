namespace PivotViewer.Core.VisualizerApp;

public partial class FilterPage : ContentPage
{
	private readonly PivotDataSource datasource;
	private readonly PivotDataSourceFilter filter;

	public FilterPage()
	{
		InitializeComponent();

		datasource = CxmlPivotDataSource.FromFile("C:\\Projects\\PivotViewPlayground\\PivotViewer.Core.Tests\\TestData\\conceptcars.cxml");

		filter = new PivotDataSourceFilter(datasource);

		BindingContext = filter;
	}
}

public class PivotDataSourceFilter : BindableObject
{
	// TODO: make readonly
	public static readonly BindableProperty CategoriesProperty = BindableProperty.Create(
		nameof(Properties), typeof(ObservableCollection<PivotFilterProperty>), typeof(PivotDataSourceFilter), null);

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
	}

	public string FilterString
	{
		get => (string)GetValue(FilterStringProperty);
		private set => SetValue(FilterStringProperty, value);
	}

	public ObservableCollection<PivotFilterProperty> Properties
	{
		get => (ObservableCollection<PivotFilterProperty>)GetValue(CategoriesProperty);
		private set => SetValue(CategoriesProperty, value);
	}

	private PivotFilterProperty CreateProperty(FilterProperty property) =>
		new(filter, property);
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
