using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Easysoft.Api.ceshi
{
    /// <summary>
    /// 基础工具类
    /// </summary>
    public static class BasicHelper
    {
        #region## 获取REQUEST的值
        /// <summary>
        /// 获取HTTP的QueryString值
        /// </summary>
        public static string GetRequestParams(string name)
        {
            if (HttpContext.Current.Request[name] == null) return "";
            return Regex.Unescape(HttpUtility.HtmlDecode(HttpContext.Current.Request[name].ToString()));
        }
        /// <summary>
        /// 获取HTTP的头部参数(Header)值
        /// </summary>
        public static string GetRequestHeader(string name)
        {
            if (HttpContext.Current.Request.Headers[name] == null) return "";
            return Regex.Unescape(HttpUtility.HtmlDecode(HttpContext.Current.Request.Headers[name].ToString()));
        }
        /// <summary>
        /// 获取HTTP的表单参数(Form)值
        /// </summary>
        public static string GetRequestForm(string name)
        {
            if (HttpContext.Current.Request.Form[name] == null) return "";
            return Regex.Unescape(HttpUtility.HtmlDecode(HttpContext.Current.Request.Form[name].ToString()));
        }
        #endregion

        /// <summary>
        /// 时间戳(秒)
        /// </summary>
        public static string GetTimestamp(DateTime dt)
        {
            return ((dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        }
        /// <summary>
        /// 时间戳转为DateTime
        /// </summary>
        public static DateTime ConvertTimeStamp(string timestamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));//当地时区
            return startTime.AddSeconds(long.Parse(timestamp));
        }
    }
}