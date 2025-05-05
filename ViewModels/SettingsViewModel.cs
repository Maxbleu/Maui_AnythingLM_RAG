using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Brush _colorAnyThingLM = Brush.Red;
        private string _protocol = "http";
        private string _host = "localhost";
        private string _port = "3000";
        private string _apiKey = "";

        private bool _isEnabledButton = false;
        private bool _isAnyThingLMRunning = false;

        public Brush ColorAnyThingLM
        {
            get => this._colorAnyThingLM;
            set
            {
                if (this._colorAnyThingLM != value)
                {
                    this._colorAnyThingLM = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Protocol
        {
            get => this._protocol;
            set
            {
                if (this._protocol != value)
                {
                    this._protocol = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Host
        {
            get => this._host;
            set
            {
                if (this._host != value)
                {
                    this._host = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Port
        {
            get => this._port;
            set
            {
                if (this._port != value)
                {
                    this._port = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsEnabledButton
        {
            get => this._isEnabledButton;
            set
            {
                if (this._isEnabledButton != value)
                {
                    this._isEnabledButton = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsAnyThingLMRunning
        {
            get => this._isAnyThingLMRunning;
            set
            {
                if (this._isAnyThingLMRunning != value)
                {
                    this._isAnyThingLMRunning = value;
                    OnPropertyChanged();
                }

                this.ColorAnyThingLM = this.IsAnyThingLMRunning ? Brush.Green : Brush.Red;
            }
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
