using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp_AnyThingLM_RAG.Models;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SettingsViewModel _settingsViewModel;

        private string _newMessageText;

        public ObservableCollection<Message> Messages { get; set; }
        public string NewMessageText
        {
            get => _newMessageText;
            set
            {
                if (_newMessageText != value)
                {
                    _newMessageText = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SendMessageCommand { get; set; }

        public ChatViewModel()
        {
            this.SendMessageCommand = new Command(SendUserMessageAsync);
            this.Messages = new ObservableCollection<Message>();

            this._settingsViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
        }

        /// <summary>
        /// Este método se encarga de enviar
        /// el mensaje escrito por el usuario
        /// al modelo para que infiera que tool
        /// ejecutar.
        /// </summary>
        private async void SendUserMessageAsync()
        {
            if (String.IsNullOrWhiteSpace(this.NewMessageText)) return;
            this.Messages.Add
            (
                new Message
                {
                    Text = this.NewMessageText,
                    IsCurrentUser = true
                }
            );
            await this._settingsViewModel.SendMessageToLLMAsync(this.NewMessageText);
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
