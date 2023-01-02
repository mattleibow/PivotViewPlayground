namespace Pivot.Core.Tests;

public partial class PivotLayoutUnitTests
{
	[Fact]
	public void LayoutsWithSameFrameFiresOnce()
	{
		var items = CreateItemList("A");
		var frame = new RectangleF(0, 0, 100, 100);

		var layout = new TestPivotLayout();

		// nothing happened (sanity check)
		Assert.Equal(0, layout.OnMeasureItemsCount);
		Assert.Equal(0, layout.OnArrangeItemsCount);

		layout.LayoutItems(items, frame);

		// layout triggered an arrange
		Assert.Equal(1, layout.OnMeasureItemsCount);
		Assert.Equal(1, layout.OnArrangeItemsCount);

		layout.LayoutItems(items, frame);

		// layout was a no-op
		Assert.Equal(1, layout.OnMeasureItemsCount);
		Assert.Equal(1, layout.OnArrangeItemsCount);
	}

	[Fact]
	public void LayoutsWithSameFrameFiresAgainAfterInvalidate()
	{
		var items = CreateItemList("A");
		var frame = new RectangleF(0, 0, 100, 100);

		var layout = new TestPivotLayout();

		layout.LayoutItems(items, frame);
		layout.Invalidate();
		layout.LayoutItems(items, frame);

		// layout happened twice
		Assert.Equal(2, layout.OnMeasureItemsCount);
		Assert.Equal(2, layout.OnArrangeItemsCount);

		layout.LayoutItems(items, frame);

		// even after invalidating, nothing happens again
		Assert.Equal(2, layout.OnMeasureItemsCount);
		Assert.Equal(2, layout.OnArrangeItemsCount);
	}

	[Fact]
	public void LayoutsWithDifferentFrameFiresAgain()
	{
		var items = CreateItemList("A");
		var frame1 = new RectangleF(0, 0, 100, 100);
		var frame2 = new RectangleF(0, 0, 120, 120);

		var layout = new TestPivotLayout();

		layout.LayoutItems(items, frame1);
		layout.LayoutItems(items, frame2);

		// layout happened twice
		Assert.Equal(2, layout.OnMeasureItemsCount);
		Assert.Equal(2, layout.OnArrangeItemsCount);

		layout.LayoutItems(items, frame2);

		// nothing happened
		Assert.Equal(2, layout.OnMeasureItemsCount);
		Assert.Equal(2, layout.OnArrangeItemsCount);
	}

	private static List<PivotRendererItem> CreateItemList(params string[] items) =>
		items.Select(i => new PivotRendererItem(CreateDataItem(i))).ToList();

	private static PivotDataItem CreateDataItem(string name, float width = 100, float height = 100) =>
		new()
		{
			Id = name,
			ImageWidth = width,
			ImageHeight = height,
		};

	class TestPivotLayout : PivotLayout
	{
		public int OnMeasureItemsCount { get; private set; }

		public int OnArrangeItemsCount { get; private set; }

		protected override void OnMeasureItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame) =>
			OnMeasureItemsCount++;

		protected override void OnArrangeItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame) =>
			OnArrangeItemsCount++;

		public void ResetCounts() =>
			OnMeasureItemsCount = OnArrangeItemsCount = 0;
	}
}
