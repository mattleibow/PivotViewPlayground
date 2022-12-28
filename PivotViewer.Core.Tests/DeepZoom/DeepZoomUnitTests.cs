using System.Xml.Linq;

namespace PivotViewer.Core.Tests;

public class DeepZoomUnitTests
{
	[Theory]
	[InlineData(0, 0)]
	[InlineData(5, 0)]
	[InlineData(0, 5)]
	[InlineData(5, 5)]
	[InlineData(5, 3)]
	[InlineData(10, 5)]
	[InlineData(5, 10)]
	[InlineData(50, 100)]
	[InlineData(100, 100)]
	[InlineData(500, 500)]
	[InlineData(1000, 1000)]
	[InlineData(2000, 1000)]
	[InlineData(1000, 2000)]
	[InlineData(2000, 2000)]
	[InlineData(ushort.MaxValue, 0)]
	[InlineData(ushort.MaxValue, ushort.MaxValue >> 1)]
	[InlineData(0, ushort.MaxValue >> 1)]
	[InlineData(ushort.MaxValue >> 1, 0)]
	public void EncodeDecodeMortonNumberMaintainsValues(int x, int y)
	{
		var morton = DeepZoomImageCollection.GetMortonNumber(x, y);
		var point = DeepZoomImageCollection.GetMortonPoint(morton);

		Assert.Equal(new Point(x, y), point);
	}

	[Theory]
	[InlineData(0, 0, 0)]
	[InlineData(1, 1, 0)]
	[InlineData(2, 0, 1)]
	[InlineData(3, 1, 1)]
	[InlineData(4, 2, 0)]
	[InlineData(5, 3, 0)]
	[InlineData(6, 2, 1)]
	[InlineData(10, 0, 3)]
	[InlineData(11, 1, 3)]
	[InlineData(15, 3, 3)]
	[InlineData(31, 7, 3)]
	[InlineData(50, 4, 5)]
	[InlineData(150, 6, 9)]
	[InlineData(500, 30, 12)]
	public void DecodeMortonNumberMaintainsValues(int morton, int x, int y)
	{
		var point = DeepZoomImageCollection.GetMortonPoint(morton);

		Assert.Equal(new Point(x, y), point);
	}

	[Theory]
	[InlineData(3, 1, 7)]
	[InlineData(1, 1, 3)]
	public void EncodeMortonNumberMaintainsValues(int x, int y, int morton)
	{
		var actual = DeepZoomImageCollection.GetMortonNumber(x, y);

		Assert.Equal(morton, actual);
	}
}


public class DeepZoomImageCollectionUnitTests
{
	[Theory]
	[InlineData("TestData\\collection-dz_deepzoom\\collection-dz.dzc")]
	[InlineData("TestData\\conceptcars-Seadragon-21\\conceptcars.dzc")]
	public void CanLoadFromFile(string filename)
	{
		var collection = DeepZoomImageCollection.FromFile(filename);
		Assert.NotNull(collection);
	}

	[Theory]
	[InlineData("TestData\\collection-dz_deepzoom\\collection-dz.dzc", "TestData/collection-dz_deepzoom/collection-dz_files")]
	[InlineData("TestData\\collection-dz_deepzoom\\37.dzi", "TestData/collection-dz_deepzoom/37_files")]
	public void GetTilesBaseUriIsCorrect(string sourceUri, string tilesUri)
	{
		var uri = DeepZoomImage.GetTilesBaseUri(sourceUri);

		Assert.Equal(tilesUri, uri);
	}

	[Theory]
	// "0"
	[InlineData("0", 0, 1)]
	[InlineData("0", 1, 1)]
	[InlineData("0", 2, 1)]
	[InlineData("0", 3, 1)]
	[InlineData("0", 4, 1)]
	[InlineData("0", 5, 1)]
	[InlineData("0", 6, 1)]
	[InlineData("0", 7, 1)]
	[InlineData("0", 8, 1)]
	[InlineData("0", 9, 2)]
	[InlineData("0", 10, 6)]
	// "7"
	[InlineData("7", 0, 1)]
	[InlineData("7", 1, 1)]
	[InlineData("7", 2, 1)]
	[InlineData("7", 3, 1)]
	[InlineData("7", 4, 1)]
	[InlineData("7", 5, 1)]
	[InlineData("7", 6, 1)]
	[InlineData("7", 7, 1)]
	[InlineData("7", 8, 1)]
	[InlineData("7", 9, 4)]
	public void GetImageListReturnsCorrectImagesForLevels(string itemId, int level, int expectedCount)
	{
		var expectedBase = Path.GetFullPath($"TestData/collection-dz_deepzoom/{itemId}_files/{level}/");

		var collection = DeepZoomImageCollection.FromFile("TestData\\collection-dz_deepzoom\\collection-dz.dzc");

		var images = collection.GetImageList(itemId, level);

		Assert.Equal(expectedCount, images.Count);
		Assert.Distinct(images);

		foreach (var img in images)
		{
			Assert.StartsWith(expectedBase, Path.GetFullPath(img.Uri));
			Assert.True(File.Exists(img.Uri));
			Assert.Equal(Rectangle.Empty, img.CropRect);
		}
	}

	[Theory]
	[InlineData("0", 0, 0, 0, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("0", 1, 0, 0, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("0", 2, 0, 0, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("0", 3, 0, 0, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("0", 4, 0, 0, 10, 7, "\\4\\0_0.jpg")]
	[InlineData("0", 5, 0, 0, 20, 14, "\\5\\0_0.jpg")]
	[InlineData("0", 6, 0, 0, 40, 27, "\\6\\0_0.jpg")]
	[InlineData("0", 7, 0, 0, 80, 54, "\\7\\0_0.jpg")]
	[InlineData("1", 0, 1, 0, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("1", 1, 2, 0, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("1", 2, 4, 0, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("1", 3, 8, 0, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("1", 4, 16, 0, 10, 8, "\\4\\0_0.jpg")]
	[InlineData("1", 5, 32, 0, 20, 15, "\\5\\0_0.jpg")]
	[InlineData("1", 6, 64, 0, 40, 30, "\\6\\0_0.jpg")]
	[InlineData("1", 7, 128, 0, 80, 60, "\\7\\0_0.jpg")]
	[InlineData("2", 0, 0, 1, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("2", 1, 0, 2, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("2", 2, 0, 4, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("2", 3, 0, 8, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("2", 4, 0, 16, 10, 8, "\\4\\0_0.jpg")]
	[InlineData("2", 5, 0, 32, 20, 15, "\\5\\0_0.jpg")]
	[InlineData("2", 6, 0, 64, 40, 30, "\\6\\0_0.jpg")]
	[InlineData("2", 7, 0, 128, 80, 60, "\\7\\0_0.jpg")]
	[InlineData("7", 0, 0, 0, 5, 5, "\\0\\0_0.jpg")]
	[InlineData("7", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("7", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("7", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("7", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("7", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("7", 6, 0, 0, 5, 5, "\\6\\0_0.jpg")]
	[InlineData("7", 7, 0, 0, 5, 5, "\\7\\0_0.jpg")]
	[InlineData("9", 0, 0, 0, 5, 5, "\\0\\0_0.jpg")]
	[InlineData("9", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("9", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("9", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("9", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("9", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("9", 6, 0, 0, 5, 5, "\\6\\0_0.jpg")]
	[InlineData("9", 7, 0, 0, 5, 5, "\\7\\0_0.jpg")]
	[InlineData("10", 0, 0, 3, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("10", 1, 0, 6, 2, 1, "\\1\\0_0.jpg")]
	[InlineData("10", 2, 0, 12, 3, 2, "\\2\\0_0.jpg")]
	[InlineData("10", 3, 0, 24, 5, 4, "\\3\\0_0.jpg")]
	[InlineData("10", 4, 0, 48, 10,7, "\\4\\0_0.jpg")]
	[InlineData("10", 5, 0, 96, 20,14, "\\5\\0_0.jpg")]
	[InlineData("10", 6, 0, 192, 40, 27, "\\6\\0_0.jpg")]
	[InlineData("10", 7, 0, 128, 80, 54, "\\7\\0_1.jpg")]
	[InlineData("31", 0, 0, 0, 5, 5, "\\0\\0_0.jpg")]
	[InlineData("31", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("31", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("31", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("31", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("31", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("31", 6, 0, 0, 5, 5, "\\6\\0_0.jpg")]
	[InlineData("31", 7, 0, 0, 5, 5, "\\7\\0_0.jpg")]
	[InlineData("49", 0, 0, 0, 1, 1, "\\0\\0_0.jpg")]
	[InlineData("49", 1, 0, 0, 5, 5, "\\1\\0_0.jpg")]
	[InlineData("49", 2, 0, 0, 5, 5, "\\2\\0_0.jpg")]
	[InlineData("49", 3, 0, 0, 5, 5, "\\3\\0_0.jpg")]
	[InlineData("49", 4, 0, 0, 5, 5, "\\4\\0_0.jpg")]
	[InlineData("49", 5, 0, 0, 1, 1, "\\5\\0_0.jpg")]
	[InlineData("49", 6, 0, 0, 5, 5, "\\6\\1_1.jpg")]
	[InlineData("49", 7, 0, 0, 5, 5, "\\7\\2_2.jpg")]
	public void GetTileImageReturnsCorrectImageForLevels(string itemId, int level, int x, int y, int width, int height, string file)
	{
		var expectedBase = Path.GetFullPath($"TestData/collection-dz_deepzoom/collection-dz_files/{level}/");

		var collection = DeepZoomImageCollection.FromFile("TestData\\collection-dz_deepzoom\\collection-dz.dzc");

		var img = collection.GetTileImage(itemId, level);

		Assert.StartsWith(expectedBase, Path.GetFullPath(img.Uri));
		Assert.EndsWith(file, Path.GetFullPath(img.Uri));
		Assert.True(File.Exists(img.Uri));
		Assert.Equal(new Rectangle(x, y, width, height), img.CropRect);
	}
}

public class DeepZoomImageSource
{
}

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
			tilesBaseUri = DeepZoomImage.GetTilesBaseUri(value);
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

	public IList<TileImage> GetImageList(string imageId, int level)
	{
		var image = Images[imageId];

		var scale = Math.Pow(2, image.MaxLevel - level);
		var levelHeight = Math.Ceiling(image.Width / image.AspectRatio / scale);
		var levelWidth = Math.Ceiling(image.Width / scale);

		var hslices = (int)Math.Ceiling(levelWidth / TileSize);
		var vslices = (int)Math.Ceiling(levelHeight / TileSize);

		var fileNames = new List<TileImage>(hslices * vslices);
		for (var i = 0; i < hslices; i++)
		{
			for (var j = 0; j < vslices; j++)
			{
				fileNames.Add(new TileImage($"{BaseUri}/{image.TilesBaseUri}/{level}/{i}_{j}.{ImageFormat}"));
			}
		}
		return fileNames;
	}

	public TileImage GetTileImage(string imageId, int level)
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

		return new TileImage(uri, cropRect);
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

	public double GetLevelScale(int level)
	{
		var diff = MaxLevel - level;
		return 1.0 / (1 << diff);
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
}

public class DeepZoomImage
{
	private string? sourceUri;
	private string? tilesBaseUri;

	public string? Id { get; set; }

	public int N { get; set; }

	public string? SourceUri
	{
		get => sourceUri;
		set
		{
			sourceUri = value;
			tilesBaseUri = GetTilesBaseUri(value);
		}
	}

	public string? TilesBaseUri => tilesBaseUri;

	public int Width { get; set; }

	public int Height { get; set; }

	/// <summary>
	/// Width / Height
	/// </summary>
	public float AspectRatio =>
		Width == 0 || Height == 0
			? 1.0f
			: (float)Width / Height;

	public int MaxLevel =>
		(int)Math.Ceiling(Math.Log2(Math.Max(Width, Height)));

	public RectangleF Viewport { get; set; }

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


public class TileImage
{
	public TileImage(string uri, Rectangle cropRect = default)
	{
		Uri = uri;
		CropRect = cropRect;
	}

	public string Uri { get; }

	public Rectangle CropRect { get; }
}

public class TileSource
{
	public TileSource(Size size, Size tileSize, int tileOverlap, int minLevel, int overviewLevel)
	{
		Size = size;
		TileSize = tileSize;
		TileOverlap = tileOverlap;
		MinLevel = minLevel;
		OverviewLevel = overviewLevel;

		IdealLevel = Math.Log2(Math.Max(Size.Width, Size.Height));
		MaxLevel = (int)Math.Ceiling(IdealLevel);
		MaxSingleTileLevel = Math.Log2(tileSize.Width);
	}

	public Size Size { get; }

	public Size TileSize { get; }

	public int TileOverlap { get; }

	public int MinLevel { get; }

	public int OverviewLevel { get; }

	public double IdealLevel { get; }

	public int MaxLevel { get; }

	public double MaxSingleTileLevel { get; }

	public double GetLevelScale(int level)
	{
		var diff = MaxLevel - level;
		return 1.0 / Math.Pow(2, diff);
	}

	public Size GetTileCould(int level)
	{
		var scale = GetLevelScale(level);

		return new Size(
			(int)Math.Ceiling(scale * Size.Width / TileSize.Width),
			(int)Math.Ceiling(scale * Size.Height / TileSize.Height));
	}
}
