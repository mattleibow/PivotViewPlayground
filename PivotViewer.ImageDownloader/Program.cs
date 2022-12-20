
using System.Xml.Linq;

XNamespace dz = "http://schemas.microsoft.com/deepzoom/2008";


var baseUri = "https://seajax.github.io/collections/conceptcars/conceptcars-Seadragon-21/";
var baseFiles = baseUri + "/conceptcars_files/";
var images = "C:\\Projects\\PivotViewPlayground\\PivotViewer.Core.Tests\\TestData\\conceptcars-Seadragon-21\\conceptcars.dzc";

var dest = "C:\\Projects\\PivotViewPlayground\\PivotViewer.Core.Tests\\TestData\\conceptcars-Seadragon-21\\";

var http = new HttpClient();

// https://seajax.github.io/collections/conceptcars/conceptcars-Seadragon-21/conceptcars_files/8/8_8.jpg
// https://seajax.github.io/collections/conceptcars/conceptcars-Seadragon-21/31473901633839669913363355_files/9/1_0.jpg

var _files = new List<string>();
_files.Add(Path.GetFileNameWithoutExtension(images));

var xdoc = XDocument.Load(images);
foreach (var i in xdoc.Root.Element(dz + "Items").Elements(dz + "I"))
{
	var fname = i.Attribute("Source").Value;
	_files.Add(Path.GetFileNameWithoutExtension(fname));
}


for (int level = 0; level < 20; level++)
{
	var url = baseFiles + "/" + level + "/" + "0_0.jpg";
	var dst = dest + "/" + level + "/" + "0_0.jpg";

	if (!await Download(url, dst))
		break;

	//for (int x = 0; x < 100; x++)
	//{
	//	for (int y = 0; y < 100; y++)
	//	{

	//	}
	//}
}

async Task<bool> Download(string url, string dst)
{
	if (File.Exists(dst))
		return false;

	try
	{
		Directory.CreateDirectory(Path.GetDirectoryName(dst));

		using var stream = await http.GetStreamAsync(url);
		using var file = File.Create(dst);
		await stream.CopyToAsync(file);

		return true;
	}
	catch
	{
		return false;
	}
}

//var xdoc = XDocument.Load(images);
//foreach (var i in xdoc.Root.Element(dz + "Items").Elements(dz + "I"))
//{
//	var fname = i.Attribute("Source").Value;
//	using var stream = await http.GetStreamAsync(baseUri + fname);
//	using var file = File.Create(dest + fname);
//	await stream.CopyToAsync(file);
//}
