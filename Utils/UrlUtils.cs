namespace MauiApp_IA_IOT.Util
{
    public static class UrlUtils
    {
        /// <summary>
        /// Este método se encarga de volver la base url resultado
        /// de la concatenación de los string que recibe por parámtro
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string GetBaseUrl(string f1api, string pathApi)
        {
            return $"{f1api}{pathApi}";
        }

        /// <summary>
        /// Este método se encarga de volver la url completa resultado
        /// de la concatenación de los string que recibe por parámtro
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullUrl(string baseUrl, string path)
        {
            return $"{baseUrl}{(!string.IsNullOrWhiteSpace(path) ? path : "")}";
        }
    }
}