namespace PivotViewer.Core.Tests;

public class FilterManagerUnitTests
{
	[Fact]
	public void NoFilterKeepsAllItems()
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		var item1 = datasource.Items[0];
		var item2 = datasource.Items[1];

		Assert.True(filterManager.IsAvailable(item1));
		Assert.True(filterManager.IsAvailable(item2));
	}

	[Theory]
	[InlineData("POne=VOne", new[] { "1" })]
	[InlineData("POne=VA", new[] { "1", "2" })]
	[InlineData("POne=VOne,VA", new[] { "1", "2" })]
	[InlineData("PThree=VThree", new[] { "2" })]
	[InlineData("POne=VOne;PTwo=VTwo", new[] { "1" })]
	[InlineData("POne=VOne;PThree=VThree", new string[0])]
	[InlineData("POne=VInvalid", new string[0])]
	//[InlineData("PInvalid=VInvalid", new string[0])]
	public void IsAvailableIsCorrect(string filter, string[] available)
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		ApplyFilter(filterManager, filter);

		foreach (var item in datasource.Items)
		{
			var shouldBe = available.Contains(item.Id);
			Assert.Equal(shouldBe, filterManager.IsAvailable(item));
		}
	}

	private static PivotDataSource BuildTestDataSource()
	{
		var datasource = new PivotDataSource();

		var property1 = new PivotProperty("POne");
		var property2 = new PivotProperty("PTwo");
		var property3 = new PivotProperty("PThree");

		var item1 = new PivotDataItem
		{
			Id = "1",
			Properties =
			{
				{ property1, new PivotPropertyValueCollection("VOne", "VA", "Va") },
				{ property2, new PivotPropertyValueCollection("VTwo") },
			}
		};

		var item2 = new PivotDataItem
		{
			Id = "2",
			Properties =
			{
				{ property1, new PivotPropertyValueCollection("VFirst", "VA", "V@") },
				{ property3, new PivotPropertyValueCollection("VThree") },
			}
		};

		datasource.Items = new List<PivotDataItem> { item1, item2 };
		datasource.Properties = new List<PivotProperty> { property1, property2, property3 };

		return datasource;
	}

	private static void ApplyFilter(FilterManager manager, string filter)
	{
		var props = filter.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (var p in props)
		{
			var pair = p.Split("=", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var name = pair[0];
			var vals = pair[1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			var availableProperty = manager.AvailableFilters[name];
			foreach (var v in vals)
			{
				if (availableProperty.TryGetFilterValue(v, out var availableFilterValue))
					manager.AppliedFilters.ApplyValue(availableFilterValue);
				else
					manager.AppliedFilters.ApplyValue(new FilterValue(availableProperty, v));
			}
		}
	}
}
