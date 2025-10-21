using BillingAgreementService.Domain.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StupidChat.Application.Common;
using StupidChat.Infrastructure.Persistence;

namespace StupidChat.Infrastructure;

public static class DependencyInjection
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddCache(services, configuration);
        AddPersistence(services, configuration);
        
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
    
    private static void AddCache(IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = configuration.GetConnectionString("Redis");
        });
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("OperationalDatabase"));
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}