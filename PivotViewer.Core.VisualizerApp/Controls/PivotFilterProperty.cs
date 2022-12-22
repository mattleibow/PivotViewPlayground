namespace PivotViewer.Core.VisualizerApp.Controls;

public class PivotFilterProperty : BindableObject
{
	// TODO: make readonly
	public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
		nameof(Values), typeof(ObservableCollection<PivotFilterValue>), typeof(PivotFilterProperty),
		defaultValueCreator: (b) => new ObservableCollection<PivotFilterValue>());

	// TODO: make readonly
	public static readonly BindableProperty NameProperty = BindableProperty.Create(
		nameof(Name), typeof(string), typeof(PivotFilterProperty), null);

	private readonly FilterManager filter;
	private readonly FilterProperty property;

	internal PivotFilterProperty(FilterManager filter, FilterProperty property)
	{
		this.filter = filter;
		this.property = property;

		Name = property.Name;

		SyncFilterCollections(property.Values, Values);
	}

	public string Name
	{
		get => (string)GetValue(NameProperty);
		private set => SetValue(NameProperty, value);
	}

	public ObservableCollection<PivotFilterValue> Values
	{
		get => (ObservableCollection<PivotFilterValue>)GetValue(ValuesProperty);
		private set => SetValue(ValuesProperty, value);
	}

	internal void SyncFilterCollections(FilterValueCollection source, ObservableCollection<PivotFilterValue> destination) =>
		CollectionHelpers.Sync(
			source,
			destination,
			(s, d) => s.Value.CompareTo(d.Value) == 0,
			(s) => new(filter, property, s));
}
