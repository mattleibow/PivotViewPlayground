namespace PivotVisualizerApp;

[ContentProperty(nameof(Template))]
public class DynamicDataTemplate : BindableObject
{
	public string? Key { get; set; }

	public DataTemplate? Template { get; set; }

	public static implicit operator DataTemplate(DynamicDataTemplate dynamicTemplate) =>
		dynamicTemplate?.Template ?? throw new NullReferenceException("No Template was specified.");
}
