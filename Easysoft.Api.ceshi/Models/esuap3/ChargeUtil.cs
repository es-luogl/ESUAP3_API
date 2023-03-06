using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Easysoft.Api.ceshi.esUap3
{
    /// <summary>
    /// 收费接口工具类
    /// </summary>
    public class ChargeUtil
    {
        public ChargeUtil() { }
        public JObject QueryChargeBill(string req_body, JObject data)
        {
            DateTime req_time = DateTime.Now;
            StringBuilder log = new StringBuilder().AppendLine("[" + DateTime.Now.ToString("F") + "] 查询费用账单接口>>>");
            JObject rsp_body = new JObject();
            try
            {
                string startdate = data["startdate"] == null ? "" : data["startdate"].ToString();
                string stopdate = data["stopdate"] == null ? "" : data["stopdate"].ToString();
                if (startdate == "" || stopdate == "")
                    rsp_body = new JObject() { { "code", 205 }, { "message", "起止时间不能为空" }, { "body", null } };
                else
                {
                    JArray datas = new JArray();

                    string fullPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\dataTable.json";
                    string fullJson = Regex.Unescape(File.ReadAllText(fullPath, Encoding.GetEncoding("utf-8")).Trim());

                    DataTable dt = BasicHelper.JsonToDataTable(fullJson, "esTemp_Table2023030300001");

                    //JObject dt = new JObject(){{"feeId","JF202302010000000001"},{"project","高层物业费"},{"amount",256.00},{"lateAmount",0},{"inputDate","2023-02-01"},{"objectName","1-1-1001"},{"ownerName","李四"},{"feeDesc","2023-02-01至2023-02-28"}};
                    JObject body = new JObject() { { "datas", BasicHelper.FnDataTableToJArray(dt) } };
                    rsp_body = new JObject() { { "code", 200 }, { "message", "OK" }, { "body", body } };
                }
            }
            catch (Exception ex) { rsp_body = new JObject() { { "code", 502 }, { "message", "系统异常,描述：" + ex.Message }, { "body", null } }; }
            //// 写日志
            //finally { BasicHelper.SetWriteTxtLog("", ""); }
            return rsp_body;
        }
    }
}