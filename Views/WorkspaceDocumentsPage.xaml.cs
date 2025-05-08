using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Views;

public partial class WorkspaceDocumentsPage : ContentPage
{
	public WorkspaceDocumentsPage(WorkspaceDocumentsViewModel workspaceDocumentsViewModel)
	{
		InitializeComponent();
		this.BindingContext = workspaceDocumentsViewModel;
    }
}