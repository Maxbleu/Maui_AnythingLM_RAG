using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class AppShellViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToHomeCommand { get; }

        public AppShellViewModel(SettingsViewModel settingsViewModel)
        {
            this.NavigateToSettingsCommand = new Command(NavigateToSettingsAsync);
            this.NavigateToHomeCommand = new Command(NavigateToHomeAsync);
        }

        /// <summary>
        /// Este método se encarga de navegar
        /// desde la página en la que me encuentre
        /// hasta la página home
        /// </summary>
        private async void NavigateToHomeAsync()
        {
            if (Shell.Current.CurrentPage is HomePage)
            {
                Shell.Current.FlyoutIsPresented = false;
                return;
            }
            await Shell.Current.Navigation.PushAsync(IPlatformApplication.Current.Services.GetService<HomePage>());
            Shell.Current.FlyoutIsPresented = false;
        }
        /// <summary>
        /// Este método se encarga de navegar desde la página en la
        /// que me encuentre hasta la página settings
        /// </summary>
        private async void NavigateToSettingsAsync()
        {
            if (Shell.Current.CurrentPage is SettingsPage)
            {
                Shell.Current.FlyoutIsPresented = false;
                return;
            }
            await Shell.Current.Navigation.PushAsync(IPlatformApplication.Current.Services.GetService<SettingsPage>());
            Shell.Current.FlyoutIsPresented = false;
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
