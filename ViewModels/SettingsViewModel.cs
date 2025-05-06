using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using MauiApp_IA_IOT.Util;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Brush _colorAnyThingLM = Brush.Red;
        private string _protocol = "http";
        private string _host = "";
        private string _port = "3001";
        private string _apiKey = "";

        private bool _isEnabledButton = true;
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
        public string ApiKey
        {
            get => this._apiKey;
            set
            {
                if (this._apiKey != value)
                {
                    this._apiKey = value;
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
                this.IsEnabledButton = this.IsAnyThingLMRunning ? false : true;
            }
        }

        public Command ReloadConfigurationCommand { get; set; }

        public SettingsViewModel()
        {
            this.ReloadConfigurationCommand = new Command(ReloadConfigurationAsync);

            this.LoadAnyThingLMAsync();
        }

        /// <summary>
        /// Este método realiza una llamada al método de recarga
        /// de la conexión con la api de AnyThingLLM
        /// </summary>
        public async void ReloadConfigurationAsync()
        {
            await this.LoadAnyThingLMAsync();
        }
        /// <summary>
        /// Este método se encarga de cargar
        /// la conexión con la api de AnyThingLLM
        /// </summary>
        /// <returns></returns>
        public async Task LoadAnyThingLMAsync()
        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.ApiKey);
                try
                {
                    string url = ThingsUtils.GetUrl(this.Protocol, this.Host, this.Port, "/api/v1/auth");
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        NotifyResponseRequestNodeRed("Se conectado con AnyThingLM", false, true);
                    }
                }
                catch (Exception ex)
                {
                    NotifyResponseRequestNodeRed("Error comunicándose con AnyThingLM: " + ex.Message, true, false);
                }
            }
        }
        /// <summary>
        /// Este método se encarga de notificar cualquier 
        /// cambio que surja en el servicio durante la
        /// ejecucción de la aplicación.
        /// </summary>
        /// <param name="mensajeSnakBar"></param>
        /// <param name="isEnabledButton"></param>
        /// <param name="isAnyThingServiceRunning"></param>
        private void NotifyResponseRequestNodeRed(string mensajeSnakBar, bool isEnabledButton, bool isAnyThingServiceRunning)
        {
            ThingsUtils.SendSnakbarMessage(mensajeSnakBar);
            this.IsEnabledButton = isEnabledButton;
            this.IsAnyThingLMRunning = isAnyThingServiceRunning;
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
