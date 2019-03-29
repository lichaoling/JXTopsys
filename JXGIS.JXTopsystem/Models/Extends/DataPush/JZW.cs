using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 建筑物综合专属信息
    /// </summary>
    public class JZWZHZSXX
    {
        public  string name = "建筑物综合";
        /// <summary>
        ///所在位置
        /// </summary>
        public string AMBX0010 { get; set; }
        /// <summary>
        ///邮政编码
        /// </summary>
        public string AMBX0020 { get; set; }
    }
    /// <summary>
    /// 房屋专属信息
    /// </summary>
    public class FWZSXX
    {
        public  string name = "房屋";
        /// <summary>
        /// 地址
        /// </summary>
        public string BNUX0011 { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string BNUX0016 { get; set; }
    }
    /// <summary>
    /// 亭台碑塔专属信息
    /// </summary>
    public class TTBTZSXX
    {
        public  string name = "亭台碑塔";
        /// <summary>
        /// 建筑结构
        /// </summary>
        public string BNVX0032 { get; set; }
    }
    /// <summary>
    /// 广场体育场专属信息
    /// </summary>
    public class GCTYCZSXX
    {
        public  string name = "广场体育场";
        /// <summary>
        /// 所在位置
        /// </summary>
        public string BNWX0013 { get; set; }
        /// <summary>
        /// 占地面积（平方米）
        /// </summary>
        public string BNWX0014 { get; set; }
    }
    /// <summary>
    /// 建筑物类
    /// </summary>
    public class JZW
    {
        /// <summary>
        /// 建筑物综合专属信息
        /// </summary>
        public JZWZHZSXX jzwzhzsxx { get; set; }
        /// <summary>
        /// 房屋专属信息
        /// </summary>
        public FWZSXX fwzsxx { get; set; }
        /// <summary>
        /// 亭台碑塔专属信息
        /// </summary>
        public TTBTZSXX ttbtzsxx { get; set; }
        /// <summary>
        /// 广场体育场专属信息
        /// </summary>
        public GCTYCZSXX gctyczsxx { get; set; }
    }
}