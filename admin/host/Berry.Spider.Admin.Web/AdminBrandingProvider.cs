using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Berry.Spider.Admin.Web;

[Dependency(ReplaceServices = true)]
public class AdminBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Admin";
}