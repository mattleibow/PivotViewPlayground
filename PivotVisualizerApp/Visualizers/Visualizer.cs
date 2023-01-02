using System.Reflection;

namespace PivotVisualizerApp.Visualizers;

public class Visualizer : BindableObject, IDrawable
{
	private readonly List<VisualizerProperty> properties = new();

	public Visualizer(string name)
	{
		Name = name;
	}

	public string Name { get; }

	public IReadOnlyList<VisualizerProperty> Properties => GetOrBuildProperties();

	[Switch("Show screen boundary lines")]
	public bool IsScreenLinesVisible { get; set; } = true;

	[Slider("Screen scale", 0.1, 1)]
	public double ScreenScale { get; set; } = 0.5;

	public void InvalidateDrawing() =>
		OnPropertyChanged(nameof(IDrawable));

	public void Draw(ICanvas canvas, RectF bounds)
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

	protected virtual void OnVisualizerPropertyValueChanged()
	{
		InvalidateDrawing();
	}

	private IReadOnlyList<VisualizerProperty> GetOrBuildProperties()
	{
		if (properties.Count == 0)
		{
			var props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach (var property in props)
			{
				if (property.GetCustomAttribute<VisualizerPropertyAttribute>() is not null)
				{
					var item = new VisualizerProperty(this, property);
					item.PropertyChanged += (s, e) =>
					{
						if (e.PropertyName == nameof(VisualizerProperty.Value))
							OnVisualizerPropertyValueChanged();
					};
					properties.Add(item);
				}
			}
		}

		return properties;
	}
}
