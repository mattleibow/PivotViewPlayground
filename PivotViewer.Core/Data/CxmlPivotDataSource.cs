using System.Xml.Linq;

namespace PivotViewer.Core.Data;

public class CxmlPivotDataSource : PivotDataSource
{
	private static readonly XNamespace xmlns = "http://schemas.microsoft.com/collection/metadata/2009";
	private static readonly XNamespace xmlnsLiveLabs = "http://schemas.microsoft.com/livelabs/pivot/collection/2009";

	private readonly IFileLoader fileLoader;

	private string sourceUri = null!;

	public CxmlPivotDataSource(string sourceUri, IFileLoader? fileLoader = default)
	{
		this.fileLoader = fileLoader ?? new DefaultFileLoader();

		SourceUri = sourceUri;
	}

	public string SourceUri
	{
		get => sourceUri;
		private set
		{
			sourceUri = value;
			BaseUri = DeepZoomExtensions.GetBaseUri(value);
		}
	}

	public string? BaseUri { get; private set; }

	public string? ImageBaseUri { get; private set; }

	public string? ImageUri
	{
		get
		{
			if (ImageBaseUri is null)
				return null;
			if (BaseUri is null)
				return ImageBaseUri;
			return Path.Combine(BaseUri, ImageBaseUri);
		}
	}

	public string? SupplementBaseUri { get; private set; }

	public string? SupplementUri
	{
		get
		{
			if (SupplementBaseUri is null)
				return null;
			if (BaseUri is null)
				return SupplementBaseUri;
			return Path.Combine(BaseUri, SupplementBaseUri);
		}
	}

	public event EventHandler? Loaded;

	public async Task LoadAsync(CancellationToken cancellationToken = default)
	{
		using var stream = await fileLoader.LoadAsync(SourceUri, cancellationToken);

		await LoadAsync(stream, cancellationToken);

		Loaded?.Invoke(this, EventArgs.Empty);
	}

	private async Task LoadAsync(Stream stream, CancellationToken cancellationToken)
	{
		var cxml = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

		var collection = cxml.Element(xmlns + "Collection");
		if (collection is null)
			throw new InvalidOperationException("CXML file does not have a collection element at the root.");

		var items = collection.Element(xmlns + "Items");
		if (items is null)
			throw new InvalidOperationException("CXML file does not contain any items.");

		Name = collection.Attribute("Name")?.Value;
		Icon = collection.Attribute(xmlnsLiveLabs + "Icon")?.Value;
		ImageBaseUri = items.Attribute("ImgBase")?.Value;

		ParseFacetCategories(collection);

		ParseItems(items, false);

		var supplement = collection.Attribute(xmlnsLiveLabs + "Supplement")?.Value;
		if (supplement is not null)
		{
			SupplementBaseUri = supplement;

			await ParseSupplementAsync(cancellationToken);
		}
	}

	private async Task ParseSupplementAsync(CancellationToken cancellationToken)
	{
		if (SupplementUri is null)
			throw new InvalidOperationException("CXML file does not have a supplement collection file.");

		using var stream = await fileLoader.LoadAsync(SupplementUri, cancellationToken);

		await ParseSupplementAsync(stream, cancellationToken);
	}

	private async Task ParseSupplementAsync(Stream stream, CancellationToken cancellationToken)
	{
		var suppCxml = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

		var collection = suppCxml.Element(xmlns + "Collection");
		if (collection is null)
			throw new InvalidOperationException("Supplement CXML file does not have a collection element at the root.");

		var xitems = collection.Element(xmlns + "Items");
		if (xitems is null)
			throw new InvalidOperationException("Supplement CXML file does not contain any items.");

		ParseItems(xitems, true);
	}

	private void ParseFacetCategories(XElement collection)
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

				Properties.Add(property);
			}
		}

		// add other default facets
		Properties.Add(new PivotProperty("Img") { IsFilterVisible = false, IsMetaDataVisible = false, IsSearchVisible = false });
		Properties.Add(new PivotProperty("Href") { IsFilterVisible = false, IsMetaDataVisible = false, IsSearchVisible = false });
		Properties.Add(new PivotProperty("Name") { IsFilterVisible = false });
		Properties.Add(new PivotProperty("Description") { IsFilterVisible = false });
	}

	private void ParseItems(XElement xitems, bool isSupplement)
	{
		if (xitems is null)
			throw new ArgumentNullException(nameof(xitems));

		foreach (var item in xitems.Elements(xmlns + "Item"))
		{
			var id = item.Attribute("Id")?.Value;
			if (id is null)
				throw new InvalidDataException("Item ID was null.");

			var dataItem = isSupplement
				? Items[id]
				: new PivotDataItem { Id = id };

			dataItem.Properties ??= new PivotDataItemPropertyCollection();

			ParseAttributeProperty(item, "Img", dataItem.Properties);
			ParseAttributeProperty(item, "Href", dataItem.Properties);
			ParseAttributeProperty(item, "Name", dataItem.Properties);
			ParseElementProperty(item, xmlns + "Description", dataItem.Properties);

			var facets = item.Element(xmlns + "Facets");
			if (facets is not null)
			{
				foreach (var facet in facets.Elements(xmlns + "Facet"))
				{
					var name = facet.Attribute("Name")?.Value;
					if (name is null)
						throw new InvalidDataException("Facet element does not have a Name attribute.");

					ParseFacetProperty(facet, Properties[name], dataItem.Properties);
				}
			}

			if (!isSupplement)
				Items.Add(dataItem);
		}
	}

	private void ParseFacetProperty(XElement facet, PivotProperty property, PivotDataItemPropertyCollection collection)
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

	private void ParseAttributeProperty(XElement item, string name, PivotDataItemPropertyCollection collection)
	{
		var attr = item.Attribute(name)?.Value;
		if (attr is null)
			return;

		var property = Properties[name];
		AddProperty(property, collection, attr);
	}

	private void ParseElementProperty(XElement item, XName name, PivotDataItemPropertyCollection collection)
	{
		var value = item.Element(name)?.Value;
		if (value is null)
			return;

		var property = Properties[name.LocalName];
		AddProperty(property, collection, value);
	}

	private void AddProperty(PivotProperty property, PivotDataItemPropertyCollection collection, IComparable value)
	{
		if (collection.TryGetValue(property, out var propertyValue))
			propertyValue.Add(value);
		else
			collection[property] = new PivotPropertyValueCollection(value);
	}
}
