using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Views
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage(ChatViewModel chatViewModel)
        {
            InitializeComponent();
            this.BindingContext = chatViewModel;
        }
    }

}
