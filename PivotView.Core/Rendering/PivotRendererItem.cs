using System.Diagnostics;

namespace PivotView.Core.Rendering;

[DebuggerDisplay("{Id}")]
public class PivotRendererItem
{
	public PivotRendererItem(PivotDataItem dataItem)
	{
		DataItem = dataItem;
	}

	public string? Id => DataItem.Id;

	public PivotDataItem DataItem { get; }

	/// <summary>
	/// Width / Height
	/// </summary>
	public float AspectRatio =>
		DataItem.ImageWidth == 0 || DataItem.ImageHeight == 0
			? 1.0f
			: (float)DataItem.ImageWidth / DataItem.ImageHeight;

	public AnimatableProperty<RectangleF> Frame { get; } = new(default);
}
