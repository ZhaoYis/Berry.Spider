﻿namespace Berry.Spider.Core
{
    /// <summary>
    /// 自定义爬虫特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SpiderServiceAttribute : Attribute
    {
        public SpiderServiceAttribute(SpiderSourceFrom from)
        {
            this.SourceFrom = from;
        }

        public SpiderSourceFrom SourceFrom { get; set; }
    }
}