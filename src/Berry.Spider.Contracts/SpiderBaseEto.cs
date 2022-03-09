using Berry.Spider.Domain.Shared;

namespace Berry.Spider
{
    public class SpiderBaseEto
    {
        protected SpiderBaseEto(SpiderSourceFrom @from)
        {
            this.SourceFrom = from;
        }

        /// <summary>
        /// 来源
        /// </summary>
        public SpiderSourceFrom SourceFrom { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 二级页面地址信息
        /// </summary>
        public List<SpiderChildPageDataItem> Items { get; set; } = new List<SpiderChildPageDataItem>();
    }
}