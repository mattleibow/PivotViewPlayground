namespace PivotViewer.Core.Animation;

class BufferedDelegate : IDisposable
{
	private long lastAnimate;
	private Timer? animateTimer;

	public void Post(TimeSpan delay, Action action)
	{
		var newAnimate = Environment.TickCount64;
		var delta = newAnimate - lastAnimate;
		lastAnimate = newAnimate;

		animateTimer?.Dispose();
		animateTimer = null;

		if (delta < delay.TotalMilliseconds)
		{
			animateTimer = new Timer(_ => Post(delay, action), null, delay, Timeout.InfiniteTimeSpan);
			return;
		}

		action();
	}

	public void Dispose()
	{
		Reset();
	}

	public void Reset()
	{
		animateTimer?.Dispose();
	}
}
