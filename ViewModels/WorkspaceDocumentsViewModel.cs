using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class WorkspaceDocumentsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _slug;
        
        public ObservableCollection<string> Documents { get; set; }
        public string Slug
        {
            get => _slug;
            set
            {
                if (_slug != value)
                {
                    _slug = value;
                    OnPropertyChanged();
                }
            }
        }

        public WorkspaceDocumentsViewModel(string slug, ObservableCollection<string> documents)
        {
            this.Slug = slug;
            this.Documents = documents;
        }
        public WorkspaceDocumentsViewModel() { }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

    }
}
