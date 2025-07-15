using Pivot.Controls.ViewModels;

namespace Pivot.Controls;

/// <summary>
/// This is the main visualization UI component of the <see cref="PivotViewer"/> control.
/// </summary>
public class PivotViewerVisualizationPane : TemplatedView
{
	private View? PART_Container;

	private FilterViewModel? viewModel;

	public PivotViewerVisualizationPane()
	{
		Themes.PivotViewerVisualizationPaneResources.EnsureRegistered();
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
