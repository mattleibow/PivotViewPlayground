using Pivot.Data.Model;

namespace Pivot.Controls;

internal class PivotViewerPivotProperty : PivotProperty
{
	public PivotViewerPivotProperty(PivotViewerProperty property)
		: base(property.Name)
	{
		PivotViewerProperty = property;
	}

	public PivotViewerProperty PivotViewerProperty { get; }
}
