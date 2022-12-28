using System.Xml.Linq;

namespace PivotViewer.Core.DeepZoom;

public class DeepZoomImageCollection
{
	private readonly static XNamespace xmlns = "http://schemas.microsoft.com/deepzoom/2009";

	private string? sourceUri;
	private string? tilesBaseUri;

	public string? BaseUri { get; set; }

	public string? TilesBaseUri => tilesBaseUri;

	public string? SourceUri
	{
		get => sourceUri;
		set
		{
			sourceUri = value;
			tilesBaseUri = GetTilesBaseUri(value);
		}
	}

	public string? ImageFormat { get; set; }

	public int MaxLevel { get; set; }

	public int TileSize { get; set; }

	public Dictionary<string, DeepZoomImage>? Images { get; set; }

	public object GetImages(string imageId, int width, int height)
	{
		var longest = Math.Max(width, height);
		var idealLevel = Math.Log2(longest);
		var level = (int)Math.Ceiling(idealLevel);

		return GetImages(imageId, level);
	}

	public object GetImages(string imageId, int level)
	{
		throw new NotImplementedException();
	}

	public IList<DeepZoomImageTile> GetImageList(string imageId, int level)
	{
		var image = Images[imageId];

		var scale = Math.Pow(2, image.MaxLevel - level);
		var levelHeight = Math.Ceiling(image.Width / image.AspectRatio / scale);
		var levelWidth = Math.Ceiling(image.Width / scale);

		var hslices = (int)Math.Ceiling(levelWidth / TileSize);
		var vslices = (int)Math.Ceiling(levelHeight / TileSize);

		var fileNames = new List<DeepZoomImageTile>(hslices * vslices);
		for (var i = 0; i < hslices; i++)
		{
			for (var j = 0; j < vslices; j++)
			{
				fileNames.Add(new DeepZoomImageTile($"{BaseUri}/{image.TilesBaseUri}/{level}/{i}_{j}.{ImageFormat}"));
			}
		}
		return fileNames;
	}

	public DeepZoomImageTile GetTileImage(string imageId, int level)
	{
		var image = Images[imageId];

		var itemSize = 1 << level;
		var numItems = TileSize / itemSize;
		var morton = GetMortonPoint(image.N);

		var itemCol = morton.X;
		var itemRow = morton.Y;

		var tileCol = (int)Math.Floor((double)itemCol / numItems);
		var tileRow = (int)Math.Floor((double)itemRow / numItems);

		var uri = $"{TilesBaseUri}/{level}/{tileCol}_{tileRow}.{ImageFormat}";

		var biggest = Math.Max(image.Width, image.Height);
		var log = (int)Math.Ceiling(Math.Log2(biggest / itemSize));
		var scale = 1 << log;

		// DZC thumbnails are always >= 1px
		var cropRect = new Rectangle(
			(int)((itemCol % numItems) * itemSize),
			(int)((itemRow % numItems) * itemSize),
			Math.Max(1, (int)Math.Ceiling((double)image.Width / scale)),
			Math.Max(1, (int)Math.Ceiling((double)image.Height / scale)));

		return new DeepZoomImageTile(uri, cropRect);
	}

	public static int GetMortonNumber(int x, int y)
	{
		var result = 0;

		for (var i = 0; i < 16; i++)
		{
			result |=
				((x & (1 << i)) << i) |
				((y & (1 << i)) << (i + 1));
		}

		return result;
	}

	public static Point GetMortonPoint(int morton)
	{
		var x = 0;
		var y = 0;

		for (var i = 0; i < 16; i++)
		{
			x |= (morton & (1 << (2 * i))) >> i;
			y |= (morton & (1 << (2 * i + 1))) >> (i + 1);
		}

		return new Point(x, y);
	}

	public static DeepZoomImageCollection FromFile(string filename)
	{
		var fullPath = Path.GetFullPath(filename);
		var basePath = Path.GetDirectoryName(fullPath)!;
		filename = Path.GetFileName(filename)!;

		var dzc = XDocument.Load(fullPath);

		var collection = dzc.Element(xmlns + "Collection");
		if (collection is null && dzc.Root is not null)
			collection = dzc.Element(dzc.Root.Name.Namespace + "Collection");

		if (collection is null)
			throw new InvalidOperationException("DZC file does not have a collection element at the root.");

		var dzic = new DeepZoomImageCollection
		{
			BaseUri = basePath,
			SourceUri = fullPath,
			ImageFormat = collection.Attribute("Format")?.Value,
			MaxLevel = int.Parse(collection.Attribute("MaxLevel")?.Value),
			TileSize = int.Parse(collection.Attribute("TileSize")?.Value),
		};

		var items = ParseDzcImages(collection);
		dzic.Images = items.ToDictionary(i => i.Id, i => i);

		return dzic;
	}

	private static Size GetMaxSize(IEnumerable<DeepZoomImage> items)
	{
		var maxSize = Size.Empty;
		foreach (var item in items)
		{
			maxSize.Width = Math.Max(maxSize.Width, item.Width);
			maxSize.Height = Math.Max(maxSize.Height, item.Height);
		}
		return maxSize;
	}

	private static IEnumerable<DeepZoomImage> ParseDzcImages(XElement xmlCollection)
	{
		var xmlns = xmlCollection.Name.Namespace;

		var items = xmlCollection.Element(xmlns + "Items");
		if (items is null)
			throw new InvalidOperationException("DCZ file does not have a collection of items.");

		foreach (var item in items.Elements(xmlns + "I"))
		{
			var size = item.Element(xmlns + "Size");
			var viewport = item.Element(xmlns + "Viewport");

			var image = new DeepZoomImage
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

				image.Viewport = new RectangleF(
					float.Parse(viewport.Attribute("X")?.Value),
					float.Parse(viewport.Attribute("Y")?.Value),
					vpWidth,
					vpWidth * image.Height / image.Width);
			}

			yield return image;
		}
	}

	public static string? GetTilesBaseUri(string? sourceUri)
	{
		if (sourceUri is null)
			return null;

		var parts = sourceUri.Split(new string[] { "/", "\\" }, StringSplitOptions.None);
		var lastI = parts.Length - 1;
		var filename = parts[lastI];
		var lastDotI = filename.LastIndexOf(".");

		if (lastDotI > -1)
			parts[lastI] = filename.Substring(0, lastDotI);

		return string.Join("/", parts) + "_files";
	}
}
