namespace PivotViewer.Core.Data;

public class PivotDataSource
{
	private IList<PivotDataItem>? items = new List<PivotDataItem>();
	private IList<PivotDataProperty>? properties = new List<PivotDataProperty>();

	public string? Name { get; set; }

	public string? Icon { get; set; }

	public IList<PivotDataItem>? Items
	{
		get => items;
		set
		{
			items = value;
			OnItemsChanged();
		}
	}

	public IList<PivotDataProperty>? Properties
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
