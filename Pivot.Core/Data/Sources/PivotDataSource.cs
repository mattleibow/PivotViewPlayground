using Pivot.Data.Model;

namespace Pivot.Data.Sources;

/// <summary>
/// This type represents the data source to be used for visualizing data.
/// </summary>
public class PivotDataSource
{
	private PivotDataItemCollection items = new();
	private PivotPropertyCollection properties = new();

	public PivotDataItemCollection Items
	{
		get => items;
		set
		{
			items = value;
			OnItemsChanged();
		}
	}

	public PivotPropertyCollection Properties
	{
		get => properties;
		set
		{
			properties = value;
			OnItemsChanged();
		}
	}

	private void OnItemsChanged()
	{
		if (items is null || items.Count == 0)
			return;

		// TODO
	}
}
