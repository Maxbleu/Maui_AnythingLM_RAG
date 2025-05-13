using System.ComponentModel;
using MauiApp_AnyThingLM_RAG.Factory;
using MauiApp_AnyThingLM_RAG.ViewModels;

namespace MauiApp_AnyThingLM_RAG
{
    public partial class AppShell : Shell
    {
        private WorkspacesListViewModel _workspacesListViewModel;
        public AppShell(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            this.BindingContext = appShellViewModel;

            this._workspacesListViewModel = IPlatformApplication.Current.Services.GetService<WorkspacesListViewModel>();
            this._workspacesListViewModel.PropertyChanged += WorkspacesListViewModel_PropertyChanged;
        }

        private void WorkspacesListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(WorkspacesListViewModel.IsWorkspacesLoaded) && this._workspacesListViewModel.IsWorkspacesLoaded)
            {
                this.scrollViewWorkspaces.Content = WokspacesListViewFactory.Create(this._workspacesListViewModel);
                Task.Delay(1000);
                this._workspacesListViewModel.IsWorkspacesLoaded = false;
            }
        }
    }
}
