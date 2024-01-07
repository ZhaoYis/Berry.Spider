using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Berry.Spider.Admin.Web.Pages.Tools.ServNodes.Models;
using Berry.Spider.Core;
using Berry.Spider.RealTime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Berry.Spider.Admin.Web.Pages.Tools.ServNodes;

public class DeployAppNodeModal : AbpPageModel
{
    private IHubContext<SpiderAgentNotifyHub, ISpiderAgentReceiveHub> _hubContext;

    public DeployAppNodeModal(IHubContext<SpiderAgentNotifyHub, ISpiderAgentReceiveHub> hubContext)
    {
        _hubContext = hubContext;
    }

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
        SpiderAgentReceiveDto agentReceiveDto = new SpiderAgentReceiveDto
        {
            Code = RealTimeMessageCode.NOTIFY_AGENT_TO_START_DEPLOYING_APP,
            Data = JsonSerializer.Serialize(new
            {
                CurrentAgentBizNo = this.Deploy.CurrentAgentBizNo,
                RunAppVersion = this.Deploy.RunAppVersion,
                RunAppCount = this.Deploy.RunAppCount
            })
        };
        await _hubContext.Clients.Groups(MachineGroupCode.Agent.GetName()).ReceiveMessageAsync(agentReceiveDto);

        return new JsonResult(this.Deploy);
    }
}