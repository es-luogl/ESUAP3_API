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
        /// JSON数组转为DataTable
        /// </summary>
        public static DataTable JArrayToDataTable(string strJson, DataTable dt)
        {
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(strJson);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dicRow in arrayList)
                    {
                        //构造表列名
                        if (dt == null)
                        {
                            dt = new DataTable();
                            foreach (KeyValuePair<string, object> key in dicRow)
                            {
                                DataColumn dc = new DataColumn();
                                dc.ColumnName = key.Key.Trim();
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
            catch { dt = null; }
            return dt;
        }
    }
}