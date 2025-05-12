using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class WorkspaceDocumentsViewModelFactory
    {
        public static WorkspaceDocumentsViewModel Create(List<Metadata> documents, string slug)
        {
            return new WorkspaceDocumentsViewModel(documents, slug);
        }
    }
}
