using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Easysoft.Api.ceshi.Controllers
{
    /// <summary>
    /// 本地测试接口
    /// </summary>
    public class LocalTestController : ApiController
    {
        private readonly Encoding encode = Encoding.GetEncoding(Startup.input_chart);

        [HttpPost]
        [AllowAnonymous]
        [Route("yrgs/local/test")]
        public HttpResponseMessage Test()
        {
            JObject rsp_body = null;
            try
            {
                string method = BasicHelper.GetRequestParams("method");
                switch (method)
                {
                    case "Test": rsp_body = Models.TestUtil.Test(); break;
                    default: rsp_body = new JObject { { "code", 404 }, { "message", "接口不存在" }, { "body", null } }; break;
                }
            }
            catch (Exception ex) { rsp_body = new JObject { { "code", 502 }, { "message", "系统异常，描述：" + ex.Message }, { "body", null } }; }
            HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", "*"); //跨域
            return new HttpResponseMessage { Content = new StringContent(rsp_body.ToString(), encode, "application/json") };
        }
    }
}
