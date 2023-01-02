namespace PivotVisualizerApp.Visualizers;

public class ItemsVisualizer : Visualizer
{
	public ItemsVisualizer(string name, IReadOnlyList<PivotRendererItem> items)
		: base(name)
	{
		Items = items;
	}

	public IReadOnlyList<PivotRendererItem> Items { get; }

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
		canvas.FillColor = Colors.LightGoldenrodYellow;
		canvas.StrokeColor = Colors.Gray;
		canvas.StrokeSize = 1;

		if (IsDesiredLocations)
		{
			foreach (var item in Items)
			{
				DrawItem(canvas, item, item.Frame.Desired.ToRect());
			}
		}
		else
		{
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
	}

	protected virtual void DrawItem(ICanvas canvas, PivotRendererItem item, Rect rect)
	{
		canvas.FillRectangle(rect);
		canvas.DrawRectangle(rect);

		canvas.DrawString(item.Id, rect, HorizontalAlignment.Center, VerticalAlignment.Center);
	}
}
