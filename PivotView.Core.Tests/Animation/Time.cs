namespace PivotView.Core.Tests.Animation;

internal static class Time
{
    public static TimeSpan Zero = TimeSpan.Zero;

    public static TimeSpan FifthSec = TimeSpan.FromMilliseconds(1000.0 / 5); // 200
    public static TimeSpan QuarterSec = TimeSpan.FromMilliseconds(1000.0 / 4); // 250
    public static TimeSpan ThirdSec = TimeSpan.FromMilliseconds(1000.0 / 3); // 333.333
    public static TimeSpan HalfSec = TimeSpan.FromMilliseconds(1000.0 / 2); // 500
    public static TimeSpan OneSec = TimeSpan.FromSeconds(1);
    public static TimeSpan OneAndHalfSec = TimeSpan.FromSeconds(1.5);
    public static TimeSpan TwoSec = TimeSpan.FromSeconds(1);

    public static TimeSpan OneHundredMilli = TimeSpan.FromMilliseconds(100);
    public static TimeSpan FourHundredMilli = TimeSpan.FromMilliseconds(400);
    public static TimeSpan EightHundredMilli = TimeSpan.FromMilliseconds(800);
}
