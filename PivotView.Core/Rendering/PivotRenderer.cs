namespace PivotView.Core.Rendering;

public class PivotRenderer
{
    private readonly PivotLayout outOfFrameLayoutRemoving = new PivotOutOfFrameLayout(false);
    private readonly PivotLayout outOfFrameLayoutAdding = new PivotOutOfFrameLayout(true);

    private readonly List<PivotRendererItem> allItems = new();
    private readonly List<PivotRendererItem> currentItems = new();

    private PivotDataSource? dataSource;
    private PivotLayout? layout;
    private RectangleF frame;
    private Func<string, bool>? filter;

    public PivotRenderer()
    {
    }


    // TODO: Filter
    // TODO: SortOrder

    internal AnimationSet? Animation { get; set; }

    public EasingDelegate AnimationEasing { get; set; } = Easing.Linear;

    public Func<string, bool>? Filter
    {
        get => filter;
        set
        {
            filter = value;

            BuildAnimationSet();

            // update current items
            currentItems.Clear();
            currentItems.AddRange(GetFilteredItems());
        }
    }

    public PivotDataSource? DataSource
    {
        get => dataSource;
        set
        {
            dataSource = value;

            allItems.Clear();
            currentItems.Clear();

            if (dataSource?.Items is not null)
            {
                // update all items
                foreach (var item in dataSource.Items)
                {
                    allItems.Add(new PivotRendererItem(item));
                }

                // TODO: sorting of all items

                // update current items based on filter
                currentItems.AddRange(GetFilteredItems());

                // TODO: re-layout
            }
        }
    }

    public PivotLayout? Layout
    {
        get => layout;
        set
        {
            layout = value;

            // TODO: re-layout
        }
    }

    public RectangleF Frame
    {
        get => frame;
        set
        {
            frame = value;

            // TODO: re-layout
        }
    }

    public IReadOnlyList<PivotRendererItem> CurrentItems => currentItems;

    public IReadOnlyList<PivotRendererItem> Items => allItems;

    public void ResetLayout()
    {
        Animation = null;

        Layout?.LayoutItems(CurrentItems, Frame);

        foreach (var item in CurrentItems)
            item.Frame.Current = item.Frame.Desired;
    }

    private PivotRendererItem[] GetFilteredItems() =>
        Filter is null
           ? allItems.ToArray()
           : allItems.Where(i => Filter(i.Name)).ToArray();

    private void BuildAnimationSet()
    {
        // TODO: reset animation
        Animation = null;

        if (allItems.Count == 0 || Layout is null)
            return;

        // Filter

        // calculate final items
        var finalItems = GetFilteredItems();
        // calculate old/new items
        var removedItems = currentItems.Except(finalItems).ToArray();
        var remainingItems = currentItems.Except(removedItems).ToArray();
        var addedItems = finalItems.Except(currentItems).ToArray();

        // Layout

        // calculate all the final positions
        Layout.LayoutItems(finalItems, Frame);
        // calculate new frames
        outOfFrameLayoutAdding.LayoutItems(addedItems, Frame);
        // calculate old frames
        outOfFrameLayoutRemoving.LayoutItems(removedItems, Frame);

        // Animation

        // add step 1 - remove old items
        var step1 = new AnimationStep(TimeSpan.FromSeconds(1), AnimationEasing);
        foreach (var item in removedItems)
        {
            step1.Add(item.Frame);
        }
        step1.OnComplete = () =>
        {
            currentItems.RemoveAll(i => removedItems.Contains(i));
        };

        // add step 2 - re-layout remaining items
        var step2 = new AnimationStep(TimeSpan.FromSeconds(1), AnimationEasing);
        foreach (var item in remainingItems)
        {
            step2.Add(item.Frame);
        }
        step2.OnComplete = () =>
        {
            currentItems.AddRange(addedItems);
        };

        // add step 3 - add new items
        var step3 = new AnimationStep(TimeSpan.FromSeconds(1), AnimationEasing);
        foreach (var item in addedItems)
        {
            step3.Add(item.Frame);
        }
    }
}
