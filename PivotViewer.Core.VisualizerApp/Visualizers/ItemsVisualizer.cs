namespace PivotViewer.Core.VisualizerApp.Visualizers;

public class ItemsVisualizer : Visualizer
{
	public ItemsVisualizer(string name, IReadOnlyList<PivotRendererItem> items)
		: base(name)
	{
		Items = items;
	}

	public IReadOnlyList<PivotRendererItem> Items { get; }

	[Switch("Show screen boundary lines")]
	public bool IsScreenLinesVisible { get; set; } = true;

	[Switch("Show items")]
	public bool IsItemsVisible { get; set; } = true;

	[Switch("Show desired locations")]
	public bool IsDesiredLocations { get; set; } = true;

	[Slider("Screen scale", 0.1, 1)]
	public double ScreenScale { get; set; } = 0.5;

	public override void Draw(ICanvas canvas, RectF bounds)
	{
		base.Draw(canvas, bounds);

		var scaledW = bounds.Width * ScreenScale;
		var scaledH = bounds.Height * ScreenScale;
		var screenRect = new RectF(
			(float)(bounds.X + (bounds.Width - scaledW) / 2),
			(float)(bounds.Y + (bounds.Height - scaledH) / 2),
			(float)scaledW,
			(float)scaledH);

		// draw items
		if (IsItemsVisible)
		{
			PrepareItems(screenRect);
			DrawItems(canvas, screenRect);
		}

		// draw "screen bounds"
		if (IsScreenLinesVisible)
		{
			canvas.StrokeColor = Colors.Gray;
			canvas.StrokeSize = 1;
			canvas.DrawRectangle(screenRect);
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

		foreach (var item in Items)
		{
			var r = IsDesiredLocations
				? item.Frame.Desired
				: item.Frame.Current;
			var itemRect = r.ToRect();

			canvas.FillRectangle(itemRect);
			canvas.DrawRectangle(itemRect);
			canvas.DrawString(item.Id, itemRect, HorizontalAlignment.Center, VerticalAlignment.Center);
		}
	}
}
