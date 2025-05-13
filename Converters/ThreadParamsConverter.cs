using System.Globalization;

namespace MauiApp_AnyThingLM_RAG.Converters
{
    public class ThreadParametersConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Asegúrate de que tienes los valores necesarios
            if (values == null || values.Length < 2)
                return null;

            // El primer valor debería ser el nombre del thread
            string threadName = values[0]?.ToString();

            // El segundo valor debería ser el slug del workspace
            string workspaceSlug = values[1]?.ToString();

            // Crear un objeto anónimo con los parámetros necesarios
            return new
            {
                ThreadName = threadName,
                WorkspaceSlug = workspaceSlug
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // No necesitamos implementar esto para nuestro caso de uso
            throw new NotImplementedException();
        }
    }
}
