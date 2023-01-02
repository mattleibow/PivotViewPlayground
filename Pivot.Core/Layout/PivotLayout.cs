namespace Pivot.Core.Layout;

abstract public class PivotLayout
{
	private RectangleF measureLastSize;
	private bool measureDirty;
	private RectangleF arrangeLastSize;
	private bool arrangeDirty;

	private int itemCountOverride;
	private float itemAspectRatioOverride;
	private float itemMargin;

	public virtual int ItemCountOverride
	{
		get => itemCountOverride;
		set
		{
			itemCountOverride = value;
			Invalidate();
		}
	}

	public virtual float ItemAspectRatioOverride
	{
		get => itemAspectRatioOverride;
		set
		{
			itemAspectRatioOverride = value;
			Invalidate();
		}
	}

	public virtual float ItemMargin
	{
		get => itemMargin;
		set
		{
			itemMargin = value;
			Invalidate();
		}
	}

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

	public void MeasureItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
	{
		if (measureLastSize == frame && !measureDirty)
			return;

		measureLastSize = frame;
		measureDirty = false;

		OnMeasureItems(items, frame);
	}

	public void ArrangeItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
	{
		if (arrangeLastSize == frame && !arrangeDirty)
			return;

		arrangeLastSize = frame;
		arrangeDirty = false;

		OnArrangeItems(items, frame);
	}

	public void Invalidate()
	{
		arrangeDirty = true;
		measureDirty = true;
	}

	protected abstract void OnMeasureItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame);

	protected abstract void OnArrangeItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame);

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
