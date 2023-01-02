using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PivotVisualizerApp;

class GraphicsViewEx : GraphicsView
{
	protected override void OnPropertyChanging([CallerMemberName] string? propertyName = null)
	{
		base.OnPropertyChanging(propertyName);

		if (propertyName == DrawableProperty.PropertyName)
		{
			if (Drawable is INotifyPropertyChanged inpc)
				inpc.PropertyChanged -= OnDrawingChanged;
		}
	}
	protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == DrawableProperty.PropertyName)
		{
			if (Drawable is INotifyPropertyChanged inpc)
				inpc.PropertyChanged += OnDrawingChanged;
		}
	}

	private void OnDrawingChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(IDrawable))
			Invalidate();
	}
}
