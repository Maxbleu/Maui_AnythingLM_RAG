using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class AppShellFactory
    {
        public static AppShell Create(AppShellViewModel appShellViewModel)
        {
            return new AppShell(appShellViewModel);
        }
    }
}
