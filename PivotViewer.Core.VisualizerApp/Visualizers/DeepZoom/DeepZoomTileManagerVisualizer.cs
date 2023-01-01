using PivotViewer.Core.DeepZoom;
using Microsoft.Maui.Graphics.Converters;
using System.Collections.Concurrent;
using System.Drawing;

#if WINDOWS
using Microsoft.Maui.Graphics.Win2D;
using PlatformImageLoadingService = Microsoft.Maui.Graphics.Win2D.W2DImageLoadingService;
#else
using Microsoft.Maui.Graphics.Platform;
#endif

namespace PivotViewer.Core.VisualizerApp.Visualizers.DeepZoom;

public class DeepZoomTileManagerVisualizer : Visualizer
{
	private readonly MauiGraphicsImageLoader imageLoader;
	private readonly ITileCache<ITileLoadingInfo> tileCache;
	private readonly TileLoader tileLoader;
	private readonly TileManager tileManager;
	private readonly MauiGraphicsCanvas mauiCanvas;

	public DeepZoomTileManagerVisualizer(string name, IDeepZoomTileSource tileSource)
		: base("TileManager " + name)
	{
		imageLoader = new MauiGraphicsImageLoader();
		tileCache = new TileCache<ITileLoadingInfo>(32);
		tileLoader = new TileLoader(imageLoader, tileCache, 1);
		tileManager = new TileManager(tileLoader);

		mauiCanvas = new MauiGraphicsCanvas();

		TileSource = tileSource;
		ScreenScale = 1;

		tileManager.TileSource = tileSource;

		tileManager.TileLoaded += (s, e) => InvalidateDrawing();

		// TODO: fix maui graphics
		imageLoader.DrawLoopRequested += (s, e) => InvalidateDrawing();
	}

	public IDeepZoomTileSource TileSource { get; set; }

	[Entry("Crop rectangle", Converter = typeof(RectFTypeConverter))]
	public RectF CropRect { get; set; } = new RectF(0, 0, 1, 1);

	[Switch("Show crop boundary lines")]
	public bool IsCropLinesVisible { get; set; } = true;

	protected override void DrawBackground(ICanvas canvas, RectF bounds)
	{
		imageLoader.DrawLoopStarted();

		base.DrawBackground(canvas, bounds);
	}

	protected override void DrawScreen(ICanvas canvas, RectF bounds)
	{
		base.DrawScreen(canvas, bounds);

		mauiCanvas.Canvas = canvas;

		var frame = bounds.ToSystemRectangleF();
		var clip = new RectangleF(
			frame.Width * CropRect.X + frame.X,
			frame.Height * CropRect.Y + frame.Y,
			frame.Width * CropRect.Width,
			frame.Height * CropRect.Height);

		tileManager.Update(frame, clip, mauiCanvas);

		mauiCanvas.Canvas = null;

		if (IsCropLinesVisible)
		{
			canvas.StrokeColor = Colors.Gray;
			canvas.StrokeSize = 1;
			canvas.FillColor = null;

			canvas.DrawRectangle(clip.X, clip.Y, clip.Width, clip.Height);
		}
	}

	class MauiGraphicsCanvas : IDeepZoomCanvas
	{
		public ICanvas? Canvas { get; set; }

		public void DrawTile(object image, RectangleF source, RectangleF destination)
		{
			if (Canvas is null)
				return;

			if (image is not Microsoft.Maui.Graphics.IImage mauiImage)
				return;

			Canvas.DrawImage(mauiImage, destination.X, destination.Y, destination.Width, destination.Height);
		}
	}

	class MauiGraphicsImageLoader : IImageLoader
	{
		private readonly IFileLoader fileLoader = new DefaultFileLoader();
		private readonly PlatformImageLoadingService imageLoadingService = new();

		private readonly ConcurrentDictionary<string, TaskCompletionSource<Microsoft.Maui.Graphics.IImage>> imageTasks = new();
		private readonly ConcurrentDictionary<string, TaskCompletionSource<Stream>> streamTasks = new();

		public event EventHandler? DrawLoopRequested;

		public async Task<object?> LoadAsync(string uri, CancellationToken cancellationToken = default)
		{
			// queue the decode task
			var imageTask = imageTasks.GetOrAdd(uri, _ => new TaskCompletionSource<Microsoft.Maui.Graphics.IImage>());

			// queue the download task
			var streamTask = streamTasks.GetOrAdd(uri, _ =>
			{
				var tcs = new TaskCompletionSource<Stream>();
				RequestStream(uri, tcs);
				return tcs;
			});

			// block
			return await imageTask.Task;
		}

		private async void RequestStream(string uri, TaskCompletionSource<Stream> tcs)
		{
			var stream = await fileLoader.LoadAsync(uri);
			tcs.SetResult(stream);

			// request a redraw
			DrawLoopRequested?.Invoke(this, EventArgs.Empty);
		}

		public void DrawLoopStarted()
		{
			foreach (var tcsPair in imageTasks)
			{
				// this image is loaded already
				if (tcsPair.Value.Task.IsCompleted)
					continue;

				// this stream is not yet complete
				if (!streamTasks.TryGetValue(tcsPair.Key, out var streamTask) || !streamTask.Task.IsCompleted)
					continue;

				// decode on this thread
				var image = imageLoadingService.FromStream(streamTask.Task.Result);

				// mark this image as complete
				tcsPair.Value.SetResult(image);
			}
		}
	}
}
