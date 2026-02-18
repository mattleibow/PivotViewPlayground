namespace PivotVisualizerApp.Visualizers;

public class SimpleVisualizer : Visualizer
{
	public SimpleVisualizer(string name)
		: base(name)
	{
	}

	[Switch("Show screen boundary lines")]
	public bool IsScreenLinesVisible { get; set; } = true;

	[Slider("Screen scale", 0.1, 1)]
	public double ScreenScale { get; set; } = 0.5;

	public override void Draw(ICanvas canvas, RectF bounds)
	{
		DrawBackground(canvas, bounds);

		// calculate the "screen" bounds
		var scaledW = bounds.Width * ScreenScale;
		var scaledH = bounds.Height * ScreenScale;
		var screenRect = new RectF(
			(float)(bounds.X + (bounds.Width - scaledW) / 2),
			(float)(bounds.Y + (bounds.Height - scaledH) / 2),
			(float)scaledW,
			(float)scaledH);

		DrawScreen(canvas, screenRect);

		// draw "screen bounds"
		if (IsScreenLinesVisible)
		{
			canvas.StrokeColor = Colors.Gray;
			canvas.StrokeSize = 1;
			canvas.DrawRectangle(screenRect);
		}
	}

	protected virtual void DrawBackground(ICanvas canvas, RectF bounds)
	{
		// clear
		canvas.FillColor = Colors.White;
		canvas.FillRectangle(bounds);
	}

	protected virtual void DrawScreen(ICanvas canvas, RectF bounds)
	{
	}
}
