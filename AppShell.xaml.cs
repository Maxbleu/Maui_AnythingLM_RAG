using System.ComponentModel;
using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG
{
    public partial class AppShell : Shell
    {
        private AppShellViewModel _appShellViewModel;
        public AppShell(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            this._appShellViewModel = appShellViewModel;
            this.BindingContext = this._appShellViewModel;
            this._appShellViewModel.PropertyChanged += AppShellViewModel_PropertyChanged;
        }
        //  EVENTOS SUBCRITOS
        private void AppShellViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppShellViewModel.TabItems))
            {
                if (!this._appShellViewModel.SettingsViewModel.IsAnyThingLMRunning) return;
                foreach (Tab tab in this._appShellViewModel.TabItems)
                {
                    this.Workspaces.Items.Add(tab);
                }
            }
        }
    }
}
