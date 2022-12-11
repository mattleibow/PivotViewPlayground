using PivotView.Core.Rendering;
using PivotView.Core.VisualizerApp.Visualizers;
using System.Collections.ObjectModel;

namespace PivotView.Core.VisualizerApp;

public partial class LayoutPage : ContentPage
{
    private LayoutVisualizer? current;
    private string? itemsText;

    public LayoutPage()
    {
        InitializeComponent();

        Visualizers =
            new()
            {
                new VerticalStackLayoutVisualizer(Items)
            };

        Current = Visualizers.FirstOrDefault();

        BindingContext = this;
    }

    public ObservableCollection<PivotRendererItem> Items { get; } =
        new()
        {
            NewItem("Item 1"),
            NewItem("Item 2"),
            NewItem("Item 3"),
            NewItem("Item 4"),
        };

    public ObservableCollection<LayoutVisualizer> Visualizers { get; }

    public LayoutVisualizer? Current
    {
        get => current;
        set
        {
            current = value;
            OnPropertyChanged();
        }
    }

    public string? ItemsText
    {
        get => itemsText;
        set
        {
            itemsText = value ?? string.Empty;
            var newItemsText = itemsText.Split(new[] { '\r', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var newLen = newItemsText.Length;

            while (Items.Count > newLen)
                Items.RemoveAt(Items.Count - 1);

            while (Items.Count < newLen)
                Items.Add(NewItem("X"));

            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i].DataItem;

                if (SizeF.TryParse(newItemsText[i], out var newSize))
                {
                    item.ImageWidth = newSize.Width;
                    item.ImageHeight = newSize.Height;
                }
            }

            Current?.InvalidateLayout();
            Current?.InvalidateDrawing();
        }
    }

    private static PivotRendererItem NewItem(string name) =>
        new(new() { Name = name });
}