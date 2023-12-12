namespace Berry.Spider.Admin.Menus;

public class AdminMenus
{
    private const string Prefix = "Admin";
    public const string Home = Prefix + ".Home";

    /// <summary>
    /// 小工具
    /// </summary>
    public static class Tools
    {
        public const string Main = Prefix + ".Tools";

        public static class Sub
        {
            public const string ServNodes = Main + ".ServNodes";
        }
    }

    //Add your menu items here...
}