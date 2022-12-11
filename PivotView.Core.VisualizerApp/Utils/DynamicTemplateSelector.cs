using System.Collections.ObjectModel;

namespace PivotView.Core.VisualizerApp;

[ContentProperty(nameof(Templates))]
public class DynamicTemplateSelector : DataTemplateSelector
{
    private const string DefaultTemplateKey = "Default";

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var name = item?.ToString();

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

    public ObservableCollection<DynamicDataTemplate> Templates { get; } = new();
}

[ContentProperty(nameof(Template))]
public class DynamicDataTemplate : BindableObject
{
    public string? Key { get; set; }

    public DataTemplate? Template { get; set; }

    public static implicit operator DataTemplate(DynamicDataTemplate dynamicTemplate) =>
        dynamicTemplate?.Template ?? throw new NullReferenceException("No Template was specified.");
}
