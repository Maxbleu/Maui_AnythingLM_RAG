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
            AppShell appShell = IPlatformApplication.Current.Services.GetService<AppShell>();
            return new Window(appShell);
        }
    }
}