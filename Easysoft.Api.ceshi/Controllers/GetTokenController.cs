using Easysoft.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Web.Http;

namespace Easysoft.Api.ceshi.Controllers
{
    public class GetTokenController : ApiController
    {
        [HttpGet]
        [Route("ceshi/getToken")]
        public object GetToken()
        {
            HttpContext.Current.Response.Headers.Remove("Server");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            string appid = BasicHelper.GetRequestParams("appid");
            string appkey = BasicHelper.GetRequestParams("appkey");
            if (appid != Startup.appid)
                return new RSP_Message { code = 401101, message = "应用ID无效" };
            if (appkey != Startup.appkey)
                return new RSP_Message { code = 401102, message = "应用密钥无效" };

            string evidence = EncryptUtil.MD5(appid + "&" + appkey).ToLower(); //凭据
            string expires_time = BasicHelper.GetTimestamp(DateTime.Now.AddSeconds(Startup.expires_in)); //token有效期
            JObject token = new JObject { { "evidence", evidence }, { "expires_time", expires_time } };
            string access_token = EncryptUtil.AES(token.ToString(), Startup.secretkey).ToLower();
            // 返回登录结果、用户信息、用户验证票据信息
            return new RSP_Message { code = 200, message = "OK", expires_in = Startup.expires_in, access_token = access_token };
        }
        private class RSP_Message
        {
            public int code { get; set; }
            public string message { get; set; }
            public int expires_in { get; set; }
            public string access_token { get; set; }
        }
    }
}
