namespace Berry.Spider.Core
{
    /// <summary>
    /// 自定义爬虫特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SpiderServiceAttribute : Attribute
    {
        public SpiderServiceAttribute(SpiderSourceFrom[] fromArray)
        {
            this.SourceFromArray = fromArray.Distinct().ToArray();
        }

        public SpiderSourceFrom[] SourceFromArray { get; set; }
    }
}