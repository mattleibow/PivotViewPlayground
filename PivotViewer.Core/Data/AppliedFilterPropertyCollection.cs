﻿using System.Diagnostics;

namespace PivotViewer.Core.Data;

[DebuggerDisplay("{DebuggerValue}")]
public class AppliedFilterPropertyCollection : FilterPropertyCollection
{
	internal string DebuggerValue =>
		string.Join(";", this.Select(p => $"{p.Name}={string.Join(",", p.Values.DebuggerValue)}"));

	public event EventHandler? FilterChanged;

	public void ApplyValue(FilterValue filterValue) =>
		ApplyOrIncrementValue(filterValue, 1);

	public void UnapplyValue(FilterValue filterValue) =>
		ApplyOrIncrementValue(filterValue, -1);

	public void ApplyOrIncrementValue(FilterValue filterValue, int change = 1) =>
		ApplyOrIncrementValue(filterValue.Property.PivotProperty, filterValue.Value, change);

	private void ApplyOrIncrementValue(PivotProperty property, IComparable value, int change = 1)
	{
		if (TryGet(property.Name, out var applied))
		{
			applied.IncrementValue(value, change);
			if (applied.Count == 0)
				Remove(applied);
		}
		else
		{
			applied = new FilterProperty(property);
			applied.IncrementValue(value, change);
			Add(applied);
		}

		FilterChanged?.Invoke(this, EventArgs.Empty);
	}
}
