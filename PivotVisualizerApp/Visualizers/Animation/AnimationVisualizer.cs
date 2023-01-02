namespace PivotVisualizerApp.Visualizers.Animation;

public class AnimationVisualizer : ItemsVisualizer
{
	long lastTime;

	public AnimationVisualizer(string name, IReadOnlyList<PivotRendererItem> items)
		: base(name + " Animation", items)
	{
	}

	[Slider("Progress (0-100)", 0, 100)]
	public double Progress { get; set; }

	[Slider("Duration (seconds)", 0, 10)]
	public double Duration { get; set; } = 3;

	[Switch("Pause updates")]
	public bool IsPaused { get; set; }

	public void TimerTick()
	{
		var now = Environment.TickCount64;
		if (lastTime == 0)
			lastTime = now;

		var dt = TimeSpan.FromMilliseconds(now - lastTime);
		lastTime = now;

		if (IsPaused)
		{
			lastTime = 0;
			return;
		}

		var current = TimeSpan.FromSeconds(Duration * (Progress / 100));
		var sec = (current + dt).TotalSeconds;

		Progress = ((sec / Duration) * 100) % 100;
		InvalidateDrawing();
	}
}
