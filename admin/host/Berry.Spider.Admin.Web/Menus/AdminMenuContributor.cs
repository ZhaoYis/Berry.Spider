using System;
using System.Threading.Tasks;
using Berry.Spider.Admin.Localization;
using Berry.Spider.Admin.MultiTenancy;
using Berry.Spider.Admin.Permissions;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Account.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace Berry.Spider.Admin.Web.Menus;

public class AdminMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public AdminMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<AdminResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                AdminMenus.Home,
                l["Menu:Home"],
                "~/",
                "fas fa-home",
                0
            )
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AdminMenus.Tools.Main,
                l["Menu:Tools:Main"],
                icon: "fa fa-book"
            ).AddItem(
                new ApplicationMenuItem(
                    AdminMenus.Tools.Sub.ServNodes,
                    l["Menu:Tools:Sub:ServNodes"],
                    "/Tools/ServNodes"
                ).RequirePermissions(requiresAll: true, AdminGlobalPermissions.Tools.ServNodes.Root)
            )
        );

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<AdminResource>();
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Manage",
            accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={_configuration["App:SelfUrl"]}",
            "fa fa-cog",
            1000,
            null,
            "_blank").RequireAuthenticated());

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Logout",
            l["Logout"],
            "~/Account/Logout",
            "fa fa-power-off",
            int.MaxValue - 1000).RequireAuthenticated());

        return Task.CompletedTask;
    }
}