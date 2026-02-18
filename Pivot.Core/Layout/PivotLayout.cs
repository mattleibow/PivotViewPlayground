namespace Pivot.Layout;

/// <summary>
/// This class is responsible for laying out a collection of items in a specific way.
/// </summary>
public abstract class PivotLayout
{
	private RectangleF measureLastSize;
	private bool measureDirty;
	private RectangleF arrangeLastSize;
	private bool arrangeDirty;

	private int itemCountOverride;
	private float itemAspectRatioOverride;
	private float itemMargin;

	/// <summary>
	/// Gets or sets the number of items to use instead of the real number of items.
	/// </summary>
	/// <remarks>
	/// This can be used to have a series of layouts of the same size, but differing number of items. 
	/// However, since item size is based on the number of items that can fit into an area, this property
	/// can be used to make sure all the items are the same size - even if this particular layout
	/// has more space available. One use case would be a histogram layout where there are multiple grid
	/// layouts lext to each other.
	/// </remarks>
	public virtual int ItemCountOverride
	{
		get => itemCountOverride;
		set
		{
			itemCountOverride = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the item aspect ratio to use instead of the real aspect ratio.
	/// </summary>
	/// <remarks>
	/// See <see cref="ItemCountOverride"/> for more information.
	/// </remarks>
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
	/// Gets the aspect ratio to use for for all items (calculated as width/height).
	/// </summary>
	public float ItemAspectRatio { get; protected set; } = 1.0f;

	public float ItemWidth { get; protected set; }

	public float ItemHeight { get; protected set; }

	public float LayoutWidth { get; protected set; }

	public float LayoutHeight { get; protected set; }

	public void LayoutItems(IReadOnlyList<PivotVisualizationItem> items, RectangleF frame)
	{
		MeasureItems(items, frame);
		ArrangeItems(items, frame);
	}

	public void MeasureItems(IReadOnlyList<PivotVisualizationItem> items, RectangleF frame)
	{
		if (measureLastSize == frame && !measureDirty)
			return;

		measureLastSize = frame;
		measureDirty = false;

		OnMeasureItems(items, frame);
	}

	public void ArrangeItems(IReadOnlyList<PivotVisualizationItem> items, RectangleF frame)
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

	protected abstract void OnMeasureItems(IReadOnlyList<PivotVisualizationItem> items, RectangleF frame);

	protected abstract void OnArrangeItems(IReadOnlyList<PivotVisualizationItem> items, RectangleF frame);

	protected float GetItemAspectRatio(IReadOnlyList<PivotVisualizationItem> items)
	{
		if (ItemAspectRatioOverride != 0.0f)
			return ItemAspectRatioOverride;

		// just use the first item for now
		if (items.Count > 0)
			return items[0].AspectRatio;

		return 1.0f;
	}
}
