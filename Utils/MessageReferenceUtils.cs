using System.Text.RegularExpressions;

namespace MauiApp_AnyThingLM_RAG.Utils
{
    public static class MessageReferenceUtils
    {
        public static Dictionary<string, List<string>> GetReferenceDocument(dynamic sources)
        {
            Dictionary<string, List<string>> references = new Dictionary<string, List<string>>();
            if (sources != null && sources.Count > 0)
            {
                foreach (var source in sources)
                {
                    string text = Regex.Replace(source["text"].ToString(), @"[\r\n]+", " ");

                    //  Get the document
                    int startIndex = text.IndexOf("sourceDocument: ") + "sourceDocument: ".Length;
                    int endIndex = text.IndexOf(" published:");
                    string sourceDocument = text.Substring(startIndex, endIndex - startIndex);

                    //  Get the reference
                    int indexOfStart = text.IndexOf("</document_metadata> ") + "</document_metadata> ".Length;
                    string reference = text.Substring(indexOfStart).Trim();

                    //  Add the reference to the dictionary
                    if (!references.ContainsKey(sourceDocument))
                    {
                        references[sourceDocument] = new List<string>();
                    }

                    references[sourceDocument].Add(reference);
                }
            }

            return references;
        }
    }
}
