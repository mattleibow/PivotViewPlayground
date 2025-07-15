using Pivot.Data.Model;

namespace Pivot.Controls;

/// <summary>
/// This type wraps arbitrary properties from a <see cref="PivotViewer"/> into
/// a <see cref="PivotProperty"/> for use in filtering and rendering.
/// </summary>
internal class PivotViewerPivotProperty : PivotProperty
{
	public PivotViewerPivotProperty(PivotViewerProperty property)
		: base(property.Name)
	{
		PivotViewerProperty = property;
	}

	public PivotViewerProperty PivotViewerProperty { get; }
}
