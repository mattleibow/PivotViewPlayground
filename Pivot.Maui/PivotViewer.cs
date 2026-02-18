using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Pivot.Data.Model;
using Pivot.Data.Sources;

namespace Pivot.Controls;

/// <summary>
/// Represents a control that enables a user to present and interact with a large amount of data.
/// </summary>
public class PivotViewer : TemplatedView
{
	/// <summary>
	/// Identifies the <see cref="ItemsSource"/> bindable property.
	/// </summary>
	public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
		nameof(ItemsSource), typeof(IEnumerable), typeof(PivotViewer), null,
		propertyChanging: OnItemsSourceChanging,
		propertyChanged: OnItemsSourceChanged);

	/// <summary>
	/// Identifies the <see cref="PivotProperties"/> bindable property.
	/// </summary>
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

	/// <summary>
	/// Gets or sets a collection that is used to generate the items in the <see cref="PivotViewer"/> control.
	/// </summary>
	public IEnumerable? ItemsSource
	{
		get => (IEnumerable?)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	/// <summary>
	/// Gets or sets the list of pivot properties that are mapped to the properties on the objects in <see cref="ItemsSource"/>.
	/// </summary>
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
