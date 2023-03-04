using Easysoft.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Easysoft.Api.ceshi
{
    public class OAuth2Attribute : AuthorizeAttribute
    {
        private int code = 200;
        private string message = "OK";
        private readonly Encoding encode = Encoding.GetEncoding(Startup.input_chart);

        /// <summary>
        /// 重写基类的验证方式，加入自定义的Ticket验证
        /// </summary>
        public override void OnAuthorization(HttpActionContext context)
        {
            //设置了不验证令牌(token)，即方法前加 [AllowAnonymous]
            if (context.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                base.IsAuthorized(context);
                return;
            }
            //从http请求的头里面获取身份验证信息，验证是否是请求发起方的ticket
            var authorization = context.Request.Headers.Authorization;
            if (authorization == null)
            {
                code = 301; message = "未传入身份(Authorization)请求头"; HandleUnauthorizedRequest(context); return;
            }
            try
            {
                string evidence = EncryptUtil.MD5(Startup.appid + "&" + Startup.appkey).ToLower(); //token的凭据
                string token = DecryptUtil.AES(authorization.Scheme, Startup.secretkey);
                JObject jObject = JObject.Parse(token);
                if (!evidence.Equals(jObject["evidence"].ToString().ToLower()))
                {
                    code = 303; message = "传入的token无效!"; HandleUnauthorizedRequest(context); return;
                }
                DateTime expiresTime = BasicHelper.ConvertTimeStamp(jObject["expires_time"].ToString());
                if (expiresTime.Subtract(DateTime.Now).TotalSeconds >= 0)
                {
                    base.IsAuthorized(context);
                    return;
                }
                else { code = 304; message = "传入的token已超过有效时限!"; HandleUnauthorizedRequest(context); return; }
            }
            catch { code = 303; message = "传入的token无效!"; HandleUnauthorizedRequest(context); return; }
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext context)
        {
            base.HandleUnauthorizedRequest(context);
            //响应报文
            var response = context.Response = context.Response ?? new HttpResponseMessage();
            response.Headers.Remove("Server");
            response.Headers.Remove("X-AspNet-Version");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.StatusCode = HttpStatusCode.OK;
            //响应内容(body)
            JObject obj = new JObject() { { "code", code }, { "message", message }, { "body", null } };
            response.Content = new StringContent(obj.ToString(), encode, "application/json");
        }
    }
}