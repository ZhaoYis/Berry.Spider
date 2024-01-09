using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Berry.Spider.Admin.Web.Pages.Tools.ServNodes.Models;
using Berry.Spider.Biz;
using Berry.Spider.Core;
using Berry.Spider.RealTime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Berry.Spider.Admin.Web.Pages.Tools.ServNodes;

public class DeployAppNodeModal : AbpPageModel
{
    private IHubContext<SpiderAgentNotifyHub, ISpiderAgentReceiveHub> _hubContext;
    private readonly ISpiderAppInfoService _spiderAppInfoService;

    public DeployAppNodeModal(IHubContext<SpiderAgentNotifyHub, ISpiderAgentReceiveHub> hubContext,
        ISpiderAppInfoService spiderAppInfoService)
    {
        _hubContext = hubContext;
        _spiderAppInfoService = spiderAppInfoService;
    }

    /// <summary>
    /// 部署节点
    /// </summary>
    [BindProperty]
    public DeployAppNodeDto Deploy { get; set; }

    public async Task OnGetAsync(string agentBizNo, string connectionId)
    {
        List<SpiderAppInfoDto> appInfoList = await _spiderAppInfoService.GetSpiderAppListAsync();
        if (appInfoList is { Count: > 0 })
        {
            this.Deploy = new DeployAppNodeDto
            {
                CurrentAgentBizNo = agentBizNo,
                ConnectionId = connectionId,
                AppVersionList = appInfoList.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.BizNo
                }).ToList(),
                RunAppCount = 3
            };
        }
        else
        {
            throw new BusinessException(message: "暂无可用应用");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string runAppVersionBizNo = this.Deploy.RunAppVersionBizNo;
        SpiderAppInfoDto appInfo = await _spiderAppInfoService.GetSpiderAppInfoAsync(runAppVersionBizNo);

        SpiderAgentReceiveDto agentReceiveDto = new SpiderAgentReceiveDto
        {
            Code = RealTimeMessageCode.NOTIFY_AGENT_TO_START_DEPLOYING_APP,
            Data = JsonSerializer.Serialize(new
            {
                RunAppInfo = appInfo,
                RunAppCount = this.Deploy.RunAppCount
            })
        };
        await _hubContext.Clients.Client(this.Deploy.ConnectionId).ReceiveMessageAsync(agentReceiveDto);

        return new JsonResult(this.Deploy);
    }
}