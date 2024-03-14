namespace efetch
{
    public interface IEfetch
    {
        Task<T> GetAsync<T>(string endpointUrl, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null);
        Task<T> PostAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default);
        Task<T> PutAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null);
        Task<T> PatchAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null);
        Task<T> DeleteAsync<T>(string endpointUrl, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null);
    }
}
