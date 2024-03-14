namespace efetch
{
    public interface ILoggingProvider
    {
        void LogRequest(HttpRequestMessage request);
        void LogResponse(HttpResponseMessage response);
        void LogError(Exception exception);
    }
}
