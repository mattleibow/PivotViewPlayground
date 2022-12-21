namespace PivotViewer.Core.VisualizerApp.Controls;

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
