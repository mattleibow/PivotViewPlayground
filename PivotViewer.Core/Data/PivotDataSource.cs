namespace PivotViewer.Core.Data;

public class PivotDataSource
{
	private PivotDataItemCollection items = new();
	private PivotPropertyCollection properties = new();

	public string? Name { get; set; }

	public string? Icon { get; set; }

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
