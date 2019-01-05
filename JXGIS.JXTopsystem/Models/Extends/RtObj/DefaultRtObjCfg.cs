using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.RtObj
{
    public class DefaultRtObjCfg : IRtObjCfg
    {
        /// <summary>
        /// 从App config中获取配置，确保app config中有name为defaultErrorMessage的节，没有的话提供默认值
        /// </summary>
        /// <returns></returns>
        public string GetDefaultErrorMessage()
        {
            string defaultErrorMessage = System.Configuration.ConfigurationManager.AppSettings["defaultErrorMessage"];
            return string.IsNullOrEmpty(defaultErrorMessage) ? "处理数据的过程中发生了错误，请联系系统管理员！" : defaultErrorMessage;
        }

        /// <summary>
        /// 从App config中获取配置，确保app config中有name为showOrginErrorMessage的节，没有的话始终为false，showOrginErrorMessage的值只能为"true"（返回true）和其它值（返回false)
        /// </summary>
        /// <returns></returns>
        public bool GetShowOrginErrorMessage()
        {
            return System.Configuration.ConfigurationManager.AppSettings["showOrginErrorMessage"] == "true";
        }

        /// <summary>
        /// 在运行目录下新增Log文件夹，并以日期生成日志文件，将错误信息写入日志
        /// </summary>
        /// <param name="ex"></param>
        public void Log(Exception ex)
        {
            var logPath = StaticVariable.basePathLogMessage;
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            var logFileName = Path.Combine(logPath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs);
                string esm = $"======={DateTime.Now.ToString()}{System.Environment.NewLine}|Message:{ex.Message}{System.Environment.NewLine}|StackTrace:{ex.StackTrace}{System.Environment.NewLine}";
                sw.Write(esm);
            }
            catch
            {

            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }
    }
}