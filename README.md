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
    }
};

var efetch = Efetch.InstanceConfig(config);

// GET request example
var responseData = await efetch.GetAsync<ResponseModel>("/endpoint");

// POST request example
var requestBody = new RequestBody { /* request body properties */ };
var response = await efetch.PostAsync<ResponseModel, RequestBody>("/endpoint", requestBody);
```

In this example:

- We first create an instance of `Efetch` without any logging.
- Then, we create another instance of `Efetch` with console logging enabled by passing `ConsoleLoggingProvider.Instance` to the `InstanceWithLoggingProvider` method.
- We perform a GET request using the first client instance and display the result.
- We perform a POST request using the second client instance with console logging and display the result.
- Similarly, other HTTP operations like PUT, PATCH, and DELETE can be performed using their respective methods.

### Author

**efetch** is developed and maintained by [Ethern Myth](https://github.com/Ethern-Myth).

### License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

---------------------------------------------------------------------------------------

Sure, here's how you can update the README.md file with the latest change:

---

# efetch

efetch is a lightweight library for making HTTP requests and logging. It provides an easy-to-use interface for sending HTTP requests asynchronously and handling responses.

## Usage

### Installation

You can install efetch via NuGet Package Manager Console:

```bash
Install-Package efetch
```

Or via .NET CLI:

```bash
dotnet add package efetch
```

### Integration with ASP.NET Core

You can integrate efetch with ASP.NET Core in your `Program.cs` file:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

In this configuration, ensure you replace `"https://api.example.com"` and `"your_access_token"` with your actual API base URL and access token respectively.

### Making Requests

Create an instance of `Efetch` using the configuration:

```csharp
var efetch = Efetch.InstanceConfig(config);
```

Now you can make HTTP requests using the provided methods:

```csharp
// GET request
var responseData = await efetch.GetAsync<ResponseModel>("/endpoint");

// POST request
var response = await efetch.PostAsync<ResponseModel, RequestBody>("/endpoint", requestBody);

// PUT request
var response = await efetch.PutAsync<ResponseModel, RequestBody>("/endpoint", requestBody);

// PATCH request
var response = await efetch.PatchAsync<ResponseModel, RequestBody>("/endpoint", requestBody);

// DELETE request
var response = await efetch.DeleteAsync<ResponseModel>("/endpoint");
```

### Logging

efetch supports logging requests and responses. You can provide your own logging provider by implementing the `ILoggingProvider` interface.

```csharp
public class MyLoggingProvider : ILoggingProvider
{
    public void LogRequest(HttpRequestMessage request)
    {
        // Implement logging logic for request
    }

    public void LogResponse(HttpResponseMessage response)
    {
        // Implement logging logic for response
    }

    public void LogError(Exception ex)
    {
        // Implement logging logic for errors
    }
}
```

### Examples

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

var efetch = Efetch.InstanceConfig(config);

// GET request example
var responseData = await efetch.GetAsync<ResponseModel>("/endpoint");

// POST request example
var requestBody = new RequestBody { /* request body properties */ };
var response = await efetch.PostAsync<ResponseModel, RequestBody>("/endpoint", requestBody);
```

---

This README provides a brief overview of efetch, covering its installation, configuration, integration with ASP.NET Core, usage, logging, and examples. For more detailed information, please refer to the code documentation or visit the [official documentation](https://github.com/your_repository).

Feel free to contribute, report issues, or suggest improvements by [opening an issue](https://github.com/your_repository/issues) or [creating a pull request](https://github.com/your_repository/pulls).
