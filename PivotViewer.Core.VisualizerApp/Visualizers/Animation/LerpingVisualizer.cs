using System.Drawing;

namespace PivotViewer.Core.VisualizerApp.Visualizers.Animation;

public class LerpingVisualizer : AnimationVisualizer
{
	public LerpingVisualizer()
		: base($"RectangleF Lerping", new List<PivotRendererItem>(new[] { CreateAnimationItem() }))
	{
		LerpingFunction = Lerping.Lerps[typeof(RectangleF)];
	}

	public LerpingDelegate LerpingFunction { get; }

	protected override void PrepareItems(RectF bounds)
	{
		var p = Progress / 100.0;

		var from = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height / 9);
		var to = new RectangleF(bounds.X + (bounds.Width / 9) * 4, bounds.Y, bounds.Width / 9, bounds.Height);

		var rect = (RectangleF)LerpingFunction(from, to, p);

		Items[0].Frame.Current = rect;
		Items[0].Frame.Desired = rect;
	}

	private static PivotRendererItem CreateAnimationItem()
	{
		var dataItem = new PivotDataItem
		{
			ImageWidth = 10,
			ImageHeight = 10
		};

		var renderItem = new PivotRendererItem(dataItem);

		var rect = new RectangleF(0, 0, 10, 10);

		renderItem.Frame.Current = rect;
		renderItem.Frame.Desired = rect;

		return renderItem;
	}
}
