using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp_AnyThingLM_RAG.Managers;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Utils;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class WorkspaceDocumentsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private AnyThingLLManager _anyThingLLManager;
        private ObservableCollection<Metadata> _documents;
        private string _slug;

        public ObservableCollection<Metadata> Documents 
        {
            get => this._documents;
            set
            {
                if(this._documents != value)
                {
                    this._documents = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Slug
        {
            get => this._slug;
            set
            {
                if(this._slug != value)
                {
                    this._slug = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand UploadDocumentCommand { get; }

        public WorkspaceDocumentsViewModel(List<Metadata> documents, string slug)
        {
            this.Slug = slug;
            this.Documents = new ObservableCollection<Metadata>(documents);

            this.UploadDocumentCommand = new Command(UploadDocumentAsync);

            this._anyThingLLManager = IPlatformApplication.Current.Services.GetService<SettingsViewModel>().AnyThingLLManager;
        }

        /// <summary>
        /// Este método se encarga de enviar un documento
        /// al workspace seleccionado por el usuario a AnyThingLLM
        /// </summary>
        private async void UploadDocumentAsync()
        {
            string message = "";
            dynamic objResult = await this._anyThingLLManager.TakeDocumentAsync(this.Slug);
            if (objResult.GetType().GetProperty("Response") != null)
            {
                message = objResult.Response.Message;
                await Task.Delay(2000);

                dynamic documents = await this._anyThingLLManager.TakeWorkspaceDocumentsAsync(this.Slug);
                if (documents.GetType().GetProperty("Data") != null)
                {
                    this.Documents = new ObservableCollection<Metadata>(documents.Data);
                }
                else
                {
                    message = objResult.Error.Message;
                }
            }
            else
            {
                message = objResult.Error.Message;
            }
            GuiUtils.SendSnakbarMessage(message);
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
