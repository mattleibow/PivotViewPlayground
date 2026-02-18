using System.Drawing;
using Pivot.Layout;
using Pivot.Layout.Transitions;

namespace PivotVisualizerApp.Visualizers.Layout;

public class OutOfFrameLayoutVisualizer : LayoutVisualizer<OutOfFrameLayoutVisualizer.Wrapper>
{
	public OutOfFrameLayoutVisualizer(bool isAdding, ObservableCollection<PivotVisualizationItem> items)
		: base($"Out Of Frame ({(isAdding ? "Adding" : "Removing")})", new Wrapper(isAdding), items)
	{
	}

	public class Wrapper : GridLayout
	{
		private readonly ExplosionLayoutTransition offscreen = new();
		private readonly bool isAdding;

		public Wrapper(bool isAdding)
		{
			this.isAdding = isAdding;
		}

		protected override void OnArrangeItems(IReadOnlyList<PivotVisualizationItem> items, RectangleF frame)
		{
			var offscreenItems = items.Where((item, idx) => idx % 2 != 0).ToArray();

			// layout items as if they were all on screen
			base.OnArrangeItems(items, frame);
			foreach (var item in items)
				item.Frame.Current = item.Frame.Desired;

			offscreen.ArrangeItems(offscreenItems, frame, isAdding ? PivotLayoutTransitionType.Enter : PivotLayoutTransitionType.Exit);
		}
	}
}
