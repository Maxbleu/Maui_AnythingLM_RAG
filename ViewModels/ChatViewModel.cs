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
        private AnyThingLLManager _anyThingLLManager;
        private SettingsViewModel _settingsViewModel;
        private ChatPage _chatPage;

        private string _newMessageText = "";
        private string _chatMode = "Chat";
        private string _slug = "my-workspace";

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

        public ICommand ShowReferencesMetaDocumentCommand { get; set; }
        public ICommand ShowWorkspaceDocumentsCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand UploadDocumentCommand { get; set; }
        public ChatViewModel()
        {
            this.SendMessageCommand = new Command(SendMessageAsync);
            this.UploadDocumentCommand = new Command(UploadDocumentAsync);
            this.ShowWorkspaceDocumentsCommand = new Command(ShowWorkspaceDocumentsAsync);
            this.ShowReferencesMetaDocumentCommand = new Command(ShowReferencesMetaDocument);
            this.Messages = new ObservableCollection<dynamic>();

            this._settingsViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
            this._settingsViewModel.PropertyChanged += _settingsViewModel_PropertyChanged;
        }

        //  EVENTOS SUBSCRITOS
        private void _settingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.AnyThingLLManager))
            {
                this._anyThingLLManager = this._settingsViewModel.AnyThingLLManager;
            }
        }

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
            var objResult = await this._anyThingLLManager.SendMessageAsync(this.NewMessageText, this.ChatMode, this.Slug);
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
        /// Este método se encarga de enviar un documento
        /// al workspace seleccionado por el usuario a AnyThingLLM
        /// </summary>
        private async void UploadDocumentAsync()
        {
            string message = "";
            dynamic objResult = await this._anyThingLLManager.TakeDocumentAsync(this.Slug);
            if(objResult.GetType().GetProperty("Response") != null)
            {
                message = objResult.Response.Message;
            }
            else
            {
                message = objResult.Error.Message;
            }
            GuiUtils.SendSnakbarMessage(message);
        }
        /// <summary>
        /// Este método se encarga de mostrar los documentos
        /// registras en el workspace seleccionado por el usuario
        /// </summary>
        private async void ShowWorkspaceDocumentsAsync()
        {
            //  Obtener los documentos del workspace
            dynamic objResult = await this._anyThingLLManager.TakeWorkspaceDocumentsAsync(this.Slug);

            //  Comprobar si se ha producido un error
            if(objResult.GetType().GetProperty("Data") != null)
            {
                ObservableCollection<string> documents = new ObservableCollection<string>((List<string>)objResult.Data);

                //  Creamos el view model de la vista
                WorkspaceDocumentsViewModel workspaceDocumentsViewModel = WorkspaceDocumentsViewModelFactory.Create(this.Slug, documents);

                //  Creamos la vista
                WorkspaceDocumentsPage workspaceDocumentsPage = WorkspaceDocumentsPageFactory.Create(workspaceDocumentsViewModel);

                //  Navegamos a la vista
                await Shell.Current.Navigation.PushAsync(workspaceDocumentsPage);
            }
            else
            {
                string errorMessage = objResult.Error.Message;
                GuiUtils.SendSnakbarMessage(errorMessage);
            }


        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
