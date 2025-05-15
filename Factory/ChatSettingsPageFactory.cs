using MauiApp_AnyThingLM_RAG.ViewModels;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class ChatSettingsPageFactory
    {
        public static ChatSettingsPage Create(ChatSettingsViewModel chatSettingsViewModel)
        {
            return new ChatSettingsPage(chatSettingsViewModel);
        }
    }
}
