using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Views;

public partial class ChatSettingsPage : ContentPage
{
	public ChatSettingsPage(ChatSettingsViewModel chatSettingsViewModel)
	{
		InitializeComponent();
		this.BindingContext = chatSettingsViewModel;
	}
}