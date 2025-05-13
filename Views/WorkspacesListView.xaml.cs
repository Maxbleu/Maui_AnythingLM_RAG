using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Views;

public partial class WorkspacesListView : ContentView
{
	public WorkspacesListView(WorkspacesListViewModel workspacesListViewModel)
	{
		InitializeComponent();
		this.BindingContext = workspacesListViewModel;
	}
}