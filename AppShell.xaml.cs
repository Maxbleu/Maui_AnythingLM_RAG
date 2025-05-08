using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("workspaceDocuments", typeof(WorkspaceDocumentsPage));
        }
    }
}
