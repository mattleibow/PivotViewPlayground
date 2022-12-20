namespace PivotViewer.Core.Rendering;

public class PivotRenderer
{
	private readonly BufferedDelegate bufferedUpdate = new();

	private readonly List<PivotRendererItem> allItems = new();
	private readonly List<PivotRendererItem> currentItems = new();
	private readonly List<PivotRendererItem> visibleItems = new();

	private Func<string, bool>? filter;
	private PivotDataSource? dataSource;
	private PivotLayout? layout = new GridLayout();
	private RectangleF frame;
	private AnimationSet? animation;

	public PivotRenderer()
	{
	}

	// TODO: Filter
	// TODO: SortOrder

	public IAnimationSet? Animation => animation;

	public Func<string, bool>? Filter
	{
		get => filter;
		set
		{
			//if (filter == value)
			//    return;

			filter = value;

			currentItems.Clear();
			currentItems.AddRange(GetFilteredItems());

			UpdateVisibleItems();

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
				// update all items
				foreach (var item in dataSource.Items)
				{
					allItems.Add(new PivotRendererItem(item));
				}

				// TODO: sorting of all items

				var filteredItems = GetFilteredItems();
				currentItems.AddRange(filteredItems);
			}

			UpdateVisibleItems();

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

			UpdateVisibleItems();
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

			UpdateVisibleItems();
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

	public IReadOnlyList<PivotRendererItem> Items => allItems;

	public IReadOnlyList<PivotRendererItem> CurrentItems => currentItems;

	public IReadOnlyList<PivotRendererItem> VisibleItems => visibleItems;

	public event EventHandler? ItemsChanged;

	public void ResetLayout()
	{
		// stop all animations
		bufferedUpdate.Reset();
		animation = null;

		// update current items
		var filteredItems = GetFilteredItems();
		currentItems.Clear();
		currentItems.AddRange(filteredItems);
		visibleItems.Clear();
		visibleItems.AddRange(filteredItems);

		// layout items
		Layout?.LayoutItems(filteredItems, Frame);

		// immediately apply layout
		foreach (var item in filteredItems)
			item.Frame.Current = item.Frame.Desired;
	}

	private PivotRendererItem[] GetFilteredItems() =>
		Filter is null
		   ? allItems.ToArray()
		   : allItems.Where(i => Filter(i.Id)).ToArray();

	private void UpdateVisibleItems()
	{
		bufferedUpdate.Post(LayoutAnimationDelay, UpdateVisibleItemsImmediate);
	}

	private void UpdateVisibleItemsImmediate()
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

	private IEnumerable<IAnimationStep> GetAnimationSteps(PivotRendererItem[] removed, PivotRendererItem[] remaining, PivotRendererItem[] added)
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
