using PivotVisualizerApp.Controls;

namespace PivotVisualizerApp;

public partial class FilterPage : ContentPage
{
	public FilterPage()
	{
		InitializeComponent();

		BindingContext = this;

		LoadCollectionAsync();
	}

	public PivotDataSourceFilter? Filter { get; private set; }

	private async void LoadCollectionAsync()
	{
		var datasource = new CxmlPivotDataSource($"{MauiProgram.TestDataPath}conceptcars.cxml");
		await datasource.LoadAsync();

		Filter = new PivotDataSourceFilter(datasource);
		OnPropertyChanged(nameof(Filter));
	}
}
