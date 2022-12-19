namespace PivotView.Core.Animation;

public delegate object LerpingDelegate(object? start, object? end, double progress);

static public class Lerping
{
	public static readonly Dictionary<Type, LerpingDelegate> Lerps =
		new()
		{
			[typeof(int)] = (s, e, p) => Lerp(Convert.ToInt32(s), Convert.ToInt32(e), p),
			[typeof(float)] = (s, e, p) => Lerp(Convert.ToSingle(s), Convert.ToSingle(e), p),
			[typeof(double)] = (s, e, p) => Lerp(Convert.ToDouble(s), Convert.ToDouble(e), p),
			[typeof(PointF)] = (s, e, p) => Lerp(ToPointF(s), ToPointF(e), p),
			[typeof(SizeF)] = (s, e, p) => Lerp(ToSizeF(s), ToSizeF(e), p),
			[typeof(RectangleF)] = (s, e, p) => Lerp(ToRectangleF(s), ToRectangleF(e), p),
		};

	public static int Lerp(int start, int end, double progress) =>
		(int)((end - start) * progress) + start;

	public static float Lerp(float start, float end, double progress) =>
		(float)((end - start) * progress) + start;

	public static double Lerp(double start, double end, double progress) =>
		(double)((end - start) * progress) + start;

	public static PointF Lerp(PointF start, PointF end, double progress) =>
		new(Lerp(start.X, end.X, progress), Lerp(start.Y, end.Y, progress));

	public static SizeF Lerp(SizeF start, SizeF end, double progress) =>
		new(Lerp(start.Width, end.Width, progress), Lerp(start.Height, end.Height, progress));

	public static RectangleF Lerp(RectangleF start, RectangleF end, double progress) =>
		new(Lerp(start.Location, end.Location, progress), Lerp(start.Size, end.Size, progress));

	private static PointF ToPointF(object? s) =>
		s is null ? PointF.Empty : (PointF)s;

	private static SizeF ToSizeF(object? s) =>
		s is null ? SizeF.Empty : (SizeF)s;

	private static RectangleF ToRectangleF(object? s) =>
		s is null ? RectangleF.Empty : (RectangleF)s;
}
