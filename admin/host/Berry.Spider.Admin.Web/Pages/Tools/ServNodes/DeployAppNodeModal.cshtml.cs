using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Berry.Spider.Admin.Web.Pages.Tools.ServNodes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Berry.Spider.Admin.Web.Pages.Tools.ServNodes;

public class DeployAppNodeModal : AbpPageModel
{
    /// <summary>
    /// 部署节点
    /// </summary>
    [BindProperty]
    public DeployAppNodeDto Deploy { get; set; }

    public async Task OnGetAsync(string agentBizNo)
    {
        this.Deploy = new DeployAppNodeDto
        {
            CurrentAgentBizNo = agentBizNo,
            AppVersionList = new List<SelectListItem>
            {
                new SelectListItem("v20231227-100", "1", true),
                new SelectListItem("v20231227-101", "2")
            },
            RunAppCount = 3
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return new JsonResult(this.Deploy);
    }
}