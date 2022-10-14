using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Admin.Data;

public class AdminEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public AdminEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the AdminDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<AdminDbContext>()
            .Database
            .MigrateAsync();
    }
}
