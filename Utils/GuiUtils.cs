using CommunityToolkit.Maui.Alerts;

namespace MauiApp_AnyThingLM_RAG.Utils
{
    public static class GuiUtils
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
        /// Este método se encarga de 
        /// volver a la página anterior
        /// en la que se encontraba el usuario
        /// </summary>
        public async static void GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
        /// <summary>
        /// Este método se encarga de mostrar en una
        /// página en específico una alerta y
        /// dependiendo de lo que elija el usuario
        /// se devolverá un true o false
        /// </summary>
        /// <param name="page"></param>
        /// <param name="titulo"></param>
        /// <param name="message"></param>
        /// <param name="textOkButton"></param>
        /// <param name="textCancelButton"></param>
        /// <returns></returns>
        public async static Task<bool> DisplayAlertAsync(Page page, string titulo, string message, string textOkButton, string textCancelButton)
        {
            return await page.DisplayAlert(
                titulo,
                message,
                textOkButton,
                textCancelButton
            );
        }
        /// <summary>
        /// Este método se encarga de mostrar un prompt alert
        /// para obtener inputs del usuario
        /// </summary>
        /// <param name="page"></param>
        /// <param name="titulo"></param>
        /// <param name="message"></param>
        /// <param name="textOkButton"></param>
        /// <param name="textCancelButton"></param>
        /// <returns></returns>
        public async static Task<string> DisplayPromptAlertAsync(Page page, string titulo, string message, string textOkButton, string textCancelButton)
        {
            return await page.DisplayPromptAsync(
                titulo,
                message,
                accept: textOkButton,
                cancel: textCancelButton
            );
        }
        public static List<Button> GetButtonReferences(List<string> references)
        {
            List<Button> buttons = new List<Button>();

            foreach (string key in references)
            {
                Button button = new Button
                {
                    Text = key,
                    BackgroundColor = new Color(0, 122, 255),
                    TextColor = Colors.White,
                    BorderColor = Colors.Transparent,
                    HorizontalOptions = LayoutOptions.Start,
                    BorderWidth = 0,
                    CornerRadius = 30
                };

                button.Clicked += (s, e) =>
                {
                    //  Mostrar el mensaje
                    SendSnakbarMessage($"Referencia: {key}");
                };
            }

            return buttons;
        }
    }
}
