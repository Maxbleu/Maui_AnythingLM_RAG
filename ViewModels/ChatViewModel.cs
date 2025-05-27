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
        private ChatSettingsViewModel _chatSettingsViewModel;
        private ObservableCollection<dynamic> _messages;
        private AnyThingLLManager _anyThingLLManager;

        private string _newMessageText = "";
        private string _theardName;
        private string _chatMode = "Chat";
        private string _slug;

        public ObservableCollection<dynamic> Messages 
        {
            get => _messages;
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged();
                }
            }
        }
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

            this.Messages = new ObservableCollection<dynamic>();

            this.SendMessageCommand = new Command(SendMessageAsync);
            this.NavigateToChatSettingsCommand = new Command(NavigateToChatSettingsAsync);
            this.NavigateToWorkspaceDocumentsCommand = new Command(NavigateToWorkspaceDocumentsAsync);

            this._chatSettingsViewModel = IPlatformApplication.Current.Services.GetService<ChatSettingsViewModel>();
            this._anyThingLLManager = IPlatformApplication.Current.Services.GetService<SettingsViewModel>().AnyThingLLManager;

            this.LoadConversation();
        }
        public ChatViewModel() { }

        /// <summary>
        /// Este método se encarga de obtener
        /// los mensajes de la conversación
        /// y cargarlos en la parte gráfica.
        /// </summary>
        private async void LoadConversation()
        {
            string threadSlug = this._anyThingLLManager.WorkspaceRoot.Workspaces.SelectMany(workspace => workspace.Threads).FirstOrDefault(thread => thread.Name == this.ThreadName)?.Slug;

            ConversationHistory conversationHistory = await this._anyThingLLManager.GetThreadMessagesAsync(this.Slug, threadSlug);

            if(conversationHistory.History.Count > 0)
            {
                this.Messages = new ObservableCollection<dynamic>();
                foreach(MessageItem item in conversationHistory.History)
                {
                    this.Messages.Add(
                        new {
                            Text = item.Content,
                            IsCurrentUser = item.Role == "user" ? true : false,
                            Keys = new ObservableCollection<string>(item.Role == "user" ? new List<string>() : MessageReferenceUtils.GetReferenceDocument(item.Sources).Keys.ToList())
                        }
                    );
                }
            }
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
            var objResult = await this._anyThingLLManager.SendMessageAsync(this.NewMessageText, this._chatSettingsViewModel.SystemPrompt, this._chatSettingsViewModel.Temperature, this._chatSettingsViewModel.MaxTokens, this.ChatMode, this.Slug);
            if(objResult.GetType().GetProperty("Data") != null)
            {

                //  Enviar el mensaje al chat
                string text = objResult.Data.Text.ToString();
                this.References = (Dictionary<string, List<string>>)objResult.Data.Refs;
                var keys = new ObservableCollection<string>(this.References.Keys);
                this.Messages.Add
                (
                    new 
                    { 
                        Text = text, 
                        Keys = keys,
                        IsCurrentUser = false 
                    }
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
        /// <summary>
        /// Este método se encarga navegar desde la pagina
        /// de chat hasta la pagina de settings del chat del
        /// modelo.
        /// </summary>
        private async void NavigateToChatSettingsAsync()
        {
            await Shell.Current.Navigation.PushAsync(IPlatformApplication.Current.Services.GetService<ChatSettingsPage>());
        }
        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
