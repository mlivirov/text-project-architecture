using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using StupidChat.Infrastructure.Persistence;
using StupidChat.WebApi;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace StupidChat.IntegrationTests.Fixtures;

public class TestWebApplicationFactory(
        PostgreSqlContainer postgreSqlContainer,
        RedisContainer redisContainer,
        List<object> dbSeeds,
        Action<IServiceCollection> testServices
    )
    : WebApplicationFactory<Startup>
{
    private Checkpoint? _checkpoint;

    public void AssertDb(Action<ApplicationDbContext> assertAction)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        assertAction.Invoke(dbContext);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_checkpoint is not null)
        {
            await using var connection = new NpgsqlConnection(postgreSqlContainer.GetConnectionString());
            await connection.OpenAsync();

            await _checkpoint.Reset(connection);
        }

        await base.DisposeAsync();
    }

    protected override IHostBuilder? CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();

                webBuilder.ConfigureAppConfiguration((_, builder) =>
                {
                    builder.AddJsonFile("appsettings.json");
                    var settings = new Dictionary<string, string>
                    {
                        { "ConnectionStrings:OperationalDatabase", postgreSqlContainer.GetConnectionString() },
                        { "ConnectionStrings:Redis", redisContainer.GetConnectionString() },
                    };
                    builder.AddInMemoryCollection(settings!);
                });

                webBuilder.ConfigureTestServices(testServices);
            });

        return builder;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        _checkpoint = new Checkpoint()
        {
            TablesToIgnore = ["__EFMigrationsHistory"],
            SchemasToInclude = ["public"],
            DbAdapter = DbAdapter.Postgres,
            WithReseed = true
        };

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        foreach (var dbSeed in dbSeeds)
        {
            dbContext.Add(dbSeed);
        }

        dbContext.SaveChanges();

        return host;
    }
}