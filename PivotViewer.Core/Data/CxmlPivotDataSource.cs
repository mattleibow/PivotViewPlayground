using System.Globalization;
using System.Xml.Linq;

namespace PivotViewer.Core.Data;

public static class CxmlPivotDataSource
{
	private static readonly XNamespace xmlns = "http://schemas.microsoft.com/collection/metadata/2009";
	private static readonly XNamespace xmlnsLiveLabs = "http://schemas.microsoft.com/livelabs/pivot/collection/2009";
	private static readonly XNamespace xmlnsDZ = "http://schemas.microsoft.com/deepzoom/2008";

	public static PivotDataSource FromFile(string filename, bool loadDeepZoom = false)
	{
		var fullPath = Path.GetFullPath(filename);
		var basePath = Path.GetDirectoryName(fullPath)!;
		filename = Path.GetFileName(filename)!;

		var cxml = XDocument.Load(fullPath);

		var collection = cxml.Element(xmlns + "Collection");
		if (collection is null)
			throw new InvalidOperationException("CXML file does not have a collection element at the root.");

		var datasource = new PivotDataSource
		{
			Name = collection.Attribute("Name")?.Value,
			Icon = collection.Attribute(xmlnsLiveLabs + "Icon")?.Value,
			Properties = new List<PivotProperty>(),
			Items = new List<PivotDataItem>(),
		};

		var properties = ParseFacetCategories(collection).ToDictionary(c => c.Name!, c => c);
		datasource.Properties = properties.Values.ToList();

		ParseItems(collection, properties, datasource.Items, false);

		var supplement = collection.Attribute(xmlnsLiveLabs + "Supplement")?.Value;
		if (supplement is not null)
			ParseSupplement(Path.Combine(basePath, supplement), properties, datasource.Items);

		if (loadDeepZoom)
			ParseDeepZoomImageSizes(collection, basePath, datasource.Items);

		return datasource;
	}

	private static void ParseDeepZoomImageSizes(XElement collection, string basePath, IList<PivotDataItem> items)
	{
		if (collection is null)
			throw new ArgumentNullException(nameof(collection));

		var imgBase = collection.Element(xmlns + "Items")?.Attribute("ImgBase")?.Value;
		if (imgBase is null)
			return;

		imgBase = Path.Combine(basePath, imgBase);

		var dzCollection = XDocument.Load(imgBase);
		var xItems = dzCollection.Element(xmlnsDZ + "Collection").Element(xmlnsDZ + "Items");

		foreach (var i in xItems.Elements(xmlnsDZ + "I"))
		{
			var id = i.Attribute("Id")?.Value;
			var item = items.FirstOrDefault(d => d.Id == id) ?? throw new InvalidDataException($"No supplement data item was found that matches ID: {id}.");

			var size = i.Element(xmlnsDZ + "Size");

			item.ImageWidth = float.Parse(size.Attribute("Width")?.Value, CultureInfo.InvariantCulture);
			item.ImageHeight = float.Parse(size.Attribute("Height")?.Value, CultureInfo.InvariantCulture);
		}
	}

	private static void ParseSupplement(string supplement, Dictionary<string, PivotProperty> properties, IList<PivotDataItem> items)
	{
		var suppCxml = XDocument.Load(supplement);

		var collection = suppCxml.Element(xmlns + "Collection");
		if (collection is null)
			throw new InvalidOperationException("Supplement CXML file does not have a collection element at the root.");

		ParseItems(collection, properties, items, true);
	}

	private static IEnumerable<PivotProperty> ParseFacetCategories(XElement collection)
	{
		if (collection is null)
			throw new ArgumentNullException(nameof(collection));

		// add facets from file
		var categories = collection.Element(xmlns + "FacetCategories");
		if (categories is not null)
		{
			foreach (var facet in categories.Elements(xmlns + "FacetCategory"))
			{
				var name = facet.Attribute("Name")?.Value;
				if (string.IsNullOrEmpty(name))
					throw new InvalidDataException("FacetCategory element does not have a Name attribute.");

				var property = new PivotProperty(name)
				{
					Format = facet.Attribute("Format")?.Value,
					Type = facet.Attribute("Type")?.Value switch
					{
						"String" => PivotPropertyType.Text,
						"LongString" => PivotPropertyType.Text,
						"Number" => PivotPropertyType.Number,
						"DateTime" => PivotPropertyType.DateTime,
						"Link" => PivotPropertyType.Text,
						_ => PivotPropertyType.Text,
					},
					IsFilterVisible = facet.Attribute(xmlnsLiveLabs + "IsFilterVisible")?.Value == "true",
					IsMetaDataVisible = facet.Attribute(xmlnsLiveLabs + "IsMetaDataVisible")?.Value == "true",
					IsSearchVisible = facet.Attribute(xmlnsLiveLabs + "IsWordWheelVisible")?.Value == "true",
				};

				yield return property;
			}
		}

		// add other default facets
		yield return new PivotProperty("Img") { IsFilterVisible = false, IsMetaDataVisible = false, IsSearchVisible = false };
		yield return new PivotProperty("Href") { IsFilterVisible = false, IsMetaDataVisible = false, IsSearchVisible = false };
		yield return new PivotProperty("Name") { IsFilterVisible = false };
		yield return new PivotProperty("Description") { IsFilterVisible = false };
	}

	private static void ParseItems(XElement collection, Dictionary<string, PivotProperty> properties, IList<PivotDataItem> destination, bool isSupplement)
	{
		if (collection is null)
			throw new ArgumentNullException(nameof(collection));

		var items = collection.Element(xmlns + "Items");
		if (items is null)
			return;

		foreach (var item in items.Elements(xmlns + "Item"))
		{
			var id = item.Attribute("Id")?.Value;
			if (id is null)
				throw new InvalidDataException("Item ID was null.");

			var dataItem = isSupplement
				? destination.FirstOrDefault(d => d.Id == id) ?? throw new InvalidDataException($"No supplement data item was found that matches ID: {id}.")
				: new PivotDataItem { Id = id };

			dataItem.Properties ??= new PivotDataItemPropertyCollection();

			ParseAttributeProperty(item, "Img", properties, dataItem.Properties);
			ParseAttributeProperty(item, "Href", properties, dataItem.Properties);
			ParseAttributeProperty(item, "Name", properties, dataItem.Properties);
			ParseElementProperty(item, xmlns + "Description", properties, dataItem.Properties);

			var facets = item.Element(xmlns + "Facets");
			if (facets is not null)
			{
				foreach (var facet in facets.Elements(xmlns + "Facet"))
				{
					var name = facet.Attribute("Name")?.Value;
					if (name is null)
						throw new InvalidDataException("Facet element does not have a Name attribute.");

					ParseFacetProperty(facet, properties[name], dataItem.Properties);
				}
			}

			if (!isSupplement)
				destination.Add(dataItem);
		}
	}

	private static void ParseFacetProperty(XElement facet, PivotProperty property, PivotDataItemPropertyCollection collection)
	{
		foreach (var element in facet.Elements())
		{
			// TODO: properly parse these facets

			var type = element.Name.LocalName;
			if (type == "String" || type == "LongString")
				AddProperty(property, collection, element.Attribute("Value")?.Value ?? string.Empty);
			else if (type == "Number")
				AddProperty(property, collection, element.Attribute("Value")?.Value ?? string.Empty);
			else if (type == "DateTime")
				AddProperty(property, collection, element.Attribute("Value")?.Value ?? string.Empty);
			else if (type == "Link")
				AddProperty(property, collection, element.Attribute("Name")?.Value ?? element.Attribute("Href")?.Value ?? string.Empty);
		}
	}

	private static void ParseAttributeProperty(XElement item, string name, Dictionary<string, PivotProperty> propertiesByName, PivotDataItemPropertyCollection collection)
	{
		var attr = item.Attribute(name)?.Value;
		if (attr is null)
			return;

		var property = propertiesByName[name];
		AddProperty(property, collection, attr);
	}

	private static void ParseElementProperty(XElement item, XName name, Dictionary<string, PivotProperty> propertiesByName, PivotDataItemPropertyCollection collection)
	{
		var value = item.Element(name)?.Value;
		if (value is null)
			return;

		var property = propertiesByName[name.LocalName];
		AddProperty(property, collection, value);
	}

	private static void AddProperty(PivotProperty property, PivotDataItemPropertyCollection collection, IComparable value)
	{
		if (collection.TryGetValue(property, out var propertyValue))
			propertyValue.Add(value);
		else
			collection[property] = new PivotPropertyValueCollection(value);
	}
}

