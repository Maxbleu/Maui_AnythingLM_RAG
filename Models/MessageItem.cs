namespace MauiApp_AnyThingLM_RAG.Models
{
    public class MessageItem
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public long SentAt { get; set; }
        public List<object> Attachments { get; set; }
        public int ChatId { get; set; }
        public string Type { get; set; }
        public List<Source> Sources { get; set; }
        public object FeedbackScore { get; set; }
        public Metrics Metrics { get; set; }
    }
}
