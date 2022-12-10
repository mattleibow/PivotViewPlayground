using System.Drawing;

namespace PivotView.Core.Tests;

class PlaceholderItem
{
    public PlaceholderItem(string name)
    {
        Name = name;
        Frame.Current = RectangleF.Empty;
    }

    public PlaceholderItem(string name, float x, float y, float w, float h)
    {
        Name = name;
        Frame.Current = new RectangleF(x, y, w, h);
    }

    public string Name { get; }

    public AnimatableProperty<RectangleF> Frame { get; } = new(default);
}
