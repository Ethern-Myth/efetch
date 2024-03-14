# efetch

**efetch** is a lightweight C# library for making HTTP requests with ease. It provides a simple interface for performing common HTTP operations such as GET, POST, PUT, PATCH, and DELETE. It also supports custom logging for requests, responses, and errors.

![NuGet Version](https://img.shields.io/nuget/v/efetch)

![GitHub Tag](https://img.shields.io/github/v/tag/ethern-myth/efetch)

![NuGet Downloads](https://img.shields.io/nuget/dt/efetch)

### Installation

You can install **efetch** via NuGet Package Manager Console:

```bash
Install-Package efetch
```

Or via .NET CLI:

```bash
dotnet add package efetch
```

### Features

- Simple and intuitive API for making HTTP requests.
- Support for GET, POST, PUT, PATCH, and DELETE operations.
- Ability to customize default headers for requests.
- Support for query parameters.
- Lightweight and easy to integrate into existing projects.

### API

#### `IEFetchClient`

- `Task<T> GetAsync<T>(string endpointUrl, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)`: Performs a GET request.
- `Task<T> PostAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default)`: Performs a POST request.
- `Task<T> PutAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)`: Performs a PUT request.
- `Task<T> PatchAsync<T, TBody>(string endpointUrl, TBody body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)`: Performs a PATCH request.
- `Task<T> DeleteAsync<T>(string endpointUrl, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default, dynamic? id = null)`: Performs a DELETE request.

#### `ILoggingProvider`

- `void LogRequest(HttpRequestMessage request)`: Logs the HTTP request.
- `void LogResponse(HttpResponseMessage response)`: Logs the HTTP response.
- `void LogError(Exception exception)`: Logs errors that occur during HTTP requests.

### Configuration

To use **efetch**, first configure the `EfetchConfig` class with the desired base URL and default headers.

```csharp
var config = new EfetchConfig
{
    BaseUrl = "https://api.example.com",
    DefaultHeaders = new Dictionary<string, string>
    {
        { "Authorization", "Bearer your_access_token" },
        { "Accept", "application/json" }
    }
};
```

### Usage

You can integrate **efetch** with .NET Core in your `Program.cs` or just .cs file like the example below:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using efetch;

namespace YourNamespace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            builder.Services.AddSingleton<EfetchConfig>(new EfetchConfig
            {
                BaseUrl = "https://api.example.com",
                DefaultHeaders = new Dictionary<string, string>
                {
                    { "Authorization", "Bearer your_access_token" },
                    { "Accept", "application/json" }
                }
            });
            // Add other services...

            var app = builder.Build();

            // Pass the configured IServiceCollection to InstanceConfig method
            var efetchInstance = Efetch.InstanceConfig(builder.Services);

            // Run the application
            app.Run();
        }
    }
}
```

On a .NET Core MVC

```csharp
builder.Services.AddTransient<IEfetch, Efetch>();
builder.Services.AddSingleton<EfetchConfig>(new EfetchConfig { BaseUrl = "http://127.0.0.1" });

// This is important when using Program.cs
new EfetchConfig(builder.Services);
```

### Example

```csharp
var config = new EfetchConfig
{
    BaseUrl = "https://api.example.com",
    DefaultHeaders = new Dictionary<string, string>
    {
        { "Authorization", "Bearer your_access_token" },
        { "Accept", "application/json" }
    },
    RetryCount = 5,
    RetryInterval = retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
};

var efetch = Efetch.InstanceConfig(config);

// GET request example
var responseData = await efetch.GetAsync<ResponseModel>("/endpoint");

// POST request example
var requestBody = new RequestBody { /* request body properties */ };
var response = await efetch.PostAsync<ResponseModel, RequestBody>("/endpoint", requestBody);
```

In this example:

- We first create an instance of `Efetch` without any logging, default is set for us.
- We perform a GET request using the first client instance and display the result.
- We perform a POST request using the second client instance with console logging and display the result.
- Similarly, other HTTP operations like PUT, PATCH, and DELETE can be performed using their respective methods.

### Author

**efetch** is developed and maintained by [Ethern Myth](https://github.com/Ethern-Myth).

### License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
