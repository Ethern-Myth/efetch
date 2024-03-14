using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using System.Text;
using System.Text.Json;

namespace efetch
{
    /// <summary>
    /// Represents configuration properties for the Efetch class.
    /// </summary>
    public class EfetchConfig
    {
        public string BaseUrl { get; set; } = "";
        public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();

        // Polly retry policy configuration

        // Default retry count
        public int RetryCount { get; set; } = 3; 
        // Default retry interval calculation
        public Func<int, TimeSpan> RetryInterval { get; set; } = retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)); 
    }

    /// <summary>
    /// Provides methods for making HTTP requests and logging.
    /// </summary>
    public class Efetch : IEfetch
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EfetchConfig _config;
        private readonly ILoggingProvider _loggingProvider;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        public string BaseUrl => _config.BaseUrl;

        /// <summary>
        /// Initializes a new instance of the Efetch class with the specified configuration.
        /// </summary>
        private Efetch(IHttpClientFactory httpClientFactory, EfetchConfig config, ILoggingProvider? loggingProvider = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _loggingProvider = loggingProvider!;

            // Create HttpClient instance through IHttpClientFactory
            var httpClient = _httpClientFactory.CreateClient();

            // Set default headers for the HttpClient instance
            foreach (var header in _config.DefaultHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // Configure Polly retry policy using EfetchConfig settings
            _retryPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                           .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                           .WaitAndRetryAsync(_config.RetryCount, _config.RetryInterval);
        }

        /// <summary>
        /// Creates an instance of Efetch with the specified configuration.
        /// </summary>
        public static Efetch InstanceConfig(EfetchConfig? efetchConfig = null, ILoggingProvider? loggingProvider = null)
        {
            if (loggingProvider == null)
            {
                loggingProvider = ConsoleLoggingProvider.Instance;
            }
            // Set up service collection for dependency injection
            var serviceProvider = new ServiceCollection()
                                        .AddHttpClient()
                                        .BuildServiceProvider();
            // Get the HttpClientFactory service
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            return new Efetch(httpClientFactory, efetchConfig!, loggingProvider);
        }

        /// <summary>
        /// Creates an instance of Efetch with the specified configuration and services.
        /// </summary>
        public static Efetch InstanceConfig(IServiceCollection? services = null, ILoggingProvider? loggingProvider = null)
        {
            if (loggingProvider == null)
            {
                loggingProvider = ConsoleLoggingProvider.Instance;
            }

            // Set up service collection for dependency injection with options
            var serviceProviderWithOptions = services!
                                            .AddHttpClient()
                                            .AddOptions()
                                            .BuildServiceProvider();

            // Get the EfetchConfig options from the service provider
            var options = serviceProviderWithOptions.GetRequiredService<IOptions<EfetchConfig>>();
            var httpClientFactoryWithOptions = serviceProviderWithOptions.GetRequiredService<IHttpClientFactory>();

            // Create and return a new instance of Efetch
            return new Efetch(httpClientFactoryWithOptions, options.Value, loggingProvider);
        }

        /// <summary>
        /// Prepares an HTTP request message with the specified method, URL, content, and headers.
        /// </summary>
        private HttpRequestMessage PrepareRequest(HttpMethod method, string url, HttpContent? content = null, Dictionary<string, string>? headers = null)
        {
            var request = new HttpRequestMessage(method, url) { Content = content };

            // Add headers to the request
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Log the request
            _loggingProvider?.LogRequest(request);
            return request;
        }

        /// <summary>
        /// Sends an HTTP request asynchronously and returns the response.
        /// </summary>
        private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var response = await _retryPolicy.ExecuteAsync(() => httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken));
                    // Read the response content
                    var responseData = await response.Content.ReadAsStringAsync();

                    // Ensure the request was successful
                    response.EnsureSuccessStatusCode();

                    // Log the response
                    _loggingProvider?.LogResponse(response);

                    // Deserialize the response data
                    return JsonSerializer.Deserialize<T>(responseData)!;
                }
            }
            catch (HttpRequestException ex)
            {
                // Log any exceptions
                _loggingProvider?.LogError(ex);
                throw;
            }
        }
        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(string endpointUrl, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)
        {
            string url = CombineUrls(endpointUrl, id, queryParams);
            var request = PrepareRequest(HttpMethod.Get, url, null, headers!);
            return await SendAsync<T>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<T> PostAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default)
        {
            string url = CombineUrls(endpointUrl);
            var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowercaseNamingPolicy(),
            });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var request = PrepareRequest(HttpMethod.Post, url, content, headers!);
            return await SendAsync<T>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<T> PutAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)
        {
            string url = CombineUrls(endpointUrl, id, queryParams);
            var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowercaseNamingPolicy(),
            });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var request = PrepareRequest(HttpMethod.Put, url, content, headers!);
            return await SendAsync<T>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<T> PatchAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)
        {
            string url = CombineUrls(endpointUrl, id, queryParams);
            var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowercaseNamingPolicy(),
            });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var request = PrepareRequest(HttpMethod.Patch, url, content, headers!);
            return await SendAsync<T>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<T> DeleteAsync<T>(string endpointUrl, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)
        {
            string url = CombineUrls(endpointUrl, id, queryParams);
            var request = PrepareRequest(HttpMethod.Delete, url, null, headers!);
            return await SendAsync<T>(request, cancellationToken);
        }

        /// <summary>
        /// Combines the base URL, endpoint URL, and optional query parameters into a single URL.
        /// </summary>
        private string CombineUrls(string endpointUrl, dynamic? id = null, Dictionary<string, string>? queryParams = null)
        {
            StringBuilder urlBuilder = new StringBuilder(_config.BaseUrl.TrimEnd('/'));
            urlBuilder.Append('/');
            urlBuilder.Append(endpointUrl.TrimStart('/'));

            // Add ID to the URL if provided
            if (id != null)
            {
                urlBuilder.Append('/');
                urlBuilder.Append(id.ToString());
            }

            // Add query parameters to the URL if provided
            if (queryParams != null && queryParams.Count > 0)
            {
                urlBuilder.Append('?');
                urlBuilder.Append(string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}")));
            }

            return urlBuilder.ToString();
        }
    }
}
