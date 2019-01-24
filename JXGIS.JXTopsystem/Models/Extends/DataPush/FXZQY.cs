using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 矿区专属信息
    /// </summary>
    public class KQZSXX
    {
        public  string name = "矿区";
        /// <summary>
        /// 驻地
        /// </summary>
        public string BNHX0019 { get; set; }
    }
    /// <summary>
    /// 农林牧渔专属信息
    /// </summary>
    public class NLMYZSXX
    {
        public  string name = "农林牧渔";
        /// <summary>
        /// 类型
        /// </summary>
        public string BNIX0017 { get; set; }
        /// <summary>
        /// 面积（平方千米）
        /// </summary>
        public string BNIX0019 { get; set; }
    }
    /// <summary>
    /// 工业区、开发区专属信息
    /// </summary>
    public class GYQKFQZSXX
    {
        public  string name = "工业务、开发区";
        /// <summary>
        /// 类型
        /// </summary>
        public string BNJX0017 { get; set; }
        /// <summary>
        /// 面积（平方千米）
        /// </summary>
        public string BNJX0019 { get; set; }
    }
    /// <summary>
    /// 边贸区口岸专属信息
    /// </summary>
    public class BMQKAZSXX
    {
        public  string name = "边贸区口岸";
        /// <summary>
        /// 类型
        /// </summary>
        public string BNKX0017 { get; set; }
        /// <summary>
        /// 面积（平方千米）
        /// </summary>
        public string BNKX0019 { get; set; }
    }
    /// <summary>
    /// 非行政区域
    /// </summary>
    public class FXZQY
    {
        /// <summary>
        /// 矿区专属信息
        /// </summary>
        public KQZSXX kqzsxx { get; set; }
        /// <summary>
        /// 农林牧渔专属信息
        /// </summary>
        public NLMYZSXX nlmyzsxx { get; set; }
        /// <summary>
        /// 工业区、开发区专属信息
        /// </summary>
        public GYQKFQZSXX gyqkfqzsxx { get; set; }
        /// <summary>
        /// 边贸区口岸专属信息
        /// </summary>
        public BMQKAZSXX bmqkazsxx { get; set; }
    }
}