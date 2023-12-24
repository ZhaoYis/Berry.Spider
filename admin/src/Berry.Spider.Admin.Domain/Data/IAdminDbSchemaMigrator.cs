using System.Threading.Tasks;

namespace Berry.Spider.Admin.Data;

public interface IAdminDbSchemaMigrator
{
    Task MigrateAsync();
}