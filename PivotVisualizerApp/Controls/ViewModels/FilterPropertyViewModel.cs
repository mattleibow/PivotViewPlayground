using System.Windows.Input;

namespace PivotVisualizerApp.Controls.ViewModels;

public class FilterPropertyViewModel : BindableObject
{
	private readonly FilterManager filter;
	private readonly FilterProperty property;

	private bool isApplied;
	private Command? clearCommand;

	internal FilterPropertyViewModel(FilterManager filter, FilterProperty property)
	{
		this.filter = filter;
		this.property = property;

		Name = property.Name;

		Sync(property.Values, Values);
	}

	public string Name { get; }

	public ObservableCollection<FilterValueViewModel> Values { get; } = new();

	public bool IsApplied
	{
		get => isApplied;
		set
		{
			if (isApplied == value)
				return;

			isApplied = value;
			OnPropertyChanged();
		}
	}

	public ICommand ClearCommand => clearCommand ??= new Command(OnClear);

	private void OnClear()
	{
		filter.AppliedFilters.UnapplyProperty(property.PivotProperty);
	}

	internal void Sync(FilterValueCollection source, ObservableCollection<FilterValueViewModel> destination)
	{
		CollectionHelpers.Sync(
			source,
			destination,
			(s, d) => s.Value.CompareTo(d.Value) == 0,
			(s) => new(filter, property, s),
			(s, d) => d.Sync(s));

		IsApplied = destination.Any(d => d.IsApplied);
	}
}
