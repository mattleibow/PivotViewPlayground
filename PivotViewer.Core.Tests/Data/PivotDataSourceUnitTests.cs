namespace PivotViewer.Core.Tests;

public class PivotDataSourceUnitTests
{
	[Fact]
	public void Test()
	{
		var datasource = new PivotDataSource();

		var property1 = new PivotProperty("Property One");
		var property2 = new PivotProperty("Property Two");

		var item = new PivotDataItem
		{
			Id = "id",
			Properties =
			{
				{ property1, new PivotPropertyValueCollection("Value One") },
				{ property2, new PivotPropertyValueCollection("Value Two") },
			}
		};

		datasource.Items = new List<PivotDataItem> { item };
		datasource.Properties = new List<PivotProperty> { property1, property2 };

		//var allValues = datasource.GetAllValues(property1, datasource.Items);

	}
}
