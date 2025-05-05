using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();
		this.BindingContext = settingsViewModel;
	}
}