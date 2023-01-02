namespace PivotVisualizerApp.Controls;

public class PivotViewer : TemplatedView
{
	private PivotDataSource? dataSource;
	private FilterManager? filterManager;

	public PivotViewer()
	{
		Themes.PivotViewerResources.EnsureRegistered();
	}

	protected override void OnApplyTemplate()
	{
		var templateChild = GetTemplateChild("PART_Container");
	}

	public PivotDataSource? DataSource
	{
		get => dataSource;
		set
		{
			filterManager = null;
			
			dataSource = value;

			if (dataSource is not null)
			{
				filterManager = new FilterManager(dataSource);

			}

			OnPropertyChanged();
		}
	}
}

public class PivotViewerFilterPane : TemplatedView
{
	public PivotViewerFilterPane()
	{
		Themes.PivotViewerFilterPaneResources.EnsureRegistered();
	}

	protected override void OnApplyTemplate()
	{
		var templateChild = GetTemplateChild("PART_Container");
	}
}
