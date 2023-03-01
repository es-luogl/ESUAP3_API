using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: OwinStartup(typeof(Easysoft.Api.ceshi.Startup))]
namespace Easysoft.Api.ceshi
{
    public class Startup
    {
        /// <summary>
        /// token有效时长，单位：秒。
        /// 默认：2小时(7200秒)
        /// </summary>
        public readonly static int expires_in = 7200;
        public readonly static string secretkey = "6d3b6cf232e94bd9"; //AES密钥
        public readonly static string appid = "yrgs"; //应用标识
        public readonly static string appkey = "2ac1c456cb224c98937d"; //应用密钥
        public readonly static string input_chart = "utf-8";
        public void Configuration(IAppBuilder app)
        {
            // 创建Web API 的配置
            HttpConfiguration config = new HttpConfiguration();
            // 移除xml返回格式数据
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // 解决json序列化时的循环引用问题
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            // 使用webApi特性路由
            config.MapHttpAttributeRoutes();
            // 设置webapi路由规则
            config.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "{controller}/{id}", defaults: new { id = RouteParameter.Optional });
            // 附加路有配置
            app.UseWebApi(config);
        }
    }
}
