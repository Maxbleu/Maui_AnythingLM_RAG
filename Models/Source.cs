namespace MauiApp_AnyThingLM_RAG.Models
{
    public class Source
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string DocAuthor { get; set; }
        public string Description { get; set; }
        public string DocSource { get; set; }
        public string ChunkSource { get; set; }
        public string Published { get; set; }
        public int WordCount { get; set; }
        public int TokenCountEstimate { get; set; }
        public string Text { get; set; }
        public double _Distance { get; set; }
        public double Score { get; set; }
    }
}
