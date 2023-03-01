using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace Easysoft.Api.ceshi.Controllers
{
    /// <summary>
    /// ceshi-收费接口
    /// </summary>
    public class ChargeController : ApiController
    {
        private readonly Encoding encode = Encoding.GetEncoding(Startup.input_chart);

        [HttpPost]
        [Route("ceshi/charge/history")]
        public HttpResponseMessage History()
        {
            DateTime reqTime = DateTime.UtcNow;
            int rsp_code = 200; string req_body = ""; string extends1 = "";
            JObject rsp_body = null;
            try
            {
                using (Stream s = HttpContext.Current.Request.InputStream)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, (int)buffer.Length);
                    req_body = HttpUtility.UrlDecode(encode.GetString(buffer));
                }
                if (req_body == "") { rsp_code = 200412; rsp_body = new JObject() { { "code", rsp_code }, { "message", "接口参数为空" }, { "body", null } }; }
                else
                {

                }
            }
            catch (Exception ex) { rsp_code = 200502; rsp_body = rsp_body = new JObject() { { "code", rsp_code }, { "message", ex.Message }, { "body", null } }; }
            finally { }
        }

    }
}
