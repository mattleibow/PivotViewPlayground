using System.Drawing;
using Pivot.Rendering;

namespace Pivot.Controls;

public class MauiGraphicsPivotVisualizationRenderer : PivotVisualizationRenderer<IMauiGraphicsPivotVisualizationCanvas>
{
	public MauiGraphicsPivotVisualizationRenderer(IPivotVisualizationSource source)
		: base(source)
	{
	}

	protected override void OnDrawBackground(IMauiGraphicsPivotVisualizationCanvas canvas, RectangleF frame)
	{
		if (canvas is not IPivotVisualizationDebugCanvas debugCanvas || debugCanvas.DebugOptions?.DrawRenderFrameBoundary != true)
			return;

		var cnv = canvas.Canvas;

		cnv.StrokeColor = Colors.Gray.WithAlpha(0.5f);
		cnv.StrokeSize = 1;

		cnv.DrawRectangle(frame.Left, frame.Top, frame.Width, frame.Height);
	}

	protected override void OnBeforeDraw(IMauiGraphicsPivotVisualizationCanvas canvas, RectangleF frame)
	{
		var cnv = canvas.Canvas;

		cnv.FillColor = Colors.White;
		cnv.FillRectangle(frame.Left, frame.Top, frame.Width, frame.Height);
	}

	protected override void OnBeforeDrawDebugItems(IMauiGraphicsPivotVisualizationCanvas canvas, RectangleF frame)
	{
		var cnv = canvas.Canvas;

		cnv.StrokeColor = Colors.Gray.WithAlpha(0.5f);
		cnv.StrokeSize = 1;
	}

	protected override void OnDrawDebugItem(IMauiGraphicsPivotVisualizationCanvas canvas, PivotVisualizationItem item, RectangleF itemFrame)
	{
		var cnv = canvas.Canvas;

		cnv.DrawRectangle(itemFrame.Left, itemFrame.Top, itemFrame.Width, itemFrame.Height);
	}

	protected override void OnBeforeDrawItems(IMauiGraphicsPivotVisualizationCanvas canvas, RectangleF frame)
	{
		var cnv = canvas.Canvas;

		cnv.FillColor = Colors.LightGoldenrodYellow;
		cnv.StrokeColor = Colors.Gray;
		cnv.StrokeSize = 1;
	}

	protected override void OnDrawItem(IMauiGraphicsPivotVisualizationCanvas canvas, PivotVisualizationItem item, RectangleF itemFrame)
	{
		var rect = new Rect(itemFrame.Left, itemFrame.Top, itemFrame.Width, itemFrame.Height);

		var cnv = canvas.Canvas;

		cnv.FillRectangle(rect);
		cnv.DrawRectangle(rect);

		cnv.DrawString(item.Id, rect, HorizontalAlignment.Center, VerticalAlignment.Center);
	}
}
