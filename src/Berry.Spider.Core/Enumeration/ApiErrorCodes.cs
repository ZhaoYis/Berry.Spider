using System.ComponentModel;

namespace Berry.Spider.Core
{
    /// <summary>
    /// 全局公共API错误码
    /// </summary>
    public enum ApiErrorCodes
    {
        /**
         * 错误码0~999为系统预留错误码位，请勿使用。
         */

        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Fail = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        OK = 200,
    }
}
