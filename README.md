# efetch

**efetch** is a lightweight C# library for performing resilient HTTP requests with minimal boilerplate. It supports GET, POST, PUT, PATCH, DELETE operations, built-in retry policies via Polly, customizable headers, and optional structured logging.

![NuGet Version](https://img.shields.io/nuget/v/efetch)
![GitHub Tag](https://img.shields.io/github/v/tag/ethern-myth/efetch)
![NuGet Downloads](https://img.shields.io/nuget/dt/efetch)

---

## 🚀 Installation

Install via NuGet Package Manager:

```bash
Install-Package efetch
```

Or via .NET CLI:

```bash
dotnet add package efetch
```

---

## ✨ Features

- ✅ Clean abstraction with `IEfetch` interface
- 🔁 Retry support using Polly
- 📦 JSON deserialization with support for primitives, objects, and arrays
- 📡 Simple query parameter handling
- 📓 Optional request/response logging
- 🧩 Built-in support for dependency injection

---

## 🧩 Configuration

Add to your app's `appsettings.json`:

```json
"Efetch": {
  "BaseUrl": "https://api.example.com",
  "DefaultHeaders": {
    "Authorization": "Bearer YOUR_TOKEN",
    "Accept": "application/json"
  },
  "RetryCount": 3
}
```

Register `Efetch` in your DI container (`Program.cs`):

```csharp
builder.Services.AddEfetch(builder.Configuration);
```

---

## 🧪 Usage

### Inject `IEfetch`:

```csharp
public class VaultController : ControllerBase
{
    private readonly IEfetch _efetch;

    public VaultController(IEfetch efetch)
    {
        _efetch = efetch;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string key)
    {
        var result = await _efetch.GetAsync<object>("vault", null, new() { { "key", key } });
        return Ok(result);
    }
}
```

---

## 📦 Example

```csharp
public class MyService
{
    private readonly IEfetch _efetch;

    public MyService(IEfetch efetch)
    {
        _efetch = efetch;
    }

    public async Task RunAsync()
    {
        // GET example
        var result = await _efetch.GetAsync<MyResponse>("/data");

        // POST example
        var body = new MyRequest { Name = "John" };
        var response = await _efetch.PostAsync<MyResponse, MyRequest>("/create", body);

        // String response example
        var plainText = await _efetch.GetAsync<string>("/version");
    }
}
```

---

## 🛠 API

### `IEfetch`

- `GetAsync<T>(...)`
- `PostAsync<T, TBody>(...)`
- `PutAsync<T, TBody>(...)`
- `PatchAsync<T, TBody>(...)`
- `DeleteAsync<T>(...)`

All methods support:
- Optional headers
- Optional query parameters
- Optional ID for REST-style routes
- Built-in retry policy

### `ILoggingProvider`

Optionally implement your own logger or use the built-in `ConsoleLoggingProvider`.

---

## 🔧 Advanced: Manual Configuration

```csharp
builder.Services.AddSingleton(new EfetchConfig
{
    BaseUrl = "https://api.example.com",
    DefaultHeaders = new()
    {
        { "Authorization", "Bearer abc123" },
        { "Accept", "application/json" }
    }
});

builder.Services.AddTransient<IEfetch, Efetch>();
```

---

## 👤 Author

**efetch** is created by [Ethern Myth](https://github.com/Ethern-Myth).

---

## 📄 License

Licensed under the [MIT License](https://opensource.org/licenses/MIT).
