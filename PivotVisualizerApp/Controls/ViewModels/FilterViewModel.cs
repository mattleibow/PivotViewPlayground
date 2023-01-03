using System.Windows.Input;

namespace PivotVisualizerApp.Controls.ViewModels;

public class FilterViewModel : BindableObject
{
	private readonly FilterManager filter;
	
	private bool isFilterApplied;
	private Command? clearAllCommand;

	public FilterViewModel()
	{
		filter = new FilterManager();

		filter.FilterUpdated += OnFilterUpdated;
	}

	public FilterViewModel(PivotDataSource datasource)
		: this()
	{
		DataSource = datasource;
	}

	public bool IsFilterApplied
	{
		get => isFilterApplied;
		set
		{
			if (isFilterApplied == value)
				return;

			isFilterApplied = value;
			OnPropertyChanged();
		}
	}

	public ICommand ClearAllCommand => clearAllCommand ??= new Command(OnClearAll);

	public ObservableCollection<FilterPropertyViewModel> Properties { get; } = new();

	public ObservableCollection<FilterPropertyViewModel> Filter { get; } = new();

	public ObservableCollection<PivotDataItem> FilteredItems { get; } = new();

	public event EventHandler? FilterUpdated;

	public PivotDataSource? DataSource
	{
		get => filter.DataSource;
		set
		{
			if (filter.DataSource == value)
				return;

			filter.DataSource = value;
			filter.RebuildIndexes();
			OnPropertyChanged();
		}
	}

	private void OnClearAll()
	{
		filter.AppliedFilters.Clear();
	}

	private FilterPropertyViewModel CreateProperty(FilterProperty property) =>
		new(filter, property);

	private void OnFilterUpdated(object? sender, EventArgs e)
	{
		Sync(filter.AvailableFilters, Properties);
		Sync(filter.AppliedFilters, Filter);

		Sync(filter.FilteredItems, FilteredItems);

		IsFilterApplied = filter.AppliedFilters.Count != 0;

		FilterUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void Sync(FilterPropertyCollection source, ObservableCollection<FilterPropertyViewModel> destination) =>
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
