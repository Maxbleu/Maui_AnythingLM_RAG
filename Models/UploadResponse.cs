namespace MauiApp_AnyThingLM_RAG.Models
{
    public class UploadResponse
    {
        public bool success { get; set; }
        public object error { get; set; }
        public List<Source> documents { get; set; }
    }
}
