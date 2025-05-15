using System.ComponentModel;
using CommunityToolkit.Maui.Views;
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

            this._appShellViewModel.PropertyChanged += WorkspacesListViewModel_PropertyChanged;
        }

        private async void WorkspacesListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(AppShellViewModel.IsExpanderItemsLoaded) && this._appShellViewModel.IsExpanderItemsLoaded)
            {
                foreach(Expander expander in this._appShellViewModel.ExpanderItems)
                {
                    this.stackLayoutWorkspaces.Add(expander);
                }
                await Task.Delay(1000);
                this._appShellViewModel.IsExpanderItemsLoaded = false;
            }
        }
    }
}
