using PivotViewer.Core.VisualizerApp.Controls;

namespace PivotViewer.Core.VisualizerApp;

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
		var datasource = new CxmlPivotDataSource("C:\\Projects\\PivotViewPlayground\\PivotViewer.Core.Tests\\TestData\\conceptcars.cxml");
		await datasource.LoadAsync();

		Filter = new PivotDataSourceFilter(datasource);
		OnPropertyChanged(nameof(Filter));
	}
}
