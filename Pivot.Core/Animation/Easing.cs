namespace Pivot.Core.Animation;

public delegate double EasingDelegate(double progress);

public static class Easing
{
	public static readonly EasingDelegate Linear = new(x => x);

	public static readonly EasingDelegate SinOut = new(x => Math.Sin(x * Math.PI * 0.5f));

	public static readonly EasingDelegate SinIn = new(x => 1.0f - Math.Cos(x * Math.PI * 0.5f));

	public static readonly EasingDelegate SinInOut = new(x => -Math.Cos(Math.PI * x) / 2.0f + 0.5f);

	public static readonly EasingDelegate CubicIn = new(x => x * x * x);

	public static readonly EasingDelegate CubicOut = new(x => Math.Pow(x - 1.0f, 3.0f) + 1.0f);

	public static readonly EasingDelegate CubicInOut = new(x => x < 0.5f ? Math.Pow(x * 2.0f, 3.0f) / 2.0f : (Math.Pow((x - 1) * 2.0f, 3.0f) + 2.0f) / 2.0f);

	public static readonly EasingDelegate SpringIn = new(x => x * x * ((1.70158f + 1) * x - 1.70158f));

	public static readonly EasingDelegate SpringOut = new(x => (x - 1) * (x - 1) * ((1.70158f + 1) * (x - 1) + 1.70158f) + 1);
}
