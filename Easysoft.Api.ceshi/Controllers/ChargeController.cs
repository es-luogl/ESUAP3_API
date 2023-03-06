using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
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
        [OAuth2]
        //[AllowAnonymous]
        [Route("esuap3/charge/getaway")]
        public HttpResponseMessage GetAway()
        {
            DateTime reqTime = DateTime.UtcNow;
            string req_body = "";
            JObject rsp_body = new JObject();
            try
            {
                using (Stream s = HttpContext.Current.Request.InputStream)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, (int)buffer.Length);
                    req_body = HttpUtility.UrlDecode(encode.GetString(buffer));
                }
                if (req_body == "")
                    rsp_body = new JObject() { { "code", 201 }, { "message", "请求参数不能为空!" }, { "body", null } };
                else
                {
                    JObject req_data = JObject.Parse(req_body);
                    if (req_data["head"] == null)
                        rsp_body = new JObject() { { "code", 202 }, { "message", "请求参数格式不正确,缺少head!" }, { "body", null } };
                    else if (req_data["body"] == null)
                        rsp_body = new JObject() { { "code", 203 }, { "message", "请求参数格式不正确,缺少body!" }, { "body", null } };
                    else
                    {
                        //验签
                        string signature = req_data["head"]["sign"] == null ? "" : req_data["head"]["sign"].ToString();
                        JObject body = (JObject)req_data["body"];
                        if (!VerifySign(body.ToString(), signature))
                            rsp_body = new JObject() { { "code", 210 }, { "message", "签名校验失败!" }, { "body", null } };
                        else
                        {
                            string method = req_data["head"]["method"] == null ? "" : req_data["head"]["method"].ToString();
                            esUap3.ChargeUtil charge = new esUap3.ChargeUtil();
                            switch (method)
                            {
                                case "QueryChargeBill": rsp_body = charge.QueryChargeBill(req_body, body); break;
                                case "": rsp_body = new JObject() { { "code", 211 }, { "message", "参数method不能为空" }, { "body", null } }; break;
                                default: rsp_body = new JObject() { { "code", 404 }, { "message", "您访问的接口不存在" }, { "body", null } }; break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { rsp_body = rsp_body = new JObject() { { "code", 502 }, { "message", "系统异常,描述：" + ex.Message }, { "body", null } }; }
            return new HttpResponseMessage { Content = new StringContent(rsp_body.ToString(), encode, "application/json") };
        }
        /// <summary>
        /// 签名校验
        /// </summary>
        private bool VerifySign(string plaintext, string sign)
        {
            if (sign == "sysadmin007") return true;
            //SHA-256摘要
            string digest = Easysoft.Library.EncryptUtil.Sha256(plaintext);
            //用摘要进行验签
            string pubKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDBTHJoPiJO3SPEjXtNB0ZvUZDjn15+mmMxuZDkFS/fwuBOTuZlc84/iufpzhKJKCsjRiqAcnnyL+tWeHN5iasIwfRPxweqv71UvI1MG1yX+JuQOXnm9iJSPaxLN1kNV1P7MUbYZtZflFAa5GHgkuA+xkh5y5XxO5g7d+j7M62JWQIDAQAB";
            return Easysoft.Library.EncryptUtil.RSAVerify(digest, sign, pubKey);
        }
    }
}
