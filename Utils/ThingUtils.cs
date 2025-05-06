using CommunityToolkit.Maui.Alerts;

namespace MauiApp_IA_IOT.Util
{
    public class ThingsUtils
    {
        /// <summary>
        /// Este método se encarga de mostrar mensajes
        /// a partir del Sankbar
        /// </summary>
        /// <param name="message"></param>
        public static void SendSnakbarMessage(string message)
        {
            Task.Run(async () =>
            {
                await Snackbar.Make(message).Show();
            });
        }

        /// <summary>
        /// Este método se encarga de volver una url resultado
        /// de la concatenación de los string que recibe por parámtro
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUrl(string protocol, string ip, string port, string? path)
        {
            return $"{protocol}://{ip}:{port}{(!string.IsNullOrWhiteSpace(path) ? path : "")}";
        }
    }
}