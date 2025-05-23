namespace efetch.Services
{
    public interface IEfetch
    {
        Task<T> GetAsync<T>(string endpointUrl, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
        Task<T> PostAsync<T, TBody>(string endpointUrl, TBody? body, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
        Task<T> PutAsync<T, TBody>(string endpointUrl, TBody? body, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
        Task<T> PatchAsync<T, TBody>(string endpointUrl, TBody? body, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
        Task<T> DeleteAsync<T>(string endpointUrl, dynamic? id = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
    }
}
