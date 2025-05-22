namespace MauiApp_AnyThingLM_RAG.Models
{
    public class Thread
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Route { get; set; }
        public string UserId { get; set; }
        public int WorkspaceId {  get; set; }
        public DataTemplate ContentTemplate { get; set; }
    }
}
