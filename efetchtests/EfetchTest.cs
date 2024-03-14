using efetch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace efetchtests
{
    public class Todo
    {
        public int Id { get; set; }
        public string Item { get; set; }
        // Add more properties as needed
    }

    public class EfetchTests
    {
        private Efetch _efetchInstance;
        private readonly ILoggingProvider _loggingProvider = ConsoleLoggingProvider.Instance;
        private readonly IHttpClientFactory _httpClientFactory;

        public EfetchTests()
        {
            // Mock IHttpClientFactory
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var httpClientMock = new Mock<HttpClient>();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClientMock.Object);
            _httpClientFactory = httpClientFactoryMock.Object;
        }

        [SetUp]
        public void Setup()
        {
            _efetchInstance = Efetch.InstanceConfig(new EfetchConfig() { BaseUrl = "http://127.0.0.1:8000" }, _loggingProvider);
        }

        [Test]
        public async Task GetAsync_Success()
        {
            // Arrange
            var url = "/todos";

            // Act
            var result = await _efetchInstance.GetAsync<dynamic>(url);

            // Assert
            Assert.IsNotNull(result);
            // Add more assertions as needed
        }

        [Test]
        public async Task PostAsync_Success()
        {
            // Arrange
            var url = "/todos";
            var requestBody = new Todo() { Id = 1, Item ="testing"};

            // Act
            var result = await _efetchInstance.PostAsync<dynamic, Todo>(url, requestBody);

            // Assert
            Assert.IsNotNull(result);
            // Add more assertions as needed
        }

        // Add similar tests for PutAsync, PatchAsync, and DeleteAsync methods

        [Test]
        public async Task GetAsync_WithId_Success()
        {
            // Arrange
            var url = "/todos";
            int id = 1;

            // Act
            var result = await _efetchInstance.GetAsync<dynamic>(url, id: id);

            // Assert
            Assert.IsNotNull(result);
            // Add more assertions as needed
        }

        [Test]
        public async Task GetAsync_Failure()
        {
            // Arrange
            var url = "/tod";

            // Act & Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _efetchInstance.GetAsync<Todo>(url));
        }

        [Test]
        public async Task GetAsync_Success_WithBearerToken()
        {
            // Arrange
            var url = "/todos";
            var bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            var headers = new Dictionary<string, string>
    {
        { "Authorization", $"Bearer {bearerToken}" }
    };

            // Act
            var result = await _efetchInstance.GetAsync<dynamic>(url, headers);

            // Assert
            Assert.IsNotNull(result);
            // Add more assertions as needed
        }

        [Test]
        public void InstanceConfig_RetrievesFromServiceCollection()
        {
            // Arrange
            var mockServiceCollection = new ServiceCollection();
            var mockOptions = new Mock<IOptions<EfetchConfig>>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockLoggingProvider = new Mock<ILoggingProvider>();

            // Set up the mock to return a mocked EfetchConfig instance
            var efetchConfig = new EfetchConfig { BaseUrl = "http://127.0.0.1:8000" };
            mockOptions.Setup(m => m.Value).Returns(efetchConfig);

            // Register the options mock with the service collection
            mockServiceCollection.AddSingleton(mockOptions.Object);

            // Act
            var result = Efetch.InstanceConfig(mockServiceCollection, mockLoggingProvider.Object);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(efetchConfig.BaseUrl, result.BaseUrl);
        }
    }
}