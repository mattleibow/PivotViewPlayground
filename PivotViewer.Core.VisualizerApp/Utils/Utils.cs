using System.Drawing;

namespace PivotViewer.Core.VisualizerApp;

internal static class Utils
{
	public static RectangleF ToSystemRectangleF(this RectF rect) =>
		new(rect.X, rect.Y, rect.Width, rect.Height);

	public static Rect ToRect(this RectangleF rect) =>
		new(rect.X, rect.Y, rect.Width, rect.Height);

	public static RectF ToRectF(this RectangleF rect) =>
		new(rect.X, rect.Y, rect.Width, rect.Height);
}
