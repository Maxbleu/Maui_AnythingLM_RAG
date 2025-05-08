using MauiApp_AnyThingLM_RAG.Managers;

namespace MauiApp_AnyThingLM_RAG.Factory
{
    public static class AnyThingLLManagerFactory
    {
        public static AnyThingLLManager Create(string baseUrl, string apiKey)
        {
            return new AnyThingLLManager(baseUrl, apiKey);
        }
    }
}
