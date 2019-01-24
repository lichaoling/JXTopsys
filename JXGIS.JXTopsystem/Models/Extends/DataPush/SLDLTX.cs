using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 水库专属信息
    /// </summary>
    public class SKZSXX
    {
        public  string name = "水库";
        /// <summary>
        /// 面积（平方千米）
        /// </summary>
        public string BYCX0017 { get; set; }
        /// <summary>
        /// 总库容（万立方米）
        /// </summary>
        public string BYCX0021 { get; set; }
    }
    /// <summary>
    ///发电站专属信息
    /// </summary>
    public class FDZZSXX
    {
        public  string name = "发电站";
        /// <summary>
        /// 类型
        /// </summary>
        public string BYMX0017 { get; set; }
    }
    /// <summary>
    /// 水利、电力、通讯设施类
    /// </summary>
    public class SLDLTX
    {
        /// <summary>
        /// 水库专属信息
        /// </summary>
        public SKZSXX skzsxx { get; set; }
        /// <summary>
        ///发电站专属信息
        /// </summary>
        public FDZZSXX fdzzsxx { get; set; }
    }
}