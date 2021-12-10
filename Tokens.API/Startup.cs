using Enmeshed.BuildingBlocks.API.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Tokens.API.Extensions;
using Tokens.API.JsonConverters;
using Tokens.Application;
using Tokens.Application.Extensions;
using Tokens.Infrastructure.EventBus;
using Tokens.Infrastructure.Persistence.Database;
using Tokens.Infrastructure.Persistence.Repository;

namespace Tokens.API;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.Configure<ApplicationOptions>(_configuration.GetSection("ApplicationOptions"));

        services.AddCustomAspNetCore(_configuration, _env, options =>
        {
            options.Authentication.Audience = "tokens";
            options.Authentication.Authority = _configuration.GetAuthorizationConfiguration().Authority;
            options.Authentication.ValidIssuer = _configuration.GetAuthorizationConfiguration().ValidIssuer;

            options.Cors.AllowedOrigins = _configuration.GetCorsConfiguration().AllowedOrigins;
            options.Cors.ExposedHeaders = _configuration.GetCorsConfiguration().ExposedHeaders;

            options.HealthChecks.SqlConnectionString = _configuration.GetSqlDatabaseConfiguration().ConnectionString;

            options.Json.Converters.Add(new TokenIdJsonConverter());
        });

        services.AddCustomApplicationInsights();

        services.AddCustomFluentValidation(_ => { });

        services.AddDatabase(dbOptions => { dbOptions.DbConnectionString = _configuration.GetSqlDatabaseConfiguration().ConnectionString; });

        services.AddAzureStorageAccount(options =>
        {
            options.ConnectionString = _configuration.GetBlobStorageConfiguration().ConnectionString;
            options.ContainerName = "tokens";
        });

        services.AddRepositories();

        services.AddEventBus(_configuration.GetEventBusConfiguration());

        services.AddApplication();

        return services.ToAutofacServiceProvider();
    }

    public void Configure(IApplicationBuilder app, TelemetryConfiguration telemetryConfiguration)
    {
        telemetryConfiguration.DisableTelemetry = !_configuration.GetApplicationInsightsConfiguration().Enabled;

        app.ConfigureMiddleware(_env);
    }
}
