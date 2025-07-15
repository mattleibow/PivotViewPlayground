namespace Pivot.Controls;

public class PivotViewerProperty
{
	private BindableProperty? bindableProperty;
	private Binding? binding;

	public string? Name { get; set; }

	public BindingBase? Binding
	{
		get => binding;
		set => binding = (Binding?)value;
	}

	internal Binding ActualBinding =>
		binding ?? throw new InvalidOperationException("Cannot reading binding if no binding is set.");

	internal BindableProperty BindableProperty =>
		bindableProperty ??= BindableProperty.Create(ActualBinding.Path, typeof(object), typeof(PivotViewerPivotDataItemBindingProxy), null);
}
