using System.Collections.ObjectModel;
using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class WorkspaceDocumentsViewModelFactory
    {
        public static WorkspaceDocumentsViewModel Create(string slug, ObservableCollection<string> documents)
        {
            return new WorkspaceDocumentsViewModel(slug, documents);
        }
    }
}
