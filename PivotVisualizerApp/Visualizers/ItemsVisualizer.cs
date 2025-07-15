namespace PivotVisualizerApp.Visualizers;

public class ItemsVisualizer : SimpleVisualizer
{
	public ItemsVisualizer(string name, IReadOnlyList<PivotVisualizationItem> items)
		: base(name)
	{
		Items = items;
	}

	public IReadOnlyList<PivotVisualizationItem> Items { get; }

	[Switch("Show items")]
	public bool IsItemsVisible { get; set; } = true;

	[Switch("Show desired locations")]
	public bool IsDesiredLocations { get; set; } = true;

	protected override void DrawScreen(ICanvas canvas, RectF bounds)
	{
		base.DrawScreen(canvas, bounds);

		// draw items
		if (IsItemsVisible)
		{
			PrepareItems(bounds);
			DrawItems(canvas, bounds);
		}
	}

	protected virtual void PrepareItems(RectF bounds)
	{
	}

	protected virtual void DrawItems(ICanvas canvas, RectF bounds)
	{
		// draw the final destinations for debug reasons
		if (IsDesiredLocations)
		{
			canvas.FillColor = Colors.Transparent;
			canvas.StrokeColor = Colors.Gray.WithAlpha(0.5f);
			canvas.StrokeSize = 1;

			foreach (var item in Items)
			{
				DrawItem(canvas, item, item.Frame.Desired.ToRect());
			}
		}

		canvas.FillColor = Colors.LightGoldenrodYellow;
		canvas.StrokeColor = Colors.Gray;
		canvas.StrokeSize = 1;

		var hasMovingItems = false;

		// draw static items below
		foreach (var item in Items)
		{
			hasMovingItems = hasMovingItems || !item.Frame.IsCurrentDesired;

			if (item.Frame.IsCurrentDesired)
			{
				DrawItem(canvas, item, item.Frame.Current.ToRect());
			}
		}

		// draw moving items on top
		if (hasMovingItems)
		{
			foreach (var item in Items)
			{
				if (!item.Frame.IsCurrentDesired)
				{
					DrawItem(canvas, item, item.Frame.Current.ToRect());
				}
			}
		}
	}

	protected virtual void DrawItem(ICanvas canvas, PivotVisualizationItem item, Rect rect)
	{
		canvas.FillRectangle(rect);
		canvas.DrawRectangle(rect);

		canvas.DrawString(item.Id, rect, HorizontalAlignment.Center, VerticalAlignment.Center);
	}
}
