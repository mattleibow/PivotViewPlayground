namespace PivotViewer.Core.VisualizerApp.Visualizers.Layout;

public class LayoutVisualizer : ItemsVisualizer
{
	private RectF lastScreenRect;

	public LayoutVisualizer(string name, PivotLayout layout, ObservableCollection<PivotRendererItem> items)
		: base(name + " Layout", items)
	{
		Layout = layout;
		items.CollectionChanged += (s, e) => InvalidateLayout();
	}

	public PivotLayout Layout { get; }

	[Slider("Item margin", 0, 20)]
	public float ItemMargin
	{
		get => Layout.ItemMargin;
		set => Layout.ItemMargin = value;
	}

	public void InvalidateLayout()
	{
		lastScreenRect = RectF.Zero;
	}

	protected override void PrepareItems(RectF bounds)
	{
		base.PrepareItems(bounds);

		UpdateLayout(bounds);
	}

	protected virtual void UpdateLayout(RectF screenRect)
	{
		if (lastScreenRect == screenRect)
			return;
		lastScreenRect = screenRect;

		Layout?.LayoutItems(Items, screenRect.ToSystemRectangleF());
	}

	protected override void OnVisualizerPropertyValueChanged()
	{
		InvalidateLayout();

		base.OnVisualizerPropertyValueChanged();
	}
}

public class LayoutVisualizer<T> : LayoutVisualizer
	where T : PivotLayout
{
	public LayoutVisualizer(string name, T layout, ObservableCollection<PivotRendererItem> items)
		: base(name, layout, items)
	{
	}

	public new T Layout => (T)base.Layout;
}
