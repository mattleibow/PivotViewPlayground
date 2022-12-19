namespace PivotViewer.Core.Tests;

public class LerpingUnitTests
{
    [Theory]
    // double
    [InlineData(0.0, 1.0, 0.0, 0.0)]
    [InlineData(0.0, 1.0, 0.3, 0.3)]
    [InlineData(0.0, 1.0, 0.5, 0.5)]
    [InlineData(0.0, 1.0, 0.7, 0.7)]
    [InlineData(0.0, 1.0, 1.0, 1.0)]
    // float
    [InlineData(0.0f, 1.0f, 0.0, 0.0f)]
    [InlineData(0.0f, 1.0f, 0.3, 0.3f)]
    [InlineData(0.0f, 1.0f, 0.5, 0.5f)]
    [InlineData(0.0f, 1.0f, 0.7, 0.7f)]
    [InlineData(0.0f, 1.0f, 1.0, 1.0f)]
    public void CanLerpSimplePrimatives(object start, object end, double progress, object expected)
    {
        var startType = start.GetType();
        var endType = end.GetType();

        Assert.Equal(startType, endType);

        var actual = Lerping.Lerps[startType](start, end, progress);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 0.0, new[] { 0f, 0f, 100f, 100f })]
    [InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 0.5, new[] { 5f, 5f, 75f, 75f })]
    [InlineData(new[] { 0f, 0f, 100f, 100f }, new[] { 10f, 10f, 50f, 50f }, 1.0, new[] { 10f, 10f, 50f, 50f })]
    public void CanLerpRectangeF(float[] s, float[] e, double progress, float[] exp)
    {
        var start = new RectangleF(s[0], s[1], s[2], s[3]);
        var end = new RectangleF(e[0], e[1], e[2], e[3]);
        var expected = new RectangleF(exp[0], exp[1], exp[2], exp[3]);

        var actual = Lerping.Lerps[typeof(RectangleF)](start, end, progress);

        Assert.Equal(expected, actual);
    }
}
