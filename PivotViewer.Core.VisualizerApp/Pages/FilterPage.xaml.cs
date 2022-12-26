using PivotViewer.Core.VisualizerApp.Controls;

namespace PivotViewer.Core.VisualizerApp;

public partial class FilterPage : ContentPage
{
	private readonly PivotDataSource datasource;

	public FilterPage()
	{
		InitializeComponent();

		datasource = CxmlPivotDataSource.FromFile("C:\\Projects\\PivotViewPlayground\\PivotViewer.Core.Tests\\TestData\\conceptcars.cxml");

		Filter = new PivotDataSourceFilter(datasource);

		BindingContext = this;
	}

	public PivotDataSourceFilter Filter { get; }
}
