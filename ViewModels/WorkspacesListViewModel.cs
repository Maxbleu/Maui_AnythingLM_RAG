using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp_AnyThingLM_RAG.Factory;
using MauiApp_AnyThingLM_RAG.Managers;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Utils;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class WorkspacesListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<Workspace> _workspaces;
        private AnyThingLLManager _anyThingLLManager;
        private SettingsViewModel _settingsViewModel;

        private bool _isWorkspacesLoaded = false;

        public ObservableCollection<Workspace> Workspaces
        {
            get => _workspaces;
            set
            {
                if (_workspaces != value)
                {
                    _workspaces = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsWorkspacesLoaded
        {
            get => _isWorkspacesLoaded;
            set
            {
                if (_isWorkspacesLoaded != value)
                {
                    _isWorkspacesLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CreateThreadCommand { get; }
        public ICommand DeleteThreadCommand { get; }
        public ICommand CreateWorkspaceCommand { get; }
        public ICommand DeleteWorkspaceCommand { get; }
        public ICommand OpenChatCommand { get; }

        public WorkspacesListViewModel()
        {
            this.CreateThreadCommand = new Command<string>(CreateThreadAsync);
            this.DeleteWorkspaceCommand = new Command<string>(DeleteWorkspaceAsync);
            this.OpenChatCommand = new Command(OpenChatAsync);

            this.Workspaces = new ObservableCollection<Workspace>();

            this._settingsViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
            this._settingsViewModel.PropertyChanged += SettingsViewModel_PropertyChanged;
        }

        //  EVENTOS SUBSCRITOS
        private void SettingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SettingsViewModel.AnyThingLLManager))
            {
                this._anyThingLLManager = this._settingsViewModel.AnyThingLLManager;
                this.LoadWorkspacesAsync();
            }
        }

        private async void LoadWorkspacesAsync()
        {
            this.Workspaces.Clear();
            var result = await this._anyThingLLManager.GetAllWorkSpaces();
            this.Workspaces = new ObservableCollection<Workspace>(result.Workspaces);
            this.IsWorkspacesLoaded = true;
        }
        private async void OpenChatAsync(dynamic parameters)
        {
            ChatViewModel chatViewModel = ChatViewModelFactory.Create(parameters.ThreadName, parameters.WorkspaceSlug);
            await Shell.Current.Navigation.PushAsync(ChatPageFactory.Create(chatViewModel));
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void CreateThreadAsync(string workspaceSlug)
        {
            string result = await GuiUtils.DisplayPromptAlertAsync(
                Shell.Current as Page,
                "Creación de thread",
                "Introduzca el nombre del thread a crear",
                "Crear",
                "Cancelar"
            );

            if (!String.IsNullOrWhiteSpace(result))
            {

            }
        }
        private async void DeleteWorkspaceAsync(string workspageSlug)
        {
            int a = 1;
        }
        private async void DeleteThreadAsync(string workspageSlug, string threadSlug)
        {
            int a = 1;
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
