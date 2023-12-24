namespace Berry.Spider.Admin.Permissions;

public static class AdminGlobalPermissions
{
    private const string RootGroupName = "Admin";

    /// <summary>
    /// 小工具
    /// </summary>
    public static class Tools
    {
        public const string Default = RootGroupName + "." + nameof(Tools);

        public static class ServNodes
        {
            public const string Root = Default + "." + nameof(ServNodes);
            public const string DeployAppNode = Root + "." + nameof(DeployAppNode);
            public const string RestartAllAppNode = Root + "." + nameof(RestartAllAppNode);
        }
    }
}