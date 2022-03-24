using Berry.Spider.Core;

namespace Berry.Spider.AspNetCore.Mvc
{
    /// <summary>
    /// 公共接口响应对象
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
    }

    /// <summary>
    /// 泛型公共接口响应对象
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    public class ApiResponse<T> : IResultDataWrapper where T : class
    {
        /// <summary>
        /// 返回code
        /// </summary>
        public ApiErrorCodes Code { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回不包含结果的未成功api结果
        /// </summary>
        /// <returns></returns>
        public static ApiResponse Failed()
        {
            return new ApiResponse { Success = false };
        }

        /// <summary>
        /// 返回未成功的api结果
        /// </summary>
        /// <returns></returns>
        public static ApiResponse Failed(object data, string? message = null, ApiErrorCodes code = ApiErrorCodes.Fail)
        {
            return new ApiResponse
            {
                Success = false,
                Code = code,
                Data = data,
                Message = message ?? code.GetDescription()
            };
        }

        /// <summary>
        /// 返回不包含结果的成功api结果
        /// </summary>
        /// <returns></returns>
        public static ApiResponse Succeed()
        {
            return new ApiResponse { Success = true };
        }

        /// <summary>
        /// 返回成功的api结果
        /// </summary>
        /// <returns></returns>
        public static ApiResponse Succeed(object data, string? message = null, ApiErrorCodes code = ApiErrorCodes.OK)
        {
            return new ApiResponse
            {
                Success = true,
                Code = code,
                Data = data,
                Message = message ?? code.GetDescription()
            };
        }
    }
}