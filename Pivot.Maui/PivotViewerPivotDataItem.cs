using Pivot.Data.Model;

namespace Pivot.Controls;

internal class PivotViewerPivotDataItem : PivotDataItem
{
	private readonly PivotViewerPivotDataItemBindingProxy bindableItem = new();
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
