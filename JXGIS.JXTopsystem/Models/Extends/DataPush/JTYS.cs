using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 海港、河港专属信息
    /// </summary>
    public class HGHGZSXX
    {
        public string name = "海港、河港";
        /// <summary>
        /// 类型
        /// </summary>
        public string BXBX0017 { get; set; }
    }
    /// <summary>
    /// 船闸升船机站专属信息
    /// </summary>
    public class CZSCJZZSXX
    {
        public string name = "船闸升船机站";
        /// <summary>
        /// 类型
        /// </summary>
        public string BXCX0017 { get; set; }
    }
    /// <summary>
    /// 公路专属信息
    /// </summary>
    public class GLZSXX
    {
        public string name = "公路";
        /// <summary>
        /// 类型
        /// </summary>
        public string AMDX0010 { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string BMDX0029 { get; set; }
        /// <summary>
        /// 途径行政区
        /// </summary>
        public string BMDX0033 { get; set; }
    }
    /// <summary>
    /// 铁路专属信息
    /// </summary>
    public class TLZSXX
    {
        public string name = "铁路";
        /// <summary>
        /// 类型1
        /// </summary>
        public string BXHX0017 { get; set; }
        /// <summary>
        /// 类型2
        /// </summary>
        public string BXHX0018 { get; set; }
        /// <summary>
        /// 主要车站
        /// </summary>
        public string BXHX0022 { get; set; }
        /// <summary>
        /// 途径行政区
        /// </summary>
        public string BXHX0023 { get; set; }

    }
    /// <summary>
    /// 火车站专属信息
    /// </summary>
    public class HCZZSXX
    {
        public string name = "火车站";
        /// <summary>
        /// 类型
        /// </summary>
        public string BXIX0017 { get; set; }
    }
    /// <summary>
    /// 航空港专属信息
    /// </summary>
    public class HKGZSXX
    {
        public string name = "航空港";
        /// <summary>
        /// 类型
        /// </summary>
        public string BXKX0017 { get; set; }
    }
    /// <summary>
    /// 道路街巷专属信息
    /// </summary>
    public class DLJXZSXX
    {
        public string name = "道路街巷";
        /// <summary>
        /// 道路等级
        /// </summary>
        public string BXNX0012 { get; set; }
    }
    /// <summary>
    /// 桥梁专属信息
    /// </summary>
    public class QLZSXX
    {
        public string name = "桥梁";
        /// <summary>
        ///类型
        /// </summary>
        public string BXRX0017 { get; set; }
        /// <summary>
        ///最大载重量（吨）
        /// </summary>
        public string BXRX0021 { get; set; }
        /// <summary>
        ///长度（米）
        /// </summary>
        public string BXRX0022 { get; set; }
    }
    /// <summary>
    /// 隧道专属信息
    /// </summary>
    public class SDZSXX
    {
        public string name = "隧道";
        /// <summary>
        ///类型
        /// </summary>
        public string BXSX0017 { get; set; }
        /// <summary>
        ///长度（米）
        /// </summary>
        public string BXSX0018 { get; set; }
    }
    /// <summary>
    /// 环岛路口专属信息
    /// </summary>
    public class HDLKZSXX
    {
        public string name = "环岛路口";
        /// <summary>
        ///类型
        /// </summary>
        public string BXVX0017 { get; set; }
    }
    /// <summary>
    /// 交通运输设施类
    /// </summary>
    public class JTYS
    {
        /// <summary>
        /// 海港、河港专属信息
        /// </summary>
        public HGHGZSXX hghgzsxx { get; set; }
        /// <summary>
        /// 船闸升船机站专属信息
        /// </summary>
        public CZSCJZZSXX czscjz { get; set; }
        /// <summary>
        /// 公路专属信息
        /// </summary>
        public GLZSXX glzsxx { get; set; }
        /// <summary>
        /// 铁路专属信息
        /// </summary>
        public TLZSXX tlzsxx { get; set; }
        /// <summary>
        /// 火车站专属信息
        /// </summary>
        public HCZZSXX hczzsxx { get; set; }
        /// <summary>
        /// 航空港专属信息
        /// </summary>
        public HKGZSXX hkgzsxx { get; set; }
        /// <summary>
        /// 道路街巷专属信息
        /// </summary>
        public DLJXZSXX dljxzsxx { get; set; }//
        /// <summary>
        /// 桥梁专属信息
        /// </summary>
        public QLZSXX qlzsxx { get; set; }//
        /// <summary>
        /// 隧道专属信息
        /// </summary>
        public SDZSXX sdzsxx { get; set; }
        /// <summary>
        /// 环岛路口专属信息
        /// </summary>
        public HDLKZSXX hdlkzsxx { get; set; }
    }
}