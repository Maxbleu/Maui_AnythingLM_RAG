using MauiApp_AnyThingLM_RAG.ViewModels;
using MauiApp_AnyThingLM_RAG.Views;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace MauiApp_AnyThingLM_RAG
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FerroRosso.ttf", "FerroRosso");
            }).UseMauiCommunityToolkit();

            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();

            builder.Services.AddTransient<ChatSettingsViewModel>();
            builder.Services.AddTransient<ChatSettingsPage>();

            builder.Services.AddTransient<ChatViewModel>();
            builder.Services.AddTransient<ChatPage>();

            builder.Services.AddSingleton<HomePage>();
            
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}