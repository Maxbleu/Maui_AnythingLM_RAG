using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class ChatViewModelFactory
    {
        public static ChatViewModel Create(string threadName, string slug)
        {
            return new ChatViewModel(threadName, slug);
        }
    }
}
