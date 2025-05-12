namespace MauiApp_AnyThingLM_RAG.Models
{
    public class Workspace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public object VectorTag { get; set; }
        public DateTime CreatedAt { get; set; }
        public double OpenAiTemp { get; set; }
        public int OpenAiHistory { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string OpenAiPrompt { get; set; }
        public double SimilarityThreshold { get; set; }
        public string ChatProvider { get; set; }
        public string ChatModel { get; set; }
        public int TopN { get; set; }
        public string ChatMode { get; set; }
        public string PfpFilename { get; set; }
        public string AgentProvider { get; set; }
        public string AgentModel { get; set; }
        public string QueryRefusalResponse { get; set; }
        public string VectorSearchMode { get; set; }
        public List<Document> Documents { get; set; }
        public List<Thread> Threads { get; set; }
    }
}
