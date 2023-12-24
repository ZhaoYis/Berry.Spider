using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Berry.Spider.Admin.Web.Pages;

public class IndexModel : AdminPageModel
{
    public void OnGet()
    {
    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}