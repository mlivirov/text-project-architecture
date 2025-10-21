using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace StupidChat.IntegrationTests.Fixtures;

public class WebApplicationFactoryFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgreSqlContainer.StartAsync(),
            _redisContainer.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgreSqlContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask());
    }

    public DisposableHost<HttpClient> CreateHost(Action<TestWebApplicationFactoryBuilder>? setup = null)
    {
        var webApplicationFactoryBuilder = new TestWebApplicationFactoryBuilder(_postgreSqlContainer, _redisContainer);
        setup?.Invoke(webApplicationFactoryBuilder);
        var webApplicationFactory = webApplicationFactoryBuilder.Build();

        var client = webApplicationFactory.CreateClient();

        return new DisposableHost<HttpClient>(
            webApplicationFactory,
            client
        );
    }
}