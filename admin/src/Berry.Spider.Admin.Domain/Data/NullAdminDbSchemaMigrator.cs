﻿using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Admin.Data;

/* This is used if database provider does't define
 * IAdminDbSchemaMigrator implementation.
 */
public class NullAdminDbSchemaMigrator : IAdminDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}