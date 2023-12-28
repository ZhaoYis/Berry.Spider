using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Berry.Spider.Admin.Web.Pages.Tools.ServNodes.Models;

public class DeployAppNodeDto
{
    /// <summary>
    /// 当前操作的代理节点编号
    /// </summary>
    [HiddenInput]
    public string CurrentAgentBizNo { get; set; }

    /// <summary>
    /// App版本集合
    /// </summary>
    public List<SelectListItem> AppVersionList { get; set; }

    /// <summary>
    /// 运行App的版本
    /// </summary>
    [DataType(DataType.Text)]
    [Display(Name = "DeployAppNodeModal:RunAppVersion")]
    [SelectItems(nameof(AppVersionList))]
    public string RunAppVersion { get; set; }

    /// <summary>
    /// 启动App的数量
    /// </summary>
    [Required]
    [Display(Name = "DeployAppNodeModal:RunAppCount")]
    [FormControlSize(AbpFormControlSize.Default)]
    public int RunAppCount { get; set; }
}