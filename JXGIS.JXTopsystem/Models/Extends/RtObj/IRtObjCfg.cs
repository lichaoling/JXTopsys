using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.RtObj
{
    /// <summary>
    /// RtObj 配置接口，用于扩展RtObj的配置，继承该接口并替换RtObj中的默认配置可实现灵活的错误信息配置和记录。
    /// </summary>
    public interface IRtObjCfg
    {
        /// <summary>
        /// 获取默认的错误信息
        /// </summary>
        /// <returns></returns>
        string GetDefaultErrorMessage();

        /// <summary>
        /// 获取是否显示原始错误信息
        /// </summary>
        /// <returns></returns>
        bool GetShowOrginErrorMessage();

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="ex"></param>
        void Log(Exception ex);
    }
}