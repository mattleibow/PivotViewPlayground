using System.Diagnostics;
using System.Drawing;

namespace PivotView.Core.Tests;

class OldThings
{
    private static PivotDataSource CreateSingleItemDataSource(float itemWidth, float itemHeight) =>
        new()
        {
            Items = new PivotDataItem[]
            {
                new PivotDataItem
                {
                    Name = "Ginger Cat",
                    ImageWidth = itemWidth,
                    ImageHeight = itemHeight,
                }
            }
        };

    //[Theory]
    //[InlineData(100, 100, 10, 10, 100, 100)]
    //[InlineData(100, 100, 100, 10, 100, 10)]
    //[InlineData(100, 100, 10, 100, 10, 100)]
    public void ImmediateUpdate(float viewW, float viewH, float itemW, float itemH, float expectedW, float expectedH)
    {
        // user supplied data
        var data = CreateSingleItemDataSource(itemW, itemH);

        // user selected UI "view" (grid, bar, map)
        var layout = new PivotGridLayout();


        // UI control container
        var ticker = new TestTicker();
        var renderer = new TestPivotRenderer(ticker);
        renderer.DataSource = data;
        renderer.Layout = layout;


        // trigger a "layout" by updating the viewport
        renderer.UpdateViewport(new RectangleF(0, 0, viewW, viewH));

        // assert
        var item = Assert.Single(renderer.Items);
        Assert.Equal(new RectangleF(0, 0, expectedW, expectedH), item.Frame);
    }

    //[Fact]
    public void Anim()
    {
        var value = new AnimatableValue<int>(0);

        value.AddStep(100, TimeSpan.FromSeconds(1));
    }

    class PivotRendererItem : Animatable
    {
        public PivotRendererItem(PivotDataItem dataItem)
        {
            DataItem = dataItem;
            //Frame = new AnimatableValue<RectangleF>(default);
        }

        public RectangleF Frame { get; set; }

        public PivotDataItem DataItem { get; }
    }

    class PivotRenderer
    {
        private readonly List<PivotRendererItem> items = new List<PivotRendererItem>();
        private PivotDataSource? dataSource;
        private Ticker ticker;

        public PivotRenderer(Ticker ticker)
        {
            this.ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        }

        public RectangleF Viewport { get; private set; }

        public PivotLayout? Layout { get; set; }

        public PivotDataSource? DataSource
        {
            get => dataSource;
            set
            {
                dataSource = value;
                UpdateRendererItems();
            }
        }

        protected IList<PivotRendererItem> Items => items;

        public void UpdateFilter()
        {
            // TODO: update "current" items

            // 1. Reset zoom, deselect, stop all other animations
            // 2. Hide UI, run filters, calculate new locations of existing/old (filtered items out of view)
            // 3. Relayout current items
            // 4. Calculate locations of new items
        }

        public void UpdateViewport(RectangleF newViewport)
        {
            // TODO: relayout based on current things (simpler filter)

            var allItems = items;

            var visibleItems = items; // TODO: actually filter

            // 1. calculate locations of new items
            var normalizedHeight = GetAverageNormalizedHeight(items);
            var columnCount = GetColumnCount(visibleItems.Count, newViewport.Size, normalizedHeight);
            var itemWidth = newViewport.Width / columnCount;

            foreach (var item in visibleItems)
            {
                var norm = item.DataItem.ImageHeight / item.DataItem.ImageWidth;
                item.Frame = new RectangleF(0, 0, itemWidth, itemWidth * norm);
            }

            // 2. animate



            Viewport = newViewport;

            //Layout?.LayoutItems(items, newViewport);
        }

        private static float GetColumnCount(int count, SizeF viewport, float normalizedHeight)
        {
            var normH = normalizedHeight;
            var viewW = viewport.Width;
            var viewH = viewport.Height;

            var result = 1f;

            while (MathF.Ceiling(count / result) * viewW / result * normH > viewH)
            {
                result++;
            }

            return result;
        }

        private static float GetAverageNormalizedHeight(IList<PivotRendererItem> items)
        {
            if (items is null || items.Count == 0)
                return 1f;

            // calculate some average
            var sum = 0f;
            foreach (var item in items)
            {
                var norm = item.DataItem.ImageHeight / item.DataItem.ImageWidth;
                sum += norm;
            }
            var avg = sum / items.Count;

            // TODO: add padding

            return avg;
        }

        private void UpdateRendererItems()
        {
            items.Clear();

            var dataItems = DataSource?.Items;
            if (dataItems is null)
                return;

            var count = dataItems.Count;
            if (count == 0)
                return;

            items.EnsureCapacity(count);

            for (var idx = 0; idx < count; idx++)
            {
                items.Add(new PivotRendererItem(dataItems[idx]));
            }
        }
    }

    class TestPivotRenderer : PivotRenderer
    {
        public TestPivotRenderer(Ticker ticker)
            : base(ticker)
        {
        }

        public new IList<PivotRendererItem> Items => base.Items;
    }

    class PivotDataSource
    {
        private IList<PivotDataItem>? items;

        public IList<PivotDataItem>? Items
        {
            get => items;
            set
            {
                items = value;
                OnItemsChanged();
            }
        }

        private void OnItemsChanged()
        {
            if (items is null || items.Count == 0)
                return;


        }
    }

    [DebuggerDisplay("{Name}")]
    class PivotDataItem
    {
        public string? Name { get; set; }

        public float ImageWidth { get; set; } = 0;

        public float ImageHeight { get; set; } = 0;

        public IList<PivotDataProperty>? Properties { get; set; } = new List<PivotDataProperty>();
    }

    class PivotDataProperty
    {
        public PivotDataProperty(string name, object value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public object Value { get; }
    }

    class Animatable
    {
    }

    class AnimatableValue<T>
    {
        private readonly LinkedList<AnimatableValueStep<T>> steps = new();

        private AnimatableValueStep<T>? currentStep;
        private TimeSpan currentDuration = TimeSpan.Zero;

        public AnimatableValue(T current)
        {
            Current = current;
        }

        public T? Current { get; private set; }

        /// <summary>
        /// Move the animation forward by the provided delta value.
        /// </summary>
        /// <param name="delta">The delta value of the update tick.</param>
        public void Update(TimeSpan delta)
        {
            if (delta < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(delta), "The animation cannot go backwards. Ensure the delta value is positive.");

            // get the corrent or next step depending
            if (currentStep is null && steps.First is not null)
            {
                currentStep = steps.First.Value;
                steps.RemoveFirst();
            }

            // if there is nothing to do, bail out
            if (currentStep is null)
                return;

            // TODO: something...

            currentDuration += delta;
        }

        /// <summary>
        /// Reset all animation and set the current state to the final end value.
        /// </summary>
        public void Reset()
        {
            if (steps.Last is not null)
                Current = steps.Last.Value.End;

            steps.Clear();

            currentStep = null;
            currentDuration = TimeSpan.Zero;
        }

        /// <summary>
        /// Append a new step to the animation chain.
        /// </summary>
        /// <param name="value">The new value of the final step in the animation.</param>
        /// <param name="duration">The duration of the animation for this step.</param>
        public void AddStep(T value, TimeSpan duration)
        {
            var start = Current;
            if (steps.Last is not null)
                start = steps.Last.Value.End;

            steps.AddLast(new AnimatableValueStep<T>(start, value, duration));
        }
    }

    class Animator
    {
        private readonly Ticker ticker;

        public Animator(Ticker ticker)
        {
        }

        public void Tick()
        {
        }
    }

    class Ticker
    {
        public Action<TimeSpan>? Tick { get; set; }
    }

    class TestTicker : Ticker
    {
        public void PerformTick(TimeSpan delta)
        {
            Tick?.Invoke(delta);
        }
    }

    public record struct AnimatableValueStep<T>(T? Start, T? End, TimeSpan Duration)
    {
        public static implicit operator (T? Start, T? End, TimeSpan Duration)(AnimatableValueStep<T?> value) =>
            (value.Start, value.End, value.Duration);

        public static implicit operator AnimatableValueStep<T?>((T? Start, T? End, TimeSpan Duration) value) =>
            new AnimatableValueStep<T?>(value.Start, value.End, value.Duration);
    }


    //    // var zoomLevel = log(max(visibleImageWidth/visibleImageHeight)) / log(2)

    //void Test2()
    //{
    //    var renderer = new PivotRenderer();
    //    renderer.Frame = new RectangleF(0, 0, 100, 100);

    //    var data = new PivotDataSource();
    //    data.Items = new PivotDataItem[] {
    //        new PivotDataItem {
    //            Name = "Ginger Cat",
    //            Properties = new PivotDataProperty[]
    //            {
    //                new PivotDataProperty("Animal", true),
    //                new PivotDataProperty("Type", "Cat"),
    //            }
    //        },
    //        new PivotDataItem {
    //            Name = "Black Dog",
    //            Properties = new PivotDataProperty[]
    //            {
    //                new PivotDataProperty("Animal", true),
    //                new PivotDataProperty("Type", "Dog"),
    //            }
    //        },
    //        new PivotDataItem {
    //            Name = "Black Panther",
    //            Properties = new PivotDataProperty[]
    //            {
    //                new PivotDataProperty("Animal", true),
    //                new PivotDataProperty("Type", "Cat"),
    //            }
    //        },
    //        new PivotDataItem {
    //            Name = "Red Mug",
    //            Properties = new PivotDataProperty[]
    //            {
    //                new PivotDataProperty("Animal", false),
    //                new PivotDataProperty("Type", "Cup"),
    //            }
    //        }
    //    };

    //    //var pivotView = new PivotView();
    //    //pivotView.DataSource = data;
    //    //pivotView.Renderer = renderer;
    //}
}