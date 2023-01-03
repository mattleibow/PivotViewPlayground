using PivotVisualizerApp.Controls.ViewModels;

namespace PivotVisualizerApp.Controls;

public class PivotViewerFilterPane : TemplatedView
{
	private View? PART_Container;

	private FilterViewModel? viewModel;

	public PivotViewerFilterPane()
	{
		Themes.PivotViewerFilterPaneResources.EnsureRegistered();
	}

	public FilterViewModel? ViewModel
	{
		get => viewModel;
		set
		{
			viewModel = value;

			if (PART_Container is not null)
				PART_Container.BindingContext = ViewModel;
		}
	}

	protected override void OnApplyTemplate()
	{
		PART_Container = GetTemplateChild("PART_Container") as View;
		if (PART_Container is not null)
			PART_Container.BindingContext = ViewModel;
	}
}
