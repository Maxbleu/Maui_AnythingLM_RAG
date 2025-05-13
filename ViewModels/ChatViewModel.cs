using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp_AnyThingLM_RAG.Factory;
using MauiApp_AnyThingLM_RAG.Managers;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Utils;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SettingsViewModel _settingsViewModel;
        private AnyThingLLManager _anyThingLLManager;

        private ChatPage _chatPage;

        private string _newMessageText = "";
        private string _chatMode = "Chat";
        private string _slug;
        private string _theardName;

        public ObservableCollection<dynamic> Messages { get; set; }
        public Dictionary<string, List<string>> References { get; set; }

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
        public string ThreadName
        {
            get => this._theardName;
            set
            {
                if(this._theardName != value)
                {
                    this._theardName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ChatMode
        {
            get => this._chatMode;
            set
            {
                if (this._chatMode != value)
                {
                    this._chatMode = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Slug
        {
            get => this._slug;
            set
            {
                if (this._slug != value)
                {
                    this._slug = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand NavigateToWorkspaceDocumentsCommand { get; }
        public ICommand ShowReferencesMetaDocumentCommand { get; }
        public ICommand NavigateToChatSettingsCommand { get; }
        public ICommand SendMessageCommand { get; }
        
        public ChatViewModel(string threadName, string slug)
        {
            this.Slug = slug;
            this.ThreadName = threadName;

            this.SendMessageCommand = new Command(SendMessageAsync);
            this.NavigateToChatSettingsCommand = new Command(NavigateToChatSettingsAsync);
            this.ShowReferencesMetaDocumentCommand = new Command(ShowReferencesMetaDocument);
            this.NavigateToWorkspaceDocumentsCommand = new Command(NavigateToWorkspaceDocumentsAsync);

            this.Messages = new ObservableCollection<dynamic>();

            this._settingsViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
            this._anyThingLLManager = this._settingsViewModel.AnyThingLLManager;
        }
        public ChatViewModel() { }

        /// <summary>
        /// Este método se encarga de mostrar las referencias
        /// que ha encontrado el modelo en el documento para
        /// realizar el mensaje que ha recibido el usuario
        /// </summary>
        /// <param name="sender"></param>
        private void ShowReferencesMetaDocument(object sender)
        {
            //  Obtener las referencias del documento


            //  Mostrar las referencias en la vista
        }
        /// <summary>
        /// Este método se encarga de enviar
        /// el mensaje escrito por el usuario
        /// al modelo
        /// </summary>
        private async void SendMessageAsync()
        {
            if(String.IsNullOrWhiteSpace(this.NewMessageText)) return;

            //  Enviar el mensaje al chat
            this.Messages.Add
            (
                new
                {
                    Text = this.NewMessageText,
                    IsCurrentUser = true
                }
            );

            //  Enviar el mensaje al modelo
            var objResult = await this._anyThingLLManager.SendMessageAsync(this.NewMessageText, this._settingsViewModel.SystemPrompt, this.ChatMode, this.Slug);
            if(objResult.GetType().GetProperty("Data") != null)
            {

                //  Enviar el mensaje al chat
                string text = objResult.Data.Text.ToString();
                this.References = (Dictionary<string, List<string>>)objResult.Data.Refs;
                var keys = new ObservableCollection<string>(this.References.Keys);
                this.Messages.Add
                (
                    new Message(text, keys, false)
                );
            }
            else
            {
                GuiUtils.SendSnakbarMessage(objResult.Error.Message);
            }

            this.NewMessageText = String.Empty;
        }
        /// <summary>
        /// Este método se encarga de obtener, 
        /// los documentos y mostrarlos en la pantalla
        /// </summary>
        private async void NavigateToWorkspaceDocumentsAsync()
        {
            dynamic objResult = await this._anyThingLLManager.TakeWorkspaceDocumentsAsync(this.Slug);

            if (objResult.GetType().GetProperty("Data") != null)
            {
                WorkspaceDocumentsViewModel workspaceDocumentsViewModel = WorkspaceDocumentsViewModelFactory.Create(objResult.Data, this.Slug);
                await Shell.Current.Navigation.PushAsync(WorkspaceDocumentsPageFactory.Create(workspaceDocumentsViewModel));
            }
            else
            {
                GuiUtils.SendSnakbarMessage(objResult.Error.ToString());
            }
        }
        private async void NavigateToChatSettingsAsync()
        {

        }
        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
