using MauiApp_AnyThingLM_RAG.Factory;
using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            AppShellViewModel appShellViewModel = IPlatformApplication.Current.Services.GetService<AppShellViewModel>();
            AppShell appShell = AppShellFactory.Create(appShellViewModel);

            return new Window(appShell);
        }
    }
}