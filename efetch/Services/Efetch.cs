using efetch.Configurations;
using efetch.Policies;
using efetch.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace efetch.Services
{
    public class Efetch : IEfetch
    {
        private readonly EfetchConfig _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConsoleLoggingProvider _loggingProvider;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

        public Efetch(IHttpClientFactory httpClientFactory, IOptions<EfetchConfig> configOptions, IConsoleLoggingProvider loggingProvider)
        {
            _config = configOptions?.Value ?? throw new ArgumentNullException(nameof(configOptions));

            _httpClientFactory = httpClientFactory;

            var httpClientFactoryWithOptions = httpClientFactory;

            _loggingProvider = loggingProvider;

            var httpClient = httpClientFactoryWithOptions.CreateClient();

            foreach (var header in _config.DefaultHeaders)
            {
                if (!string.IsNullOrWhiteSpace(header.Key) && !string.IsNullOrWhiteSpace(header.Value))
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            // Configure Polly retry policy using EfetchConfig settings
            _retryPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                           .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                           .WaitAndRetryAsync(_config.RetryCount, _config.RetryInterval);
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

                    var responseData = await response.Content.ReadAsStringAsync();

                    response.EnsureSuccessStatusCode();

                    _loggingProvider?.LogResponse(response);

                    try
                    {
                        if (typeof(T) == typeof(string))
                        {
                            return (T)(object)responseData;
                        }
                        else if (typeof(T) == typeof(object))
                        {
                            return (T)(object)responseData;
                        }

                        var result = JsonSerializer.Deserialize<T>(responseData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return result!;
                    }
                    catch (JsonException ex)
                    {
                        _loggingProvider?.LogError(ex);
                        throw;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _loggingProvider?.LogError(ex);
                throw;
            }
        }
        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(string endpointUrl, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            string url = CombineUrls(endpointUrl, id, queryParams);
            var request = PrepareRequest(HttpMethod.Get, url, null, headers!);
            return await SendAsync<T>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<T> PostAsync<T, TBody>(string endpointUrl, TBody? body, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            string url = CombineUrls(endpointUrl, id, queryParams);
            var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowercaseNamingPolicy(),
            });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var request = PrepareRequest(HttpMethod.Post, url, content, headers!);
            return await SendAsync<T>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<T> PutAsync<T, TBody>(string endpointUrl, TBody? body, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
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
        public async Task<T> PatchAsync<T, TBody>(string endpointUrl, TBody? body, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
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
        public async Task<T> DeleteAsync<T>(string endpointUrl, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
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
