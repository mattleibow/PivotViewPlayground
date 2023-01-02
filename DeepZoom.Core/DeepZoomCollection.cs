using System.Xml.Linq;

namespace DeepZoom.Core;

public class DeepZoomCollection
{
	private static readonly XNamespace xmlns = "http://schemas.microsoft.com/deepzoom/2009";

	private readonly Dictionary<string, DeepZoomCollectionTileSource> dzcTileSources = new();
	private readonly Dictionary<string, DeepZoomImageTileSource> dziTileSources = new();

	private readonly IDeepZoomFileFetcher fileLoader;

	private string sourceUri = null!;

	public DeepZoomCollection(string sourceUri, IDeepZoomFileFetcher? fileLoader = default)
	{
		this.fileLoader = fileLoader ?? new DefaultDeepZoomFileFetcher();

		SourceUri = sourceUri;
	}

	public string SourceUri
	{
		get => sourceUri;
		private set
		{
			sourceUri = value;
			BaseUri = DeepZoomExtensions.GetBaseUri(value);
			TileBaseUri = DeepZoomExtensions.GetTileBaseUri(value);
		}
	}

	public string? BaseUri { get; private set; }

	public string? TileBaseUri { get; private set; }

	public string? TileFileFormat { get; private set; }

	public int MaxLevel { get; private set; }

	public int TileSize { get; private set; }

	public DeepZoomItemCollection Items { get; } = new();

	public event EventHandler? Loaded;

	public async Task LoadAsync(CancellationToken cancellationToken = default)
	{
		using var stream = await fileLoader.FetchAsync(SourceUri, cancellationToken);

		await LoadAsync(stream, cancellationToken);

		Loaded?.Invoke(this, EventArgs.Empty);
	}

	public async Task LoadImageAsync(string itemId, CancellationToken cancellationToken = default)
	{
		var item = Items[itemId];

		if (item.Image is not null)
			return;

		using var stream = await fileLoader.FetchAsync($"{BaseUri}/{item.SourceUri}", cancellationToken);

		await LoadImageAsync(item, stream, cancellationToken);

		Loaded?.Invoke(this, EventArgs.Empty);
	}

	private async Task LoadAsync(Stream stream, CancellationToken cancellationToken)
	{
		var dzc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

		var collection = dzc.Element(xmlns + "Collection");
		if (collection is null && dzc.Root is not null)
			collection = dzc.Element(dzc.Root.Name.Namespace + "Collection");

		if (collection is null)
			throw new InvalidOperationException("DZC file does not have a collection element at the root.");

		TileFileFormat = collection.Attribute("Format")?.Value;
		MaxLevel = int.Parse(collection.Attribute("MaxLevel")?.Value);
		TileSize = int.Parse(collection.Attribute("TileSize")?.Value);

		ParseDzcItems(collection);
	}

	private async Task LoadImageAsync(DeepZoomItem item, Stream stream, CancellationToken cancellationToken)
	{
		var dzi = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

		var image = dzi.Element(xmlns + "Image");
		if (image is null && dzi.Root is not null)
			image = dzi.Element(dzi.Root.Name.Namespace + "Image");

		if (image is null)
			throw new InvalidOperationException("DZI file does not have an image element at the root.");

		ParseDziItem(item, image);
	}

	private void ParseDzcItems(XElement xmlCollection)
	{
		var xmlns = xmlCollection.Name.Namespace;

		var items = xmlCollection.Element(xmlns + "Items");
		if (items is null)
			throw new InvalidOperationException("DCZ file does not have a collection of items.");

		foreach (var item in items.Elements(xmlns + "I"))
		{
			var size = item.Element(xmlns + "Size");
			var viewport = item.Element(xmlns + "Viewport");

			var dzci = new DeepZoomItem
			{
				Id = item.Attribute("Id")?.Value,
				N = int.Parse(item.Attribute("N")?.Value),
				SourceUri = item.Attribute("Source")?.Value,
				Width = int.Parse(size?.Attribute("Width")?.Value),
				Height = int.Parse(size?.Attribute("Height")?.Value),
			};

			if (viewport is not null)
			{
				var vpWidth = float.Parse(viewport.Attribute("Width")?.Value);

				dzci.Viewport = new RectangleF(
					float.Parse(viewport.Attribute("X")?.Value),
					float.Parse(viewport.Attribute("Y")?.Value),
					vpWidth,
					vpWidth * dzci.Height / dzci.Width);
			}

			Items.Add(dzci);

			dzcTileSources[dzci.Id] = new DeepZoomCollectionTileSource(
				dzci.Id,
				dzci.N,
				dzci.Width,
				dzci.Height,
				MaxLevel,
				TileSize,
				TileBaseUri,
				TileFileFormat);
		}
	}

	private void ParseDziItem(DeepZoomItem item, XElement xmlImage)
	{
		var xmlns = xmlImage.Name.Namespace;

		var size = xmlImage.Element(xmlns + "Size");

		var image = new DeepZoomImage
		{
			SourceUri = $"{BaseUri}/{item.SourceUri}",
			Height = int.Parse(size?.Attribute("Height")?.Value),
			Width = int.Parse(size?.Attribute("Width")?.Value),
			TileFileFormat = xmlImage.Attribute("Format")?.Value,
			TileSize = int.Parse(xmlImage?.Attribute("TileSize")?.Value),
			TileOverlap = int.Parse(xmlImage?.Attribute("Overlap")?.Value),
		};

		item.Image = image;

		dziTileSources[item.Id] = new DeepZoomImageTileSource(
			item.Id,
			image.Width,
			image.Height,
			image.MaxLevel,
			image.TileSize,
			image.TileOverlap,
			image.TileBaseUri,
			image.TileFileFormat);
	}

	public IList<DeepZoomTileSourceTile>? GetFullImageTiles(string itemId, int level)
	{
		if (dziTileSources.TryGetValue(itemId, out var source))
			return source.GetImageTiles(level);

		return null;
	}

	public DeepZoomTileSourceTile? GetThumbnailImageTile(string itemId, int level)
	{
		if (dzcTileSources.TryGetValue(itemId, out var source))
			return source.GetImageTiles(level).FirstOrDefault();

		return null;
	}

	public IList<DeepZoomTileSourceTile> GetImageTiles(string itemId, int level)
	{
		if (dziTileSources.TryGetValue(itemId, out var dziSource))
			return dziSource.GetImageTiles(level);

		if (dzcTileSources.TryGetValue(itemId, out var dzcSource))
			return dzcSource.GetImageTiles(level);

		throw new KeyNotFoundException(nameof(itemId));
	}
}
