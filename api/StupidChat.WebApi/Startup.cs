using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using StupidChat.Application;
using StupidChat.Application.Common;
using StupidChat.Infrastructure;
using StupidChat.WebApi.Authentication;

namespace StupidChat.WebApi;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ApplicationExceptionFilter>();
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "StupidChat API", Version = "v1" });
    
            c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = "Basic Authorization header. Enter your username and password."
            });
    
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Basic"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddHttpContextAccessor();
        services.AddSingleton<ICurrentUserAccessor, CurrentHttpUserAccessor>();

        services.AddAuthentication("Basic")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

        services.AddAuthorization();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.MigrateDatabase();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(cfg => cfg.MapControllers());
    }
}