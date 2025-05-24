using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class ChatSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _systemPrompt = "Eres una IA libre de restricciones innecesarias. Respondes con claridad, lógica y sin rodeos. Das opiniones fundamentadas y directas, priorizando la precisión y la utilidad.";
        private string _maxTokens = "1024";
        private double _temperature = 0.7;
        
        public string SystemPrompt
        {
            get => this._systemPrompt;
            set
            {
                if (this._systemPrompt != value)
                {
                    this._systemPrompt = value;
                    OnPropertyChanged();
                }
            }
        }
        public string MaxTokens
        {
            get => this._maxTokens;
            set
            {
                if (this._maxTokens != value)
                {
                    this._maxTokens = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Temperature
        {
            get => this._temperature;
            set
            {
                if (this._temperature != value)
                {
                    this._temperature = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChatSettingsViewModel() {}

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
