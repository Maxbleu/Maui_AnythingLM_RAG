using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiApp_AnyThingLM_RAG.Models
{
    public class Message
    {
        public string Text { get; set; }
        public ObservableCollection<string> Keys { get; set; }
        public bool IsCurrentUser { get; set; }

        public Message(string text, ObservableCollection<string> keys, bool isCurrentUser)
        {
            Text = text;
            Keys = keys;
            IsCurrentUser = isCurrentUser;
        }
    }
}
