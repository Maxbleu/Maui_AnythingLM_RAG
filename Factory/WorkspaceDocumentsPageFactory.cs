using MauiApp_AnyThingLM_RAG.ViewModels;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class WorkspaceDocumentsPageFactory
    {
        public static WorkspaceDocumentsPage Create(WorkspaceDocumentsViewModel workspaceDocumentsViewModel)
        {
            return new WorkspaceDocumentsPage(workspaceDocumentsViewModel);
        }
    }
}
