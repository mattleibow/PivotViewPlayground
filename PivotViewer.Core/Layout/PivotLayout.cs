namespace PivotViewer.Core.Layout;

abstract public class PivotLayout
{
	public virtual int ItemCountOverride { get; set; }

	public virtual float ItemAspectRatioOverride { get; set; }

	public virtual float ItemMargin { get; set; }

	/// <summary>
	/// Width/Height
	/// </summary>
	public float ItemAspectRatio { get; protected set; } = 1.0f;

	public float ItemWidth { get; protected set; }

	public float ItemHeight { get; protected set; }

	public float LayoutWidth { get; protected set; }

	public float LayoutHeight { get; protected set; }

	public void LayoutItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
	{
		MeasureItems(items, frame);
		ArrangeItems(items, frame);
	}

	public abstract void MeasureItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame);

	public abstract void ArrangeItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame);

	protected float GetItemAspectRatio(IReadOnlyList<PivotRendererItem> items)
	{
		if (ItemAspectRatioOverride != 0.0f)
			return ItemAspectRatioOverride;

		// just use the first item for now
		if (items.Count > 0)
			return items[0].AspectRatio;

		return 1.0f;
	}
}
