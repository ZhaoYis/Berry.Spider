using Berry.Spider.Admin.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Berry.Spider.Admin.Web.Pages;

public abstract class AdminPageModel : AbpPageModel
{
    protected AdminPageModel()
    {
        LocalizationResourceType = typeof(AdminResource);
    }
}