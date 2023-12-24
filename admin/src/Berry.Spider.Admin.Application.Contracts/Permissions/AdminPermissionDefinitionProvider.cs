using Berry.Spider.Admin.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Berry.Spider.Admin.Permissions;

public class AdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //小工具
        var toolsGroup = context.AddGroup(AdminGlobalPermissions.Tools.Default, L(AdminGlobalPermissions.Tools.Default));
        //服务节点
        var servNodesPermission = toolsGroup.AddPermission(AdminGlobalPermissions.Tools.ServNodes.Root, L(AdminGlobalPermissions.Tools.ServNodes.Root));
        servNodesPermission.AddChild(AdminGlobalPermissions.Tools.ServNodes.DeployAppNode, L(AdminGlobalPermissions.Tools.ServNodes.DeployAppNode));
        servNodesPermission.AddChild(AdminGlobalPermissions.Tools.ServNodes.RestartAllAppNode, L(AdminGlobalPermissions.Tools.ServNodes.RestartAllAppNode));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AdminResource>($"Permission.{name}");
    }
}