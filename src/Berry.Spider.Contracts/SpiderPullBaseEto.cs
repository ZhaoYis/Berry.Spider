using Berry.Spider.Core;

namespace Berry.Spider
{
    public class SpiderPullBaseEto : ISpiderPullEto
    {
        protected SpiderPullBaseEto(SpiderSourceFrom @from)
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
        public string Keyword { get; set; } = null!;

        /// <summary>
        /// 保存这次记录最终的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 二级页面地址信息
        /// </summary>
        public List<ChildPageDataItem> Items { get; set; } = new List<ChildPageDataItem>();
    }
}