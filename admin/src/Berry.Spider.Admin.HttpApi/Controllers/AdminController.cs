using Berry.Spider.Admin.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Berry.Spider.Admin;

/* Inherit your controllers from this class.
 */
public abstract class AdminController : AbpControllerBase
{
    protected AdminController()
    {
        LocalizationResource = typeof(AdminResource);
    }
}