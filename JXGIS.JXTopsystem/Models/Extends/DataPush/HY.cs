using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 海洋专属信息
    /// </summary>
    public class HYZSXX
    {
        public string name = "海洋";
        /// <summary>
        /// 潮汐类型
        /// </summary>
        public string BMNX0017 { get; set; }
    }
    /// <summary>
    /// 海湾专属信息
    /// </summary>
    public class HWZSXX
    {
        public string name = "海湾";
        /// <summary>
        /// 所在海洋
        /// </summary>
        public string BMOX0028 { get; set; }
    }
    /// <summary>
    /// 岛屿专属信息
    /// </summary>
    public class DYZSXX
    {
        public string name = "岛屿";
        /// <summary>
        /// 面积（平方千米）
        /// </summary>
        public string BMQX0021 { get; set; }
    }
    /// <summary>
    /// 群岛专属信息
    /// </summary>
    public class QDZSXX
    {
        public string name = "群岛";
        /// <summary>
        /// 类型 详见：BL102，群岛类型代码表
        /// </summary>
        public string BMRX0017 { get; set; }
        /// <summary>
        /// 面积（平方千米）
        /// </summary>
        public string BMRX0026 { get; set; }
    }
    /// <summary>
    /// 半岛、岬角专属信息
    /// </summary>
    public class BDJJZSXX
    {
        public string name = "半岛、岬角";
        /// <summary>
        /// 类型  详见：BL103，半岛、岬角类型代码表
        /// </summary>
        public string BMSX0017 { get; set; }
        /// <summary>
        /// 面积（平方米）
        /// </summary>
        public string BMSX0018 { get; set; }
    }
    /// <summary>
    /// 滩涂、海岸专属信息
    /// </summary>
    public class TTHAZSXX
    {
        public string name = "半滩涂、海岸";
        /// <summary>
        /// 所在海洋
        /// </summary>
        public string BMTX0022 { get; set; }
    }

    /// <summary>
    ///海域类
    /// </summary>
    public class HY
    {
        /// <summary>
        /// 海洋专属信息
        /// </summary>
        public HYZSXX hyzsxx { get; set; }
        /// <summary>
        /// 海湾专属信息
        /// </summary>
        public HWZSXX hwzsxx { get; set; }
        /// <summary>
        /// 岛屿专属信息
        /// </summary>
        public DYZSXX dyzsxx { get; set; }
        /// <summary>
        /// 群岛专属信息
        /// </summary>
        public QDZSXX qdzsxx { get; set; }
        /// <summary>
        /// 半岛、岬角专属信息
        /// </summary>
        public BDJJZSXX bdjjzsxx { get; set; }
        /// <summary>
        /// 滩涂、海岸专属信息
        /// </summary>
        public TTHAZSXX tthazsxx { get; set; }
    }
}