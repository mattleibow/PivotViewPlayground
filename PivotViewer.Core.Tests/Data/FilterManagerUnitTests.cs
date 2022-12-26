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

	[Theory]
	[InlineData("POne=VOne", new[] { FilterChangedAction.AddProperty })]
	[InlineData("POne=VA", new[] { FilterChangedAction.AddProperty })]
	[InlineData("POne=VOne,VA", new[] { FilterChangedAction.AddProperty, FilterChangedAction.AddValue })]
	[InlineData("POne=VOne,VFirst", new[] { FilterChangedAction.AddProperty, FilterChangedAction.AddValue })]
	[InlineData("POne=VOne,VA,Va", new[] { FilterChangedAction.AddProperty, FilterChangedAction.AddValue, FilterChangedAction.AddValue })]
	[InlineData("POne=VOne;PTwo=VTwo;POne=Va", new[] { FilterChangedAction.AddProperty, FilterChangedAction.AddProperty, FilterChangedAction.AddValue })]
	[InlineData("PThree=VThree", new[] { FilterChangedAction.AddProperty })]
	[InlineData("POne=VOne;PTwo=VTwo", new[] { FilterChangedAction.AddProperty, FilterChangedAction.AddProperty })]
	[InlineData("POne=VOne;PThree=VThree", new[] { FilterChangedAction.AddProperty, FilterChangedAction.AddProperty })]
	[InlineData("POne=VOne;PThree=VThree;POne=VA", new[] { FilterChangedAction.AddProperty, FilterChangedAction.AddProperty, FilterChangedAction.AddValue })]
	[InlineData("POne=VInvalid", new[] { FilterChangedAction.AddProperty })]
	//[InlineData("PInvalid=VInvalid", new string[0])]
	public void ApplyFilterInvokesCorrectEventAction(string filter, FilterChangedAction[] expectedEvents)
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		var events = new List<FilterChangedEventArgs>();
		filterManager.AppliedFilters.FilterChanged += (sender, e) => events.Add(e);

		ApplyFilter(filterManager, filter);

		Assert.Equal(expectedEvents.Length, events.Count);

		for (var i = 0; i < events.Count; i++)
		{
			var actual = events[i].Action;
			var expected = expectedEvents[i];

			Assert.Equal(expected, actual);
		}
	}

	[Theory]
	[InlineData("POne=VOne", "POne=VOne", new[] { FilterChangedAction.RemoveProperty })]
	[InlineData("POne=VOne,VA", "POne=VA", new[] { FilterChangedAction.RemoveValue })]
	[InlineData("POne=VOne,VFirst", "POne=VOne,VFirst", new[] { FilterChangedAction.RemoveValue, FilterChangedAction.RemoveProperty })]
	[InlineData("POne=VOne,VA,Va", "POne=VOne,VA,Va", new[] { FilterChangedAction.RemoveValue, FilterChangedAction.RemoveValue, FilterChangedAction.RemoveProperty })]
	[InlineData("POne=VOne;PTwo=VTwo;POne=Va", "POne=VOne;PTwo=VTwo;POne=Va", new[] { FilterChangedAction.RemoveValue, FilterChangedAction.RemoveProperty, FilterChangedAction.RemoveProperty })]
	[InlineData("POne=VOne;PTwo=VTwo", "POne=VOne;PTwo=VTwo", new[] { FilterChangedAction.RemoveProperty, FilterChangedAction.RemoveProperty })]
	[InlineData("POne=VOne;PThree=VThree", "POne=VOne;PThree=VThree", new[] { FilterChangedAction.RemoveProperty, FilterChangedAction.RemoveProperty })]
	[InlineData("POne=VOne;PThree=VThree;POne=VA", "POne=VOne;PThree=VThree;POne=VA", new[] { FilterChangedAction.RemoveValue, FilterChangedAction.RemoveProperty, FilterChangedAction.RemoveProperty })]
	[InlineData("POne=VInvalid", "POne=VInvalid", new[] { FilterChangedAction.RemoveProperty })]
	//[InlineData("PInvalid=VInvalid", new string[0])]
	public void UnapplyFilterInvokesCorrectEventAction(string applyFilter, string unapplyFilter, FilterChangedAction[] expectedEvents)
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		var events = new List<FilterChangedEventArgs>();

		ApplyFilter(filterManager, applyFilter);

		filterManager.AppliedFilters.FilterChanged += (sender, e) => events.Add(e);

		UnapplyFilter(filterManager, unapplyFilter);

		Assert.Equal(expectedEvents.Length, events.Count);

		for (var i = 0; i < events.Count; i++)
		{
			var actual = events[i].Action;
			var expected = expectedEvents[i];

			Assert.Equal(expected, actual);
		}
	}

	[Theory]
	[InlineData("POne=VOne", new[] { "1" })]
	[InlineData("POne=VA", new[] { "1", "2" })]
	[InlineData("POne=VOne,VA", new[] { "1", "2" })]
	[InlineData("POne=VOne,VFirst", new[] { "1", "2" })]
	[InlineData("POne=VOne,VA,Va", new[] { "1", "2" })]
	[InlineData("POne=VOne;PTwo=VTwo;POne=Va", new[] { "1" })]
	[InlineData("PThree=VThree", new[] { "2" })]
	[InlineData("POne=VOne;PTwo=VTwo", new[] { "1" })]
	[InlineData("POne=VOne;PThree=VThree", new string[0])]
	[InlineData("POne=VOne;PThree=VThree;POne=VA", new[] { "2" })]
	[InlineData("POne=VInvalid", new string[0])]
	//[InlineData("PInvalid=VInvalid", new string[0])]
	public void ApplyFilterMovesFromFilteredToRemoved(string filter, string[] available)
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		ApplyFilter(filterManager, filter);

		Assert.Equal(available.Length, filterManager.FilteredItems.Count);
		Assert.Equal(datasource.Items.Count - available.Length, filterManager.RemovedItems.Count);

		foreach (var item in datasource.Items)
		{
			var inFiltered = available.Contains(item.Id);

			if (inFiltered)
			{
				Assert.Contains(filterManager.FilteredItems, i => i.Id == item.Id);
				Assert.DoesNotContain(filterManager.RemovedItems, i => i.Id == item.Id);
			}
			else
			{
				Assert.DoesNotContain(filterManager.FilteredItems, i => i.Id == item.Id);
				Assert.Contains(filterManager.RemovedItems, i => i.Id == item.Id);
			}
		}
	}

	[Theory]
	[InlineData("POne=VOne", "POne=VOne", new[] { "1", "2" })]
	[InlineData("POne=VOne,VA", "POne=VA", new[] { "1" })]
	[InlineData("POne=VOne,VA", "POne=VOne", new[] { "1", "2" })]
	[InlineData("POne=VOne,VFirst", "POne=VOne", new[] { "2" })]
	[InlineData("POne=VOne,VFirst", "POne=VOne,VFirst", new[] { "1", "2" })]
	[InlineData("POne=VOne,VA,Va", "POne=VOne,VA,Va", new[] { "1", "2" })]
	[InlineData("POne=VOne;PTwo=VTwo;POne=Va", "POne=VOne;PTwo=VTwo;POne=Va", new[] { "1", "2" })]
	[InlineData("POne=VOne;PTwo=VTwo", "POne=VOne;PTwo=VTwo", new[] { "1", "2" })]
	[InlineData("POne=VOne;PThree=VThree", "POne=VOne;PThree=VThree", new[] { "1", "2" })]
	[InlineData("POne=VOne;PThree=VThree;POne=VA", "POne=VOne;PThree=VThree;POne=VA", new[] { "1", "2" })]
	[InlineData("POne=VInvalid", "POne=VInvalid", new[] { "1", "2" })]
	//[InlineData("PInvalid=VInvalid", new string[0])]
	public void UnapplyFilterMovesFromRemovedToFiltered(string applyFilter, string unapplyFilter, string[] available)
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		var events = new List<FilterChangedEventArgs>();

		ApplyFilter(filterManager, applyFilter);

		UnapplyFilter(filterManager, unapplyFilter);

		Assert.Equal(available.Length, filterManager.FilteredItems.Count);
		Assert.Equal(datasource.Items.Count - available.Length, filterManager.RemovedItems.Count);

		foreach (var item in datasource.Items)
		{
			var inFiltered = available.Contains(item.Id);

			if (inFiltered)
			{
				Assert.Contains(filterManager.FilteredItems, i => i.Id == item.Id);
				Assert.DoesNotContain(filterManager.RemovedItems, i => i.Id == item.Id);
			}
			else
			{
				Assert.DoesNotContain(filterManager.FilteredItems, i => i.Id == item.Id);
				Assert.Contains(filterManager.RemovedItems, i => i.Id == item.Id);
			}
		}
	}

	[Fact]
	public void TestDataSourceFiltersAreCorrect()
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		var expected = new Dictionary<string, Dictionary<string, int>>
		{
			["POne"] = new()
			{
				["VOne"] = 1,
				["VA"] = 2,
				["Va"] = 1,
				["VFirst"] = 1,
				["V@"] = 1,
			},
			["PTwo"] = new()
			{
				["VTwo"] = 1,
				["V2"] = 1,
			},
			["PThree"] = new()
			{
				["VThree"] = 1,
				["V3"] = 1,
			},
		};

		Assert.Empty(filterManager.AppliedFilters);
		AssertFilter(filterManager.AllFilters, expected);
		AssertFilter(filterManager.AvailableFilters, expected);
	}

	[Fact]
	public void TestDataSourceFiltersAreCorrectUsingFilterString()
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		var expected = "POne=VOne|1,VA|2,Va|1,VFirst|1,V@|1;PTwo=VTwo|1,V2|1;PThree=VThree|1,V3|1";

		Assert.Empty(filterManager.AppliedFilters);
		AssertFilter(filterManager.AllFilters, expected);
		AssertFilter(filterManager.AvailableFilters, expected);
	}

	[Theory]
	[InlineData("", "", "POne=VOne|1,VA|2,Va|1,VFirst|1,V@|1;PTwo=VTwo|1,V2|1;PThree=VThree|1,V3|1")]
	[InlineData("POne=VOne", "POne=VOne|1", "POne=VOne|1,VA|2,Va|1,VFirst|1,V@|1;PTwo=VTwo|1,V2|1")]
	[InlineData("PTwo=V2", "PTwo=V2|1", "POne=VOne|1,VA|1,Va|1;PTwo=VTwo|1,V2|1")]
	[InlineData("PThree=VThree", "PThree=VThree|1", "POne=VA|1,VFirst|1,V@|1;PThree=VThree|1,V3|1")]
	public void AppliedFiltersAreCorrect(string filter, string applied, string remaining)
	{
		var datasource = BuildTestDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		ApplyFilter(filterManager, filter);

		AssertFilter(filterManager.AppliedFilters, applied);

		AssertFilter(filterManager.AvailableFilters, remaining);
	}

	[Fact]
	public void SimpleDataFiltersCorrectAfterSingleItem()
	{
		var datasource = new PivotDataSource();

		var property1 = new PivotProperty("POne");

		var item1 = new PivotDataItem
		{
			Id = "1",
			Properties =
			{
				{ property1, new PivotPropertyValueCollection("VOne") },
			}
		};

		datasource.Items = new List<PivotDataItem> { item1 };
		datasource.Properties = new List<PivotProperty> { property1 };

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		AssertFilter(filterManager.AppliedFilters, "");
		AssertFilter(filterManager.AvailableFilters, "POne=VOne|1");

		ApplyFilter(filterManager, "POne=VOne");

		AssertFilter(filterManager.AppliedFilters, "POne=VOne|1");
		AssertFilter(filterManager.AvailableFilters, "POne=VOne|1");
	}

	[Theory]
	[InlineData("", 298)]
	[InlineData("Fuel=Gas", 212)]
	[InlineData("Fuel=Plutonium", 1)]
	[InlineData("Fuel=Vegetable oil", 2)]
	[InlineData("Fuel=Plutonium,Vegetable oil", 3)]
	[InlineData("Fuel=Vegetable oil;Manufacturer=EcoJet", 1)]
	public void ConceptCarsIsAvailableIsCorrect(string filter, int expectedCount)
	{
		var datasource = LoadConceptCarsDataSource();

		var filterManager = new FilterManager(datasource);
		filterManager.RebuildIndexes();

		ApplyFilter(filterManager, filter);

		var filteredItems = filterManager.GetFilteredItems().ToList();

		Assert.Equal(expectedCount, filteredItems.Count);
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
				{ property2, new PivotPropertyValueCollection("VTwo", "V2") },
			}
		};

		var item2 = new PivotDataItem
		{
			Id = "2",
			Properties =
			{
				{ property1, new PivotPropertyValueCollection("VFirst", "VA", "V@") },
				{ property3, new PivotPropertyValueCollection("VThree", "V3") },
			}
		};

		datasource.Items = new List<PivotDataItem> { item1, item2 };
		datasource.Properties = new List<PivotProperty> { property1, property2, property3 };

		return datasource;
	}

	private static PivotDataSource LoadConceptCarsDataSource() =>
		LoadDataSource("TestData/conceptcars.cxml");

	private static PivotDataSource LoadDataSource(string filename)
	{
		var datasource = CxmlPivotDataSource.FromFile(filename);

		return datasource;
	}

	private static void AssertFilter(FilterPropertyCollection filters, string expected)
	{
		var expectedProps = expected.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		Assert.Equal(expectedProps.Length, filters.Count);
		foreach (var expectedPropertyPair in expectedProps)
		{
			var expectedPropPair = expectedPropertyPair.Split("=", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var expectedPropName = expectedPropPair[0];
			var expectedPropVals = expectedPropPair[1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			var prop = filters[expectedPropName];
			Assert.Equal(expectedPropVals.Length, prop.Values.Count);

			foreach (var expectedValuePair in expectedPropVals)
			{
				var expectedVPair = expectedValuePair.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				var expectedValue = expectedVPair[0];
				var expectedCount = expectedVPair[1];

				var val = prop.Values.Get(expectedValue);
				Assert.Equal(int.Parse(expectedCount), val.Count);
			}
		}
	}

	private static void AssertFilter(FilterPropertyCollection filters, Dictionary<string, Dictionary<string, int>> expected)
	{
		Assert.Equal(expected.Count, filters.Count);
		foreach (var expectedPropertyPair in expected)
		{
			var prop = filters[expectedPropertyPair.Key];
			Assert.Equal(expectedPropertyPair.Value.Count, prop.Values.Count);

			foreach (var expectedValuePair in expectedPropertyPair.Value)
			{
				var val = prop.Values.Get(expectedValuePair.Key);
				Assert.Equal(expectedValuePair.Value, val.Count);
			}
		}
	}

	private static void ApplyFilter(FilterManager manager, string filter)
	{
		var props = filter.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (var p in props)
		{
			var pair = p.Split("=", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var name = pair[0];
			var vals = pair[1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			var availableProperty = manager.AllFilters[name];
			foreach (var v in vals)
			{
				if (availableProperty.TryGetFilterValue(v, out var availableFilterValue))
					manager.AppliedFilters.ApplyValue(availableFilterValue);
				else
					manager.AppliedFilters.ApplyValue(new FilterValue(availableProperty, v));
			}
		}
	}

	private static void UnapplyFilter(FilterManager manager, string filter)
	{
		var props = filter.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (var p in props)
		{
			var pair = p.Split("=", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var name = pair[0];
			var vals = pair[1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			foreach (var v in vals)
			{
				manager.AppliedFilters.UnapplyValue(name, v);
			}
		}
	}
}
