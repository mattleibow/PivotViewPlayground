namespace PivotView.Core.VisualizerApp;

[ContentProperty(nameof(Templates))]
public class DynamicTemplateSelector : DataTemplateSelector
{
	private const string DefaultTemplateKey = "Default";

	public ObservableCollection<DynamicDataTemplate> Templates { get; } = new();

	public BindingBase? ItemKeyBinding { get; set; }

	protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
	{
		var name = GetItemKey(item);

		DynamicDataTemplate? def = null;

		foreach (var template in Templates)
		{
			if (template.Key == name)
				return template;

			if (template.Key is null || template.Key == DefaultTemplateKey)
				def = template;
		}

		return def ?? throw new InvalidOperationException("Missing the 'Default' data template.");
	}

	private string? GetItemKey(object? item)
	{
		if (ItemKeyBinding is null)
			return item?.ToString();

		var obj = new KeyObject();
		obj.SetBinding(KeyObject.KeyProperty, ItemKeyBinding);
		obj.BindingContext = item;

		var key = (string?)obj.GetValue(KeyObject.KeyProperty);

		obj.RemoveBinding(KeyObject.KeyProperty);
		obj.BindingContext = null;

		return key;
	}

	class KeyObject : BindableObject
	{
		public static readonly BindableProperty KeyProperty =
			BindableProperty.Create("Key", typeof(string), typeof(DynamicTemplateSelector), default(string));
	}
}

[ContentProperty(nameof(Template))]
public class DynamicDataTemplate : BindableObject
{
	public string? Key { get; set; }

	public DataTemplate? Template { get; set; }

	public static implicit operator DataTemplate(DynamicDataTemplate dynamicTemplate) =>
		dynamicTemplate?.Template ?? throw new NullReferenceException("No Template was specified.");
}
