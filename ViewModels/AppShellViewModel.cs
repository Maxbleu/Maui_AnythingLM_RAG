using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp_AnyThingLM_RAG.Factory;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class AppShellViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private List<Tab> _tabItems;

        public SettingsViewModel SettingsViewModel { get; }
        public List<Workspace> ListWorkSpaces { get; set; }
        public List<Tab> TabItems
        {
            get => this._tabItems;
            set
            {
                if(this._tabItems != value)
                {
                    this._tabItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand NavigateToSettingsCommand { get; }

        public AppShellViewModel(SettingsViewModel settingsViewModel)
        {
            this.NavigateToSettingsCommand = new Command(NavigateToSettingsAsync);

            this.ListWorkSpaces = new List<Workspace>();
            this.TabItems = new List<Tab>();

            this.SettingsViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
            this.SettingsViewModel.PropertyChanged += SettingsViewModel_PropertyChanged;
        }

        //  EVENTOS SUBCRITOS
        private async void SettingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.AnyThingLLManager))
            {
                //  Obtener workspaces
                var result = await this.SettingsViewModel.AnyThingLLManager.GetAllWorkSpaces();
                this.ListWorkSpaces = result.Workspaces;

                List<Tab> tabItems = new List<Tab>();

                foreach (var workspace in this.ListWorkSpaces)
                {
                    var flyoutItem = new Tab
                    {
                        Title = workspace.Name,
                    };

                    foreach (Models.Thread thread in workspace.Threads)
                    {
                        var chatViewModel = ChatViewModelFactory.Create(thread.Name, workspace.Slug);
                        var tabThread = new ShellContent
                        {
                            Title = thread.Name,
                            ContentTemplate = new DataTemplate(() => ChatPageFactory.Create(chatViewModel))
                        };

                        flyoutItem.Items.Add(tabThread);
                    }
                    tabItems.Add(flyoutItem);
                }

                this.TabItems = tabItems;
            }
        }
        private async void NavigateToSettingsAsync()
        {
            await Shell.Current.Navigation.PushAsync(IPlatformApplication.Current.Services.GetService<SettingsPage>());
            Shell.Current.FlyoutIsPresented = false;
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
