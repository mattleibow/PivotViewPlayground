using PivotView.Core.Layout;
using PivotView.Core.Rendering;
using System.Collections.ObjectModel;
using System.Drawing;

namespace PivotView.Core.VisualizerApp.Visualizers.Layout;

public class OutOfFrameLayoutVisualizer : LayoutVisualizer<OutOfFrameLayoutVisualizer.Wrapper>
{
    public OutOfFrameLayoutVisualizer(bool isAdding, ObservableCollection<PivotRendererItem> items)
        : base($"Out Of Frame ({(isAdding ? "Adding" : "Removing")})", new Wrapper(isAdding), items)
    {
    }

    public class Wrapper : PivotOutOfFrameLayout
    {
        private readonly PivotGridLayout gridLayout = new();

        public Wrapper(bool isAdding)
            : base(isAdding)
        {
        }

        public override double ItemMargin
        {
            get => base.ItemMargin;
            set
            {
                base.ItemMargin = value;
                gridLayout.ItemMargin = value;
            }
        }

        public override void Measure(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
        {
            var otherItems = items.Where((item, idx) => idx % 2 != 0).ToArray();

            // layout items as if they were all on screen
            gridLayout.Measure(items, frame);
            foreach (var item in items)
                item.Frame.Current = item.Frame.Desired;

            base.Measure(otherItems, frame);
        }

        public override void Arrange(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
        {
            var otherItems = items.Where((item, idx) => idx % 2 != 0).ToArray();

            // layout items as if they were all on screen
            gridLayout.Arrange(items, frame);
            foreach (var item in items)
                item.Frame.Current = item.Frame.Desired;

            base.Arrange(otherItems, frame);
        }
    }
}
