using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 居民点专属信息
    /// </summary>
    public class JMDZSXX
    {
        public  string name = "居民点";
        /// <summary>
        /// 类型
        /// </summary>
        public string BNPX0017 { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string BNPX0020 { get; set; }
    }
    /// <summary>
    /// 居民点类
    /// </summary>
    public class JMD
    {
        /// <summary>
        /// 居民点专属信息
        /// </summary>
        public JMDZSXX jmdzsxx { get; set; }
    }
}