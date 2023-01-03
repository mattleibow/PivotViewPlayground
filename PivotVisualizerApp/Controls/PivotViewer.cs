using System.Collections;
using System.Collections.Specialized;

namespace PivotVisualizerApp.Controls;

public class PivotViewer : TemplatedView
{
	public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
		nameof(ItemsSource), typeof(IEnumerable), typeof(PivotViewer), null,
		propertyChanging: OnItemsSourceChanging,
		propertyChanged: OnItemsSourceChanged);

	public static readonly BindableProperty PivotPropertiesProperty = BindableProperty.Create(
		nameof(PivotProperties), typeof(ICollection<PivotViewerProperty>), typeof(PivotViewer),
		defaultValueCreator: bindable =>
		{
			var def = new ObservableCollection<PivotViewerProperty>();
			OnPivotPropertiesChanged(bindable, null, def);
			return def;
		},
		propertyChanging: OnPivotPropertiesChanging,
		propertyChanged: OnPivotPropertiesChanged);

	private readonly ViewModels.FilterViewModel filterViewModel = new();

	private PivotDataSource? dataSource;

	private PivotViewerFilterPane? PART_FilterPane;

	public PivotViewer()
	{
		Themes.PivotViewerResources.EnsureRegistered();
	}

	protected override void OnApplyTemplate()
	{
		PART_FilterPane = GetTemplateChild("PART_FilterPane") as PivotViewerFilterPane;
		if (PART_FilterPane is not null)
			PART_FilterPane.ViewModel = filterViewModel;
	}

	public IEnumerable? ItemsSource
	{
		get => (IEnumerable?)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public ICollection<PivotViewerProperty>? PivotProperties
	{
		get => (ICollection<PivotViewerProperty>?)GetValue(PivotPropertiesProperty);
		set => SetValue(PivotPropertiesProperty, value);
	}

	private void RecreateDataSource()
	{
		// TODO: anything but this as this is the slowest and worst thing ever

		dataSource = new PivotDataSource();

		var pivotViewerPivotProperties = new List<PivotViewerPivotProperty>();
		if (PivotProperties is not null)
		{
			foreach (var pivotViewerProperty in PivotProperties)
			{
				var property = new PivotViewerPivotProperty(pivotViewerProperty);
				dataSource.Properties.Add(property);
				pivotViewerPivotProperties.Add(property);
			}
		}

		if (ItemsSource is not null)
		{
			foreach (var pivotViewerItem in ItemsSource)
			{
				var item = new PivotViewerPivotDataItem(pivotViewerItem);
				dataSource.Items.Add(item);

				foreach (var property in pivotViewerPivotProperties)
				{
					item.BindProperty(property.PivotViewerProperty);

					var value = item.GetPropertyValue(property.PivotViewerProperty);
					if (value is not null)
					{
						var values = new PivotPropertyValueCollection();

						if (value is ICollection collection)
						{
							foreach (var val in collection)
								values.Add((IComparable)val);
						}
						else
						{
							values.Add((IComparable)value);
						}

						item.Properties[property] = values;
					}
				}
			}
		}

		filterViewModel.DataSource = dataSource;
	}

	private void OnPivotPropertiesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		RecreateDataSource();
	}

	private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		RecreateDataSource();
	}

	private static void OnPivotPropertiesChanging(BindableObject? bindable, object? oldValue, object? newValue)
	{
		if (bindable is not PivotViewer pv)
			return;

		if (oldValue is INotifyCollectionChanged incc)
			incc.CollectionChanged -= pv.OnPivotPropertiesCollectionChanged;
	}

	private static void OnPivotPropertiesChanged(BindableObject? bindable, object? oldValue, object? newValue)
	{
		if (bindable is not PivotViewer pv)
			return;

		if (newValue is INotifyCollectionChanged incc)
			incc.CollectionChanged += pv.OnPivotPropertiesCollectionChanged;
	}

	private static void OnItemsSourceChanging(BindableObject? bindable, object? oldValue, object? newValue)
	{
		if (bindable is not PivotViewer pv)
			return;

		if (oldValue is INotifyCollectionChanged incc)
			incc.CollectionChanged -= pv.OnItemsSourceCollectionChanged;
	}

	private static void OnItemsSourceChanged(BindableObject? bindable, object? oldValue, object? newValue)
	{
		if (bindable is not PivotViewer pv)
			return;

		if (newValue is INotifyCollectionChanged incc)
			incc.CollectionChanged += pv.OnItemsSourceCollectionChanged;

		pv.RecreateDataSource();
	}
}

internal class PivotViewerPivotProperty : PivotProperty
{
	public PivotViewerPivotProperty(PivotViewerProperty property)
		: base(property.Name)
	{
		PivotViewerProperty = property;
	}

	public PivotViewerProperty PivotViewerProperty { get; }
}

internal class PivotViewerPivotDataItem : PivotDataItem
{
	private readonly PivotViewerBindableItem bindableItem = new();
	private readonly Dictionary<string, PivotViewerProperty> pivotViewerProperties = new();

	public PivotViewerPivotDataItem(object item)
	{
		Id = Guid.NewGuid().ToString();

		PivotViewerItem = item;
	}

	public object PivotViewerItem { get; }

	public void BindProperty(PivotViewerProperty property)
	{
		var binding = new Binding
		{
			Source = PivotViewerItem,
			Path = property.ActualBinding.Path,
		};

		bindableItem.SetBinding(property.BindableProperty, binding);

		pivotViewerProperties.Add(property.Name, property);
	}

	public object? GetPropertyValue(string propertyName) =>
		GetPropertyValue(pivotViewerProperties[propertyName]);

	public object? GetPropertyValue(PivotViewerProperty property) =>
		bindableItem.GetValue(property.BindableProperty);
}

internal class PivotViewerBindableItem : BindableObject
{
}

public class PivotViewerProperty
{
	private BindableProperty? bindableProperty;
	private Binding? binding;

	public string? Name { get; set; }

	public BindingBase? Binding
	{
		get => binding;
		set => binding = (Binding?)value;
	}

	internal Binding ActualBinding =>
		binding ?? throw new InvalidOperationException("Cannot reading binding if no binding is set.");

	internal BindableProperty BindableProperty =>
		bindableProperty ??= BindableProperty.Create(ActualBinding.Path, typeof(object), typeof(PivotViewerBindableItem), null);
}
