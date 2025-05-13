using MauiApp_AnyThingLM_RAG.ViewModels;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class WokspacesListViewFactory
    {
        public static WorkspacesListView Create(WorkspacesListViewModel workspacesListViewModel)
        {
            return new WorkspacesListView(workspacesListViewModel);
        }
    }
}
