namespace PivotView.Core.Tests;

public class EasingUnitTests
{
    [Theory]
    [InlineData(-0.1, -0.1)]
    [InlineData(0.0, 0.0)]
    [InlineData(0.1, 0.1)]
    [InlineData(0.5, 0.5)]
    [InlineData(0.9, 0.9)]
    [InlineData(1.0, 1.0)]
    [InlineData(1.1, 1.1)]
    public void Linear(double progress, double expected)
    {
        var actual = Easing.Linear(progress);
        Assert.Equal(expected, actual);
    }
}
