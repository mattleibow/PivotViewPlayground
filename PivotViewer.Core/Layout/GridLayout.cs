using System.Diagnostics.CodeAnalysis;

namespace PivotViewer.Core.Layout;

public class GridLayout : PivotLayout
{
	public enum LayoutOrigin
	{
		Top,
		Bottom,
	}

	public enum SearchDirection
	{
		Left,
		Right,
		Up,
		Down
	}

	private readonly Dictionary<PivotRendererItem, (int Row, int Column)> itemToPosition = new();
	private readonly Dictionary<(int Row, int Column), PivotRendererItem> positionToItem = new();

	public LayoutOrigin Origin { get; set; }

	public int Columns { get; protected set; }

	public int Rows { get; protected set; }

	public bool TryGetItem(int row, int column, [MaybeNullWhen(false)] out PivotRendererItem item) =>
		positionToItem.TryGetValue((row, column), out item);

	public bool TryGetGridPosition(PivotRendererItem item, out (int Row, int Column) position) =>
		itemToPosition.TryGetValue(item, out position);

	public PivotRendererItem? GetNextItem(PivotRendererItem item, SearchDirection direction)
	{
		if (!itemToPosition.TryGetValue(item, out var pos))
			return null;

		PivotRendererItem? next = null;
		var nextRow = pos.Row;
		var nextCol = pos.Column;

		do
		{
			switch (direction)
			{
				case SearchDirection.Left:
					nextCol--;
					if (nextCol < 0)
					{
						nextCol = Columns - 1;
						nextRow--;
					}
					break;
				case SearchDirection.Right:
					nextCol++;
					if (nextCol >= Columns)
					{
						nextCol = 0;
						nextRow++;
					}
					break;
				case SearchDirection.Up:
					nextRow--;
					if (nextRow < 0)
					{
						nextRow = Rows - 1;
						nextCol--;
					}
					break;
				case SearchDirection.Down:
					nextRow++;
					if (nextRow >= Rows)
					{
						nextRow = 0;
						nextCol++;
					}
					break;
			}
			positionToItem.TryGetValue((nextRow, nextCol), out next);
		} while (nextRow < Rows && nextRow >= 0 && nextCol < Columns && nextCol >= 0 && next == null);

		return next;
	}

	protected override void OnMeasureItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
	{
		// 1. Get the aspect ratio of the items
		ItemAspectRatio = GetItemAspectRatio(items);

		// 2.Calculate the smallest/best area to lay out the items
		var count = Math.Max(items.Count, ItemCountOverride);
		// start of with one tall columns with all the items
		var columns = 1;
		var rows = count;
		// calculate the max size of each item to fit
		var maxItemHeight = frame.Height / rows;
		var maxItemWidth = ItemAspectRatio * maxItemHeight;
		// loop while there is enough space to add one more column
		while (frame.Width - (columns + 1) * maxItemWidth >= 0.0 && columns < count)
		{
			// add another column
			columns++;
			rows = (int)Math.Ceiling((float)count / columns);
			// calculate the max size of each item to fit
			maxItemHeight = frame.Height / rows;
			maxItemWidth = ItemAspectRatio * maxItemHeight;
		}

		// 3. Calculate the general item size
		ItemHeight = frame.Height / rows;
		ItemWidth = ItemHeight * ItemAspectRatio;
		if (ItemWidth * columns > frame.Width)
		{
			ItemWidth = frame.Width / columns;
			ItemHeight = ItemWidth / ItemAspectRatio;
		}

		// 4. Calculate the number of rows/cols
		Columns = Math.Min(columns, items.Count);
		Rows = (int)Math.Ceiling((float)items.Count / Columns);

		// 5. Calculate the total items area
		LayoutWidth = ItemWidth * Columns;
		LayoutHeight = ItemHeight * Rows;
	}

	protected override void OnArrangeItems(IReadOnlyList<PivotRendererItem> items, RectangleF frame)
	{
		// reset any local state
		itemToPosition.Clear();
		positionToItem.Clear();

		// bail if the items are not actually visible
		if (ItemWidth == 0 || ItemHeight == 0)
			return;

		var count = items.Count;

		// determine the origin, direction and step values
		var yOffset = frame.Y;
		var yStep = ItemHeight;
		var xStep = ItemWidth;
		if (Origin == LayoutOrigin.Bottom)
		{
			yOffset += frame.Height - ItemHeight;
			yStep *= -1;
			xStep *= -1;
		}

		// determine item actual size
		var maxItemWidth = Math.Max(0.0f, ItemWidth - ItemMargin * 2);
		var maxItemHeight = Math.Max(0.0f, ItemHeight - ItemMargin * 2);

		// determine item list loop bounds
		int currentIndex, stepIndex, lastIndex;
		if (Origin == LayoutOrigin.Bottom)
		{
			currentIndex = items.Count - 1;
			stepIndex = -1;
			lastIndex = -1;
		}
		else
		{
			currentIndex = 0;
			stepIndex = 1;
			lastIndex = items.Count;
		}

		// build up the grid
		for (var rowIndex = 0; rowIndex < Rows; rowIndex++)
		{
			// determine initial X offset
			var xOffset = frame.X;
			if (Origin == LayoutOrigin.Bottom)
			{
				var partialCol = count % Columns;
				xOffset += rowIndex == Rows - 1 && partialCol > 0
					? ItemWidth * (partialCol - 1)
					: ItemWidth * (Columns - 1);
			}

			for (var colIndex = 0; colIndex < Columns; colIndex++)
			{
				if (currentIndex == lastIndex)
					break;

				var item = items[currentIndex];

				// determine item aspect ratio
				var itemAspect = item.AspectRatio;
				if (float.IsNaN(itemAspect))
					itemAspect = 1.0f;
				// make the item fit in the space allotted
				float width, height;
				if (itemAspect > ItemAspectRatio)
				{
					width = maxItemWidth;
					height = width / itemAspect;
				}
				else
				{
					height = maxItemHeight;
					width = height * itemAspect;
				}

				// calculate item bounds
				var newFrame = new RectangleF(
					xOffset + ItemMargin + (maxItemWidth - width) / 2.0f,
					yOffset + ItemMargin + (maxItemHeight - height) / 2.0f,
					width,
					height);

				item.Frame.Desired = newFrame;

				// step
				xOffset += xStep;
				currentIndex += stepIndex;

				// find grid position
				var itemRow = Origin == LayoutOrigin.Bottom
					? Rows - 1 - rowIndex
					: rowIndex;
				var itemCol = colIndex;
				if (Origin == LayoutOrigin.Bottom)
				{
					itemCol = Columns - 1 - colIndex;
					var partialCol = count % Columns;
					if (rowIndex == Rows - 1 && partialCol > 0)
						itemCol -= Columns - partialCol;
				}

				var gridPos = (itemRow, itemCol);
				itemToPosition[item] = gridPos;
				positionToItem[gridPos] = item;
			}

			// step
			yOffset += yStep;
		}
	}
}
