namespace Pivot.Rendering;

public class PivotVisualizationRenderer : PivotVisualizationRenderer<IPivotVisualizationCanvas>
{
	public PivotVisualizationRenderer(IPivotVisualizationSource controller)
		: base(controller)
	{
	}
}

public class PivotVisualizationRenderer<TCanvas> : IPivotVisualizationRenderer
	where TCanvas : IPivotVisualizationCanvas
{
	public PivotVisualizationRenderer(IPivotVisualizationSource source)
	{
		Source = source;
	}

	public IPivotVisualizationSource Source { get; }

	void IPivotVisualizationRenderer.Draw(IPivotVisualizationCanvas canvas, RectangleF frame) =>
		Draw((TCanvas)canvas, frame);

	public void Draw(TCanvas canvas, RectangleF frame)
	{
		OnBeforeDraw(canvas, frame);

		var renderFrame = frame;

		var debugOptions = (canvas as IPivotVisualizationDebugCanvas)?.DebugOptions;

		if (debugOptions is not null)
		{
			var scale = debugOptions.RenderScale;
			if (scale != 1.0f)
			{
				var scaledW = frame.Width * scale;
				var scaledH = frame.Height * scale;
				renderFrame = new RectangleF(
					(float)(frame.X + (frame.Width - scaledW) / 2f),
					(float)(frame.Y + (frame.Height - scaledH) / 2f),
					(float)scaledW,
					(float)scaledH);
			}
		}

		Source.RenderFrame = renderFrame;

		OnDrawBackground(canvas, renderFrame);

		if (debugOptions?.DrawItems != false)
		{
			OnDrawItems(canvas, renderFrame);
		}

		OnDrawForeground(canvas, renderFrame);

		OnAfterDraw(canvas, frame);
	}

	private void OnDrawItems(TCanvas canvas, RectangleF frame)
	{
		var items = Source.Items;
		if (items.Count == 0)
			return;

		// draw items to help debugging
		if (canvas is IPivotVisualizationDebugCanvas debugCanvas && debugCanvas.DebugOptions?.DrawDebugItems == true)
		{
			OnBeforeDrawDebugItems(canvas, frame);

			foreach (var item in items)
			{
				if (!item.Frame.IsCurrentDesired)
				{
					OnDrawDebugItem(canvas, item, item.Frame.Desired);
				}
			}

			OnAfterDrawDebugItems(canvas, frame);
		}

		var hasMovingItems = false;

		OnBeforeDrawItems(canvas, frame);

		OnBeforeDrawStaticItems(canvas, frame);

		// draw static items below
		foreach (var item in items)
		{
			hasMovingItems = hasMovingItems || !item.Frame.IsCurrentDesired;

			if (item.Frame.IsCurrentDesired)
			{
				OnDrawStaticItem(canvas, item, item.Frame.Current);
			}
		}

		OnAfterDrawStaticItems(canvas, frame);

		OnBeforeDrawMovingItems(canvas, frame);

		// draw moving items on top
		if (hasMovingItems)
		{
			foreach (var item in items)
			{
				if (!item.Frame.IsCurrentDesired)
				{
					OnDrawMovingItem(canvas, item, item.Frame.Current);
				}
			}
		}

		OnAfterDrawMovingItems(canvas, frame);

		OnAfterDrawItems(canvas, frame);
	}

	// background

	protected virtual void OnBeforeDraw(TCanvas canvas, RectangleF frame) { }

	protected virtual void OnDrawBackground(TCanvas canvas, RectangleF frame) { }

	// debug 

	protected virtual void OnBeforeDrawDebugItems(TCanvas canvas, RectangleF frame) { }

	protected virtual void OnDrawDebugItem(TCanvas canvas, PivotVisualizationItem item, RectangleF itemFrame) { }

	protected virtual void OnAfterDrawDebugItems(TCanvas canvas, RectangleF frame) { }

	// items

	protected virtual void OnBeforeDrawItems(TCanvas canvas, RectangleF frame) { }

	protected virtual void OnDrawItem(TCanvas canvas, PivotVisualizationItem item, RectangleF itemFrame) { }

	protected virtual void OnAfterDrawItems(TCanvas canvas, RectangleF frame) { }

	// static items

	protected virtual void OnBeforeDrawStaticItems(TCanvas canvas, RectangleF frame) { }

	protected virtual void OnDrawStaticItem(TCanvas canvas, PivotVisualizationItem item, RectangleF itemFrame) => OnDrawItem(canvas, item, itemFrame);

	protected virtual void OnAfterDrawStaticItems(TCanvas canvas, RectangleF frame) { }

	// moving items

	protected virtual void OnBeforeDrawMovingItems(TCanvas canvas, RectangleF frame) { }

	protected virtual void OnDrawMovingItem(TCanvas canvas, PivotVisualizationItem item, RectangleF itemFrame) => OnDrawItem(canvas, item, itemFrame);

	protected virtual void OnAfterDrawMovingItems(TCanvas canvas, RectangleF frame) { }

	// overlay

	protected virtual void OnDrawForeground(TCanvas canvas, RectangleF frame) { }

	protected virtual void OnAfterDraw(TCanvas canvas, RectangleF frame) { }
}
