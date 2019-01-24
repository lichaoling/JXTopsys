using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 人物事件纪念地专属信息
    /// </summary>
    public class RWSJJNDZSXX
    {
        public  string name = "人物事件纪念地";
        /// <summary>
        ///管理单位
        /// </summary>
        public string BNQX0018 { get; set; }
        /// <summary>
        ///人物或事件
        /// </summary>
        public string BNQX0022 { get; set; }
    }
    /// <summary>
    /// 公园风景区专属信息
    /// </summary>
    public class GYFJQZSXX
    {
        public  string name = "公园风景区";
        /// <summary>
        ///管理单位
        /// </summary>
        public string BNSX0018 { get; set; }
    }
    /// <summary>
    /// 自然保护区专属信息
    /// </summary>
    public class ZRBHQZSXX
    {
        public  string name = "自然保护区";
        /// <summary>
        ///管理单位
        /// </summary>
        public string BNTX0020 { get; set; }
    }
    /// <summary>
    ///纪念地与旅游景点类
    /// </summary>
    public class LYJD
    {
        /// <summary>
        /// 人物事件纪念地专属信息
        /// </summary>
        public RWSJJNDZSXX rwsjjndzsxx { get; set; }
        /// <summary>
        /// 公园风景区专属信息
        /// </summary>
        public GYFJQZSXX gyfjqzsxx { get; set; }
        /// <summary>
        /// 自然保护区专属信息
        /// </summary>
        public ZRBHQZSXX zrbhqzsxx { get; set; }
    }
}