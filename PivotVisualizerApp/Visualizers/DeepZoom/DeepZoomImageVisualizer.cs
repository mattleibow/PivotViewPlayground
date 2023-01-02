using System.Drawing;
using Microsoft.Maui.Graphics.Converters;

#if WINDOWS
using Microsoft.Maui.Graphics.Win2D;
using PlatformImageLoadingService = Microsoft.Maui.Graphics.Win2D.W2DImageLoadingService;
#else
using Microsoft.Maui.Graphics.Platform;
#endif

namespace PivotVisualizerApp.Visualizers.DeepZoom;

public class DeepZoomImageVisualizer : Visualizer
{
	private readonly Dictionary<string, Stream?> streams = new();
	private readonly Dictionary<string, Microsoft.Maui.Graphics.IImage?> images = new();
	private readonly PlatformImageLoadingService imageLoadingService = new();
	private readonly IDeepZoomFileFetcher fileFetcher = new DefaultDeepZoomFileFetcher();

	public DeepZoomImageVisualizer(string name, ITileSource tileSource)
		: base("DeepZoom " + name)
	{
		TileSource = tileSource;
		ScreenScale = 1;
	}

	public ITileSource TileSource { get; set; }

	[Slider("Level", -1, 10)]
	public int Level { get; set; } = 6;

	[Entry("Crop rectangle", Converter = typeof(RectFTypeConverter))]
	public RectF CropRect { get; set; } = new RectF(0, 0, 1, 1);

	[Switch("Show crop boundary lines")]
	public bool IsCropLinesVisible { get; set; } = true;

	protected override void DrawScreen(ICanvas canvas, RectF bounds)
	{
		base.DrawScreen(canvas, bounds);

		var actualLevel = Level != -1 ? Level : GetIdealLevel(bounds.ToSystemRectangleF());
		actualLevel = Math.Clamp(actualLevel, 0, TileSource.MaxLevel);

		var fitBounds = TileManager.AspectFillImageIntoFrame(bounds.ToSystemRectangleF(), TileSource.ItemSize);
		var newBounds = fitBounds.ToRectF();

		var imageTiles = TileSource.GetImageTiles(actualLevel).ToDictionary(t => t.Coordinates, t => t);

		var screenRect = new RectangleF(0, 0, 1, 1);

		var cropped = screenRect;
		cropped.Intersect(CropRect.ToSystemRectangleF());

		var tiles = TileSource.GetTiles(actualLevel, cropped);

		for (var y = tiles.Top; y < tiles.Bottom; y++)
		{
			for (var x = tiles.Left; x < tiles.Right; x++)
			{
				var tile = imageTiles[new(x, y)];

				var image = GetImage(tile);
				if (image is null)
					continue;

				var tileBounds = TileSource.GetTileBounds(actualLevel, x, y);

				var pixelBounds = new RectangleF(
					tileBounds.X * newBounds.Width,
					tileBounds.Y * newBounds.Height,
					tileBounds.Width * newBounds.Width,
					tileBounds.Height * newBounds.Height);
				pixelBounds.Offset(newBounds.ToSystemRectangleF().Location);

				canvas.DrawImage(image, pixelBounds.X, pixelBounds.Y, pixelBounds.Width, pixelBounds.Height);
			}
		}

		if (IsCropLinesVisible)
		{
			canvas.StrokeColor = Colors.Gray;
			canvas.StrokeSize = 1;
			canvas.FillColor = null;

			var pixelBounds = new Rect(
				CropRect.X * newBounds.Width,
				CropRect.Y * newBounds.Height,
				CropRect.Width * newBounds.Width,
				CropRect.Height * newBounds.Height);
			pixelBounds = pixelBounds.Offset(newBounds.Location);

			canvas.DrawRectangle(pixelBounds);
		}
	}

	private Microsoft.Maui.Graphics.IImage? GetImage(DeepZoomTileSourceTile tile)
	{
		// is the image loaded (or loading)
		if (images.TryGetValue(tile.Uri, out var image))
			return image;

		// is there a file stream
		if (streams.TryGetValue(tile.Uri, out var stream))
		{
			// start a new tile loading
			images[tile.Uri] = null;
			DecodeImage(tile, stream);
			return null;
		}

		// start a new tile downloading
		streams[tile.Uri] = null;
		LoadStream(tile);
		return null;
	}

	private async void LoadStream(DeepZoomTileSourceTile tile)
	{
		// load in the background
		await Task.Run(async () =>
		{
			var stream = await fileFetcher.FetchAsync(tile.Uri);
			streams[tile.Uri] = stream;
		});

		// redraw
		InvalidateDrawing();
	}

	private async void DecodeImage(DeepZoomTileSourceTile tile, Stream? stream)
	{
		// stream is still downloading
		if (stream is null)
			return;

#if WINDOWS
		// load in the the current thread
		var image = imageLoadingService.FromStream(stream);
		images[tile.Uri] = image;
#else
		// load in the background
		await Task.Run(() =>
		{
			var image = imageLoadingService.FromStream(stream);
			images[tile.Uri] = image;
		});
#endif

		// redraw
		InvalidateDrawing();
	}

	private static int GetIdealLevel(RectangleF frame)
	{
		var longest = Math.Max(frame.Width, frame.Height);
		return (int)Math.Ceiling(Math.Log2(longest));
	}
}
