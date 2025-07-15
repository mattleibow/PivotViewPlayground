using Pivot.Animation.Steps;
using Pivot.Data.Model;
using Pivot.Data.Sources;
using Pivot.Layout;
using Pivot.Layout.Transitions;

namespace Pivot.Rendering;

/// <summary>
/// This class is responsible for updating all the positions of the items
/// in the data source based on the current layout and filters.
/// </summary>
public class PivotVisualizationController : IPivotVisualizationSource
{
	private readonly BufferedDelegate bufferedUpdate = new();

	private readonly List<PivotVisualizationItem> allItems = new();
	private readonly Dictionary<PivotDataItem, PivotVisualizationItem> dataItemToRendererItem = new();
	private readonly List<PivotVisualizationItem> currentItems = new();
	private readonly List<PivotVisualizationItem> visibleItems = new();

	private IReadOnlyCollection<PivotDataItem>? filter;

	private PivotDataSource? dataSource;
	private PivotLayout? layout = new GridLayout();
	private RectangleF frame;
	private AnimationSet? animation;

	public PivotVisualizationController()
	{
	}

	IReadOnlyList<PivotVisualizationItem> IPivotVisualizationSource.Items => VisibleItems;

	RectangleF IPivotVisualizationSource.RenderFrame
	{
		get => Frame;
		set => Frame = value;
	}

	// TODO: SortOrder

	public IAnimationSet? Animation => animation;

	public IReadOnlyCollection<PivotDataItem>? Filter
	{
		get => filter;
		set
		{
			filter = value;

			ResetCurrentItemsList();

			InvalidateVisibleItems();

			ItemsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public PivotDataSource? DataSource
	{
		get => dataSource;
		set
		{
			if (dataSource == value)
				return;

			dataSource = value;

			allItems.Clear();
			currentItems.Clear();
			visibleItems.Clear();

			if (dataSource?.Items is not null)
			{
				allItems.EnsureCapacity(dataSource.Items.Count);
				currentItems.EnsureCapacity(dataSource.Items.Count);
				visibleItems.EnsureCapacity(dataSource.Items.Count);

				ResetAllItemsList();
				ResetCurrentItemsList();

				// TODO: sorting of all items
			}

			InvalidateVisibleItems();

			ItemsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public PivotLayout? Layout
	{
		get => layout;
		set
		{
			if (layout == value)
				return;

			layout = value;

			InvalidateVisibleItems();
		}
	}

	public PivotLayoutTransition? LayoutTransition { get; set; } = new ExplosionLayoutTransition();

	public RectangleF Frame
	{
		get => frame;
		set
		{
			if (frame == value)
				return;

			frame = value;

			InvalidateVisibleItems();
		}
	}

	public TimeSpan LayoutAnimationDelay { get; set; } = TimeSpan.FromSeconds(0.4);

	public TimeSpan MinimumAnimationDelay { get; set; } = TimeSpan.FromSeconds(0.0);

	public TimeSpan MaximumAnimationDelay { get; set; } = TimeSpan.FromSeconds(0.2);

	public TimeSpan RemoveItemsAnimationDuration { get; set; } = TimeSpan.FromSeconds(0.5);

	public TimeSpan MoveItemsAnimationDuration { get; set; } = TimeSpan.FromSeconds(0.5);

	public TimeSpan AddItemsAnimationDuration { get; set; } = TimeSpan.FromSeconds(0.5);

	public EasingDelegate AddItemsAnimationEasing { get; set; } = Easing.CubicInOut;

	public EasingDelegate MoveItemsAnimationEasing { get; set; } = Easing.CubicInOut;

	public EasingDelegate RemoveItemsAnimationEasing { get; set; } = Easing.CubicInOut;

	public IReadOnlyList<PivotVisualizationItem> Items => allItems;

	public IReadOnlyList<PivotVisualizationItem> CurrentItems => currentItems;

	public IReadOnlyList<PivotVisualizationItem> VisibleItems => visibleItems;

	public event EventHandler? ItemsChanged;

	public void ResetLayout()
	{
		// stop all animations
		bufferedUpdate.Reset();
		animation = null;

		// update current items
		visibleItems.Clear();
		visibleItems.AddRange(currentItems);

		// layout items
		Layout?.LayoutItems(currentItems, Frame);

		// immediately apply layout
		foreach (var item in currentItems)
			item.Frame.Current = item.Frame.Desired;
	}

	private void ResetAllItemsList()
	{
		allItems.Clear();

		// bail out if there are not items at all
		if (dataSource?.Items is not { } items)
			return;

		// create renderer items for all data items
		foreach (var item in items)
		{
			var rendererItem = new PivotVisualizationItem(item);
			allItems.Add(rendererItem);
			dataItemToRendererItem[item] = rendererItem;
		}
	}

	private void ResetCurrentItemsList()
	{
		currentItems.Clear();

		// bail out if there are not items at all
		if (allItems.Count <= 0)
			return;

		// if the filter is null or empty, then show all items
		if (filter is null || filter.Count == 0)
		{
			currentItems.AddRange(allItems);
			return;
		}

		// otherwise, show only the items that match the filter
		currentItems.AddRange(GetFilteredItems());

		IEnumerable<PivotVisualizationItem> GetFilteredItems()
		{
			foreach (var item in filter)
			{
				if (dataItemToRendererItem.TryGetValue(item, out var rendererItem))
					yield return rendererItem;
			}
		}
	}

	private void InvalidateVisibleItems()
	{
		bufferedUpdate.Post(LayoutAnimationDelay, InvalidateVisibleItemsImmediate);
	}

	private void InvalidateVisibleItemsImmediate()
	{
		animation = null;

		// skip the layout if there are no items or no place to put them
		if (allItems.Count == 0 || Frame.IsEmpty)
			return;

		// Filter

		// calculate old/new items
		var removedItems = visibleItems.Except(currentItems).ToArray();
		var remainingItems = visibleItems.Except(removedItems).ToArray();
		var addedItems = currentItems.Except(visibleItems).ToArray();

		// Layout

		// invalidate whatever we have now
		Layout?.Invalidate();
		// calculate all the final positions
		Layout?.LayoutItems(currentItems, Frame);
		// calculate new frames
		LayoutTransition?.ArrangeItems(addedItems, Frame, PivotLayoutTransitionType.Enter);
		// calculate old frames
		LayoutTransition?.ArrangeItems(removedItems, Frame, PivotLayoutTransitionType.Exit);

		// Animation

		animation = new AnimationSet();

		foreach (var step in GetAnimationSteps(removedItems, remainingItems, addedItems))
			animation.Add(step);
	}

	private IEnumerable<IAnimationStep> GetAnimationSteps(PivotVisualizationItem[] removed, PivotVisualizationItem[] remaining, PivotVisualizationItem[] added)
	{
		// add step 1 - remove old items
		if (removed.Length > 0)
		{
			// 1.1 animate out
			var step1 = new PropertyAnimationStep(RemoveItemsAnimationDuration, RemoveItemsAnimationEasing, MinimumAnimationDelay, MaximumAnimationDelay)
			{
				Name = "Exit",
			};
			foreach (var item in removed)
			{
				step1.Add(item.Frame);
			}
			yield return step1;

			// 1.2 remove from visible items
			var step1end = new ActionAnimationStep
			{
				Name = "Hide",
				Action = () => visibleItems.RemoveAll(i => removed.Contains(i)),
			};
			yield return step1end;
		}

		// add step 2 - re-layout remaining items
		if (remaining.Length > 0)
		{
			// 2.1 rearrange
			var step2 = new PropertyAnimationStep(MoveItemsAnimationDuration, MoveItemsAnimationEasing, MinimumAnimationDelay, MaximumAnimationDelay)
			{
				Name = "Layout"
			};
			foreach (var item in remaining)
			{
				step2.Add(item.Frame);
			}
			yield return step2;
		}

		// 2.2 add new items to visible items
		if (added.Length > 0)
		{
			var step2end = new ActionAnimationStep
			{
				Name = "Add",
				Action = () => visibleItems.AddRange(added),
			};
			yield return step2end;
		}

		// add step 3 - add new items
		if (added.Length > 0)
		{
			// 3.1 animate new items in
			var step3 = new PropertyAnimationStep(AddItemsAnimationDuration, AddItemsAnimationEasing, MinimumAnimationDelay, MaximumAnimationDelay)
			{
				Name = "Enter"
			};
			foreach (var item in added)
			{
				step3.Add(item.Frame);
			}

			yield return step3;
		}
	}
}
