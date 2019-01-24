using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 单位专属信息
    /// </summary>
    public class DWZSXX
    {
        public  string name = "单位";
        /// <summary>
        /// 地址
        /// </summary>
        public string BNYX0017 { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string BNYX0022 { get; set; }
    }
    /// <summary>
    /// 单位类
    /// </summary>
    public class DW
    {
        /// <summary>
        /// 单位专属信息
        /// </summary>
        public DWZSXX dwzsxx { get; set; }
    }
}