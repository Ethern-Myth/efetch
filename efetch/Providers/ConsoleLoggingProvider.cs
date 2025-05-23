namespace efetch.Providers
{
    /// <summary>
    /// Provides logging functionality for HTTP requests and responses.
    /// </summary>
    public class ConsoleLoggingProvider : IConsoleLoggingProvider
    {
        public ConsoleLoggingProvider() { }

        public void LogRequest(HttpRequestMessage request)
        {
            Console.WriteLine($"Request: {request.Method} {request.RequestUri}");
            Console.WriteLine("Headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }
            Console.WriteLine();
        }

        public void LogResponse(HttpResponseMessage response)
        {
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine("Headers:");
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }
            Console.WriteLine();
        }

        public void LogError(Exception exception)
        {
            Console.WriteLine($"Error: {exception.Message}");
            Console.WriteLine($"StackTrace: {exception.StackTrace}");
            Console.WriteLine();
        }
    }
}
