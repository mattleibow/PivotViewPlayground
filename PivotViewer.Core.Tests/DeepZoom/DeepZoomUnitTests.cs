using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PivotViewer.Core.Tests;

public class DeepZoomUnitTests
{
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
	[InlineData(0, 1)]
	[InlineData(1, 1)]
	[InlineData(2, 1)]
	[InlineData(3, 1)]
	[InlineData(4, 1)]
	[InlineData(5, 1)]
	[InlineData(6, 1)]
	[InlineData(7, 1)]
	[InlineData(8, 1)]
	[InlineData(9, 2)]
	[InlineData(10, 6)]
	public void GetImageListReturnsCorrectImagesForLevels(int level, int expectedCount)
	{
		var expectedBase = Path.GetFullPath($"TestData/collection-dz_deepzoom/0_files/{level}/");

		var collection = DeepZoomImageCollection.FromFile("TestData\\collection-dz_deepzoom\\collection-dz.dzc");

		var images = collection.GetImageList("0", level);

		Assert.Equal(expectedCount, images.Count);
		Assert.Distinct(images);

		foreach (var img in images)
		{
			Assert.StartsWith(expectedBase, Path.GetFullPath(img));
			Assert.True(File.Exists(img));
		}
	}
}

public class DeepZoomImageSource
{
}

public class DeepZoomImageCollection
{
	private readonly static XNamespace xmlns = "http://schemas.microsoft.com/deepzoom/2009";

	public string? BaseUri { get; set; }

	public string? SourceUri { get; set; }

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

	public IList<string> GetImageList(string imageId, int level)
	{
		if (Images is null || Images.Count == 0)
			return Array.Empty<string>();

		var image = Images[imageId];

		var scale = Math.Pow(2, image.MaxLevel - level);
		var levelHeight = Math.Ceiling(image.Width / image.AspectRatio / scale);
		var levelWidth = Math.Ceiling(image.Width / scale);

		var hslices = (int)Math.Ceiling(levelWidth / TileSize);
		var vslices = (int)Math.Ceiling(levelHeight / TileSize);

		var fileNames = new List<string>(hslices * vslices);
		for (var i = 0; i < hslices; i++)
		{
			for (var j = 0; j < vslices; j++)
			{
				fileNames.Add($"{BaseUri}/{image.TilesBaseUri}/{level}/{i}_{j}.{ImageFormat}");
			}
		}
		return fileNames;
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
	public string? Uri { get; set; }

	public RectangleF CropRect { get; set; }
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
