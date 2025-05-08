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
        public static List<Button> GetButtonReferences(List<string> references)
        {
            List<Button> buttons = new List<Button>();

            foreach(string key in references)
            {
                Button button = new Button
                {
                    Text = key,
                    BackgroundColor = new Color(0,122,255),
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
