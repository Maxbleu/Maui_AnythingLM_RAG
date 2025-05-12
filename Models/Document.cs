namespace MauiApp_AnyThingLM_RAG.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string DocId { get; set; }
        public string Filename { get; set; }
        public string Docpath { get; set; }
        public int WorkspaceId { get; set; }
        public string Metadata { get; set; }
        public bool Pinned { get; set; }
        public bool Watched { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
