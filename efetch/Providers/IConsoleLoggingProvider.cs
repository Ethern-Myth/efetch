namespace efetch.Providers
{
    public interface IConsoleLoggingProvider
    {
        void LogRequest(HttpRequestMessage request);
        void LogResponse(HttpResponseMessage response);
        void LogError(Exception exception);
    }
}
