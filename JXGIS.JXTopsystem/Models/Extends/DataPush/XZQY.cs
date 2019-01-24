using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 行政区域专属信息
    /// </summary>
    public class XZQYZSXX
    {
        public string name = "行政区域";
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string AMAX0013 { get; set; }
        /// <summary>
        /// 总面积（平方千米）
        /// </summary>
        public string AMAX0015 { get; set; }
        /// <summary>
        /// 行政区划单位
        /// </summary>
        public string BMAX0030 { get; set; }
        /// <summary>
        ///行政级别
        /// </summary>
        public string BMAX0031 { get; set; }
    }
    /// <summary>
    ///行政区域类
    /// </summary>
    public class XZQY
    {
        /// <summary>
        /// 行政区域专属信息
        /// </summary>
        public XZQYZSXX xzqyzsxx { get; set; }
    }
}