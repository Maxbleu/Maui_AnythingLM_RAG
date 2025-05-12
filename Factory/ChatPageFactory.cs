using MauiApp_AnyThingLM_RAG.ViewModels;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class ChatPageFactory
    {
        public static ChatPage Create(ChatViewModel chatViewModel)
        {
            return new ChatPage(chatViewModel);
        }
    }
}
