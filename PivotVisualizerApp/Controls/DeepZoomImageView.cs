//using Pivot.Core.DeepZoom;

//namespace PivotVisualizerApp.Controls;

//public class DeepZoomImageView : ContentView, IDrawable
//{
//	public static readonly BindableProperty TileSourceProperty = BindableProperty.Create(
//		nameof(TileSource), typeof(DeepZoomCollectionTileSource), typeof(DeepZoomImageView), null,
//		propertyChanged: OnTileSourceChanged);

//	public static readonly BindableProperty LevelProperty = BindableProperty.Create(
//		nameof(Level), typeof(int), typeof(DeepZoomImageView), -1,
//		propertyChanged: OnTileSourceChanged);

//	private readonly GraphicsView graphicsView;
//	private readonly TileManager loader = new();

//	public DeepZoomImageView()
//	{
//		graphicsView = new GraphicsView();
//		graphicsView.Drawable = this;

//		Content = graphicsView;

//		loader.TileLoaded += OnTileLoaded;
//	}

//	public DeepZoomCollectionTileSource TileSource
//	{
//		get => (DeepZoomCollectionTileSource)GetValue(TileSourceProperty);
//		set => SetValue(TileSourceProperty, value);
//	}

//	public int Level
//	{
//		get => (int)GetValue(LevelProperty);
//		set => SetValue(LevelProperty, value);
//	}

//	public void Draw(ICanvas canvas, RectF dirtyRect)
//	{
//		if (TileSource is null)
//			return;

//		var frame = dirtyRect.ToSystemRectangleF();

//		loader.Update(frame, frame, new DeepZoomImageViewCanvas(canvas));
//	}

//	private void OnTileLoaded(object? sender, EventArgs e)
//	{
//		graphicsView.Invalidate();
//	}

//	private static void OnTileSourceChanged(BindableObject bindable, object oldValue, object newValue)
//	{
//		if (bindable is not DeepZoomImageView dziv)
//			return;

//		dziv.loader.LevelOverride = dziv.Level;
//		dziv.loader.TileSource = dziv.TileSource;

//		dziv.graphicsView.Invalidate();
//	}

//	class DeepZoomImageViewCanvas : IDeepZoomCanvas
//	{
//		private ICanvas canvas;

//		public DeepZoomImageViewCanvas(ICanvas canvas)
//		{
//			this.canvas = canvas;
//		}

//		public void DrawTile(Tile tile)
//		{
//		}
//	}
//}
