namespace MauiApp_AnyThingLM_RAG.Models
{
    public class Metrics
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public double OutputTps { get; set; }
        public double Duration { get; set; }
    }
}
