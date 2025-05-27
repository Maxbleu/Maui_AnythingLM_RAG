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
        private ObservableCollection<Source> _documents = new ObservableCollection<Source>();
        private string _slug;

        public ObservableCollection<Source> Documents 
        {
            get => this._documents;
            set
            {
                if(this._documents != value)
                {
                    this._documents = value;
                    OnPropertyChanged(nameof(Documents));
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

        public WorkspaceDocumentsViewModel(List<Source> documents, string slug)
        {
            this.Slug = slug;
            this.AddNewDocuments(documents);

            this.UploadDocumentCommand = new Command(UploadDocumentAsync);

            this._anyThingLLManager = IPlatformApplication.Current.Services.GetService<SettingsViewModel>().AnyThingLLManager;
        }

        public WorkspaceDocumentsViewModel() { }

        /// <summary>
        /// Este método se encarga de enviar un documento
        /// al workspace seleccionado por el usuario a AnyThingLLM
        /// </summary>
        private async void UploadDocumentAsync()
        {
            Source source = await this._anyThingLLManager.TakeDocumentAsync(this.Slug);
            GuiUtils.SendSnakbarMessage("Se ha obtenido el documento");
            await Task.Delay(10000);
            if (source != null)
            {
                List<Source> sources = this.Documents.ToList<Source>();
                sources.Add(source);

                AddNewDocuments(sources);
            }
        }

        private void AddNewDocuments(List<Source> documents)
        {
            if (documents != null && documents.Count > 0)
            {
                this.Documents.Clear();
                foreach (Source document in documents)
                {
                    this.Documents.Add(document);
                }
            }
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
