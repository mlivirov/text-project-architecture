using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace StupidChat.IntegrationTests.Fixtures;

public class TestWebApplicationFactoryBuilder(
    PostgreSqlContainer postgreSqlContainer,
    RedisContainer redisContainer)
{
    private readonly List<object> _dbSeeds = new();
    private Action<IServiceCollection> _serviceCollectionSetup = (_) => { };

    public TestWebApplicationFactoryBuilder WithDbSeeds(params object[] dbSeeds)
    {
        _dbSeeds.AddRange(dbSeeds);
        return this;
    }

    public TestWebApplicationFactoryBuilder WithServiceCollectionSetup(
        Action<IServiceCollection> serviceCollectionSetup)
    {
        _serviceCollectionSetup = serviceCollectionSetup;
        return this;
    }

    public TestWebApplicationFactory Build()
    {
        return new TestWebApplicationFactory(
            postgreSqlContainer,
            redisContainer,
            _dbSeeds,
            _serviceCollectionSetup);
    }
}