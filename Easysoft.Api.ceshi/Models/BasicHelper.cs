using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Easysoft.Api.ceshi
{
    /// <summary>
    /// 基础工具类
    /// </summary>
    public static class BasicHelper
    {
        #region## 获取Http.Request的各类型值
        /// <summary>
        /// 获取HTTP的QueryString值
        /// </summary>
        public static string GetRequestParams(string key)
        {
            if (HttpContext.Current.Request[key] == null) return "";
            return Regex.Unescape(HttpUtility.HtmlDecode(HttpContext.Current.Request[key].ToString()));
        }
        /// <summary>
        /// 获取HTTP的头部参数(Header)值
        /// </summary>
        public static string GetRequestHeader(string key)
        {
            if (HttpContext.Current.Request.Headers[key] == null) return "";
            return Regex.Unescape(HttpUtility.HtmlDecode(HttpContext.Current.Request.Headers[key].ToString()));
        }
        /// <summary>
        /// 获取HTTP的表单参数(Form)值
        /// </summary>
        public static string GetRequestForm(string key)
        {
            if (HttpContext.Current.Request.Form[key] == null) return "";
            return Regex.Unescape(HttpUtility.HtmlDecode(HttpContext.Current.Request.Form[key].ToString()));
        }
        /// <summary>
        /// 获取HTTP的QueryString值
        /// </summary>
        public static string GetRequestParams(HttpContext context, string key)
        {
            if (context.Request[key] == null) return "";
            return HttpUtility.HtmlDecode(context.Request[key].ToString());
        }
        /// <summary>
        /// 获取HTTP的头部参数(Header)值
        /// </summary>
        public static string GetRequestHeader(HttpContext context, string key)
        {
            if (context.Request.Headers[key] == null) return "";
            return HttpUtility.HtmlDecode(context.Request.Headers[key].ToString());
        }
        /// <summary>
        /// 获取HTTP的表单参数(Form)值
        /// </summary>
        public static string GetRequestForm(HttpContext context, string key)
        {
            if (context.Request.Form[key] == null) return "";
            return HttpUtility.HtmlDecode(context.Request.Form[key].ToString());
        }
        #endregion

        /// <summary>
        /// 生成随机数(数字+字母)
        /// </summary>
        /// <param name="len">随机数长度</param>
        public static string CreateRandom(int len)
        {
            string word = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            //随机类
            Random ra = new Random();
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                //拼接字符
                result.Append(word[ra.Next(62)]);
            }
            return result.ToString();
        }
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
        public static bool JsonLookUpKey(JObject data, string key)
        {
            if (data == null) { return false; }
            if (!(data is IDictionary tdictionary)) { return false; }
            if (tdictionary.Contains(key)) { return true; }
            else { return false; }
        }
        /// <summary>
        /// 日志写入文件(追加内容)
        /// </summary>
        /// <param name="fileName">保存的文件名</param>
        /// <param name="content">日志内容</param>
        public static void SetWriteTxtLog(string fileName, string content)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "TempFile";
            DirectoryInfo dir = new DirectoryInfo(filePath);
            if (!Directory.Exists(filePath)) { dir.Create(); }
            //拼接日志文件的全量地址
            string fullPath = filePath + "\\" + fileName + ".txt";
            File.AppendAllText(fullPath, content, Encoding.GetEncoding("utf-8"));
        }
        /// <summary>
        /// DataTable转为JObject(JSON数据)
        /// </summary>
        /// <param name="dt">DataTable数据集</param>
        public static JObject FnDataTableToJObject(DataTable dt)
        {
            JObject data = new JObject();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.DataType == typeof(int)) { data[column.ColumnName] = (int)dt.Rows[0][column.ColumnName]; }
                    else if (column.DataType == typeof(double)) { data[column.ColumnName] = (double)dt.Rows[0][column.ColumnName]; }
                    else if (column.DataType == typeof(decimal)) { data[column.ColumnName] = (decimal)dt.Rows[0][column.ColumnName]; }
                    else if (column.DataType == typeof(float)) { data[column.ColumnName] = (float)dt.Rows[0][column.ColumnName]; }
                    else if (column.DataType == typeof(Byte[]))
                    {
                        if (dt.Rows[0][column.ColumnName] != DBNull.Value)
                            data[column.ColumnName] = "data:image/png;base64," + Convert.ToBase64String((byte[])dt.Rows[0][column.ColumnName]);
                        else
                            data[column.ColumnName] = null;
                    }
                    else { data[column.ColumnName] = dt.Rows[0][column.ColumnName].ToString(); }
                }
            }
            return data;
        }
        /// <summary>
        /// DataTable转为JArray(JSON数组)
        /// </summary>
        /// <param name="dt">DataTable数据集</param>
        public static JArray FnDataTableToJArray(DataTable dt)
        {
            JArray datas = new JArray();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    JObject data = new JObject();
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.DataType == typeof(int)) { data[column.ColumnName] = (int)row[column.ColumnName]; }
                        else if (column.DataType == typeof(double)) { data[column.ColumnName] = (double)row[column.ColumnName]; }
                        else if (column.DataType == typeof(decimal)) { data[column.ColumnName] = (decimal)row[column.ColumnName]; }
                        else if (column.DataType == typeof(float)) { data[column.ColumnName] = (float)row[column.ColumnName]; }
                        else if (column.DataType == typeof(Byte[]))
                        {
                            if (row[column.ColumnName] != DBNull.Value)
                                data[column.ColumnName] = "data:image/png;base64," + Convert.ToBase64String((byte[])row[column.ColumnName]);
                            else
                                data[column.ColumnName] = null;
                        }
                        else { data[column.ColumnName] = row[column.ColumnName].ToString(); }
                    }
                    datas.Add(data);
                }
            }
            return datas;
        }
        /// <summary>
        /// JSON转DataTable
        /// </summary>
        /// <param name="strJson">JSON内容</param>
        /// <param name="tablename">表名</param>
        public static DataTable JsonToDataTable(string strJson, string tablename)
        {
            DataTable dt = null;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                if (strJson.StartsWith("{")) //JSON对象
                {
                    Dictionary<string, object> dicPairs = (Dictionary<string, object>)javaScriptSerializer.DeserializeObject(strJson);
                    if (dt == null) //构造表列名
                    {
                        dt = new DataTable { TableName = tablename };
                        foreach (var key in dicPairs.Keys)
                            dt.Columns.Add(key, typeof(string));
                    }
                    // 录入数据
                    DataRow row = dt.NewRow();
                    foreach (var item in dicPairs)
                        row[item.Key] = item.Value;
                    dt.Rows.Add(row);
                }
                else //JSON数组
                {
                    ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(strJson);
                    if (arrayList.Count > 0)
                    {
                        foreach (Dictionary<string, object> dicRow in arrayList)
                        {
                            //构造表列名
                            if (dt == null)
                            {
                                dt = new DataTable { TableName = tablename };
                                foreach (KeyValuePair<string, object> key in dicRow)
                                {
                                    DataColumn dc = new DataColumn { ColumnName = key.Key.Trim() };
                                    dt.Columns.Add(dc);
                                }
                                dt.AcceptChanges();
                            }
                            //增加类容
                            DataRow dr = dt.NewRow();
                            foreach (KeyValuePair<string, object> kpCol in dicRow)
                            {
                                dr[kpCol.Key] = kpCol.Value;
                            }
                            dt.Rows.Add(dr);
                            dt.AcceptChanges();
                        }
                    }
                }
            }
            catch { dt = null; }
            return dt;
        }
        /// <summary>
        /// 把请求要素按照"参数=参数值"的模式用"&"字符拼接成字符串
        /// </summary>
        /// <param name="para">请求要素</param>
        /// <param name="sort">是否需要根据key值作升序排列</param>
        /// <param name="encode">是否需要URL编码</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>拼接成的字符串</returns>
        public static string CreateLinkString(Dictionary<string, string> para, bool sort, bool encode, Encoding encoding)
        {
            if (para == null || para.Count == 0) return "";

            List<string> list = new List<string>(para.Keys);
            if (sort) list.Sort(StringComparer.Ordinal);

            StringBuilder sb = new StringBuilder();
            foreach (string key in list)
            {
                string value = para[key];
                if (encode && value != null)
                {
                    try { value = HttpUtility.UrlEncode(value, encoding); }
                    catch (Exception ex) { return "#ERROR: HttpUtility.UrlEncode Error!" + ex.Message; }
                }
                sb.Append(key).Append("=").Append(value).Append("&");
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }
    }
}