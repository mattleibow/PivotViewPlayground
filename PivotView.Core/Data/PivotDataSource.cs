namespace PivotView.Core.Data;

public class PivotDataSource
{
	private IList<PivotDataItem>? items;

	public IList<PivotDataItem>? Items
	{
		get => items;
		set
		{
			items = value;
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
