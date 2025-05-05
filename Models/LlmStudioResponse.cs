namespace MauiApp_AnyThingLM_RAG.Models
{
    public class LlmStudioResponse
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public bool Close { get; set; }
        public object Error { get; set; }
        public int ChatId { get; set; }
        public string TextResponse { get; set; }
        public List<Source> Sources { get; set; }
        public Metrics Metrics { get; set; }
    }
}
