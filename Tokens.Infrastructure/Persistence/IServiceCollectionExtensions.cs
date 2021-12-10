using Microsoft.Extensions.DependencyInjection;
using Tokens.Infrastructure.Persistence.Database;
using Tokens.Infrastructure.Persistence.Repository;

namespace Tokens.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, Action<PersistenceOptions> setupOptions)
    {
        var options = new PersistenceOptions();
        setupOptions?.Invoke(options);

        services.AddDatabase(options.DbOptions);

        services.AddAzureStorageAccount(options.BlobStorageOptions);

        services.AddRepositories();
    }

    public class PersistenceOptions
    {
        public DbOptions DbOptions { get; set; } = new();
        public AzureStorageAccountOptions BlobStorageOptions { get; set; } = new();
    }
}
