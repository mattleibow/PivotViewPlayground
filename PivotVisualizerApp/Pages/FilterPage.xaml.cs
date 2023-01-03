using PivotVisualizerApp.Controls.ViewModels;

namespace PivotVisualizerApp;

public partial class FilterPage : ContentPage
{
	public FilterPage()
	{
		InitializeComponent();

		BindingContext = this;

		LoadCollectionAsync();
	}

	public FilterViewModel? Filter { get; private set; }

	private async void LoadCollectionAsync()
	{
		var datasource = new CxmlPivotDataSource($"{MauiProgram.TestDataPath}conceptcars.cxml");
		await datasource.LoadAsync();

		Filter = new FilterViewModel(datasource);
		OnPropertyChanged(nameof(Filter));
	}
}
