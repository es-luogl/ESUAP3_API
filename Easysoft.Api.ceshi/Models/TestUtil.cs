using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Easysoft.Api.ceshi.Models
{
    /// <summary>
    /// 测试接口工具类
    /// </summary>
    public static class TestUtil
    {
        public static JObject Test()
        {
            StringBuilder log = new StringBuilder();
            JObject rsp_data = new JObject();
            try
            {
                string serverurl = "http://localhost/api/"; string appid = Startup.appid; string appkey = Startup.appkey;
                string authorization = GetToken(serverurl, appid, appkey); log.AppendLine("1.获取token值：" + authorization);

                string posturl = $"{serverurl}esuap3/charge/getaway";
                string postdata = "{\"head\":{\"method\":\"QueryChargeBill\",\"sign\":\"sysadmin007\"},\"body\":{\"startdate\":\"2023-02-01\",\"stopdate\":\"2023-02-28\"}}";
                log.AppendLine("2.地址：" + posturl + Environment.NewLine + "3.请求头：Authorization=" + authorization + Environment.NewLine + "4.参数：" + postdata);
                Dictionary<string, string> header = new Dictionary<string, string> { { "Authorization", authorization } };
                Easysoft.Library.HttpUtil.HttpPost(posturl, postdata.ToString(), "application/json", header, out string rsp_body);
                log.AppendLine("5.响应：" + rsp_body);
                rsp_data = JObject.Parse(rsp_body);
            }
            catch (Exception ex) { rsp_data = new JObject { { "code", 502 }, { "message", "系统异常，描述：" + ex.Message }, { "body", null } }; }
            // 写日志
            finally { BasicHelper.SetWriteTxtLog("本地测试接口日志" + DateTime.Now.ToString("yyyyMMdd"), log.ToString()); }
            return rsp_data;
        }
        private static string GetToken(string serverurl, string appid, string appkey)
        {
            string token = "";
            string posturl = $"{serverurl}aouth2/getToken?appid={appid}&appkey={appkey}";
            Easysoft.Library.HttpUtil.HttpGet(posturl, null, out string rsp_body);
            JObject data = JObject.Parse(rsp_body);
            if (data["code"].ToString().Equals("200"))
                token = data["access_token"].ToString();

            return token;
        }
    }
}