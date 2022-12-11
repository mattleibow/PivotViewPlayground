using System.Drawing;

namespace PivotView.Core.VisualizerApp;

internal static class Utils
{
    public static RectangleF ToSystemRectangleF(this RectF rect) =>
        new(rect.X, rect.Y, rect.Width, rect.Height);

    public static Rect ToRect(this RectangleF rect) =>
        new(rect.X, rect.Y, rect.Width, rect.Height);
}
