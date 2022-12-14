using System.Diagnostics;

namespace Pivot.Core.Data;

[DebuggerDisplay("{DebuggerValue}")]
public class AppliedFilterPropertyCollection : FilterPropertyCollection
{
	internal string DebuggerValue =>
		string.Join(";", this.Select(p => $"{p.Name}={string.Join(",", p.Values.DebuggerValue)}"));

	public event EventHandler<FilterChangedEventArgs>? FilterChanged;

	public override void Clear()
	{
		// TODO: this is really slow

		for (var i = Count - 1; i >= 0; i--)
		{
			UnapplyProperty(this[i].Name);
		}
	}

	public void ApplyValue(FilterValue filterValue) =>
		UpdateValue(filterValue.Property.PivotProperty, filterValue.Value, 1);

	public void ApplyValue(FilterProperty property, IComparable value) =>
		UpdateValue(property.PivotProperty, value, 1);

	public void ApplyValue(PivotProperty property, IComparable value) =>
		UpdateValue(property, value, 1);

	public void UnapplyValue(PivotProperty property, IComparable value) =>
		UnapplyValue(property.Name, value);

	public void UnapplyValue(FilterValue filterValue) =>
		UnapplyValue(filterValue.Property.Name, filterValue.Value);

	public void UnapplyValue(string propertyName, IComparable value)
	{
		if (!TryGet(propertyName, out var applied))
			return;

		applied.IncrementValue(value, -1);

		if (applied.Count == 0)
		{
			Remove(applied);

			FilterChanged?.Invoke(this, new(FilterChangedAction.RemoveProperty, applied, value));
		}
		else
		{
			FilterChanged?.Invoke(this, new(FilterChangedAction.RemoveValue, applied, value));
		}
	}

	public void UnapplyProperty(PivotProperty property) =>
		UnapplyProperty(property.Name);

	public void UnapplyProperty(string propertyName)
	{
		if (!TryGet(propertyName, out var applied))
			return;

		var args = new FilterChangedEventArgs(FilterChangedAction.RemoveProperty, applied, applied.Values.Values);

		Remove(applied);

		FilterChanged?.Invoke(this, args);
	}

	private void UpdateValue(PivotProperty property, IComparable value, int change = 1)
	{
		// no need to do anything if there is no actual change
		if (change == 0)
			return;

		FilterChangedEventArgs args;

		if (TryGet(property.Name, out var applied))
		{
			applied.IncrementValue(value, change);

			if (applied.Count == 0)
			{
				Remove(applied);

				args = new(FilterChangedAction.RemoveProperty, applied, value);

			}
			else
			{
				if (change > 0)
					args = new(FilterChangedAction.AddValue, applied, value);
				else
					args = new(FilterChangedAction.RemoveValue, applied, value);
			}
		}
		else
		{
			applied = new FilterProperty(property);
			applied.IncrementValue(value, change);
			Add(applied);

			args = new(FilterChangedAction.AddProperty, applied, value);
		}

		FilterChanged?.Invoke(this, args);
	}
}

public class FilterChangedEventArgs : EventArgs
{
	public FilterChangedEventArgs(FilterChangedAction action, FilterProperty property, IComparable value)
	{
		Action = action;
		Property = property;
		Values = new[] { value };
	}

	public FilterChangedEventArgs(FilterChangedAction action, FilterProperty property, IEnumerable<IComparable> values)
	{
		Action = action;
		Property = property;
		Values = values.ToArray();
	}

	public FilterChangedAction Action { get; }

	public FilterProperty Property { get; }

	public string PropertyName => Property.Name;

	public IList<IComparable> Values { get; }
}

public enum FilterChangedAction
{
	AddProperty,
	AddValue,
	RemoveValue,
	RemoveProperty,
}
