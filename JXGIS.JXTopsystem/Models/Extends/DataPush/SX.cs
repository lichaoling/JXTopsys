using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 河流专属信息
    /// </summary>
    public class HLZSXX
    {
        public string name = "河流";
        /// <summary>
        /// 长度（千米）
        /// </summary>
        public string AMEX0008 { get; set; }
        /// <summary>
        /// 河流级别
        /// </summary>
        public string BMEX0028 { get; set; }
        /// <summary>
        /// 河流类别1
        /// </summary>
        public string BMEX0029 { get; set; }
        /// <summary>
        /// 河流类别2
        /// </summary>
        public string BMEX0030 { get; set; }
        /// <summary>
        /// 河流类别3
        /// </summary>
        public string BMEX0031 { get; set; }
    }

    /// <summary>
    /// 河湾专属信息
    /// </summary>
    public class HEWZSXX
    {
        public string name = "河湾";
        /// <summary>
        /// 所属河流
        /// </summary>
        public string BMYX0017 { get; set; }
    }
    /// <summary>
    /// 河口专属信息
    /// </summary>
    public class HEKZSXX
    {
        public string name = "河口";
        /// <summary>
        /// 类型
        /// </summary>
        public string BMZX0017 { get; set; }
    }
    /// <summary>
    /// 湖泊专属信息
    /// </summary>
    public class HPZSXX
    {
        public string name = "湖泊";
        /// <summary>
        /// 类型1
        /// </summary>
        public string BMFX0030 { get; set; }
        /// <summary>
        /// 类型2
        /// </summary>
        public string BMFX0031 { get; set; }
    }
    /// <summary>
    /// 陆地岛屿专属信息
    /// </summary>
    public class LDDYZSXX
    {
        public string name = "陆地岛屿";
        /// <summary>
        /// 类型
        /// </summary>
        public string BLFX0017 { get; set; }
        /// <summary>
        /// 面积（平方千米）
        /// </summary>
        public string BLFX0021 { get; set; }
    }
    /// <summary>
    /// 泉专属信息
    /// </summary>
    public class QZSXX
    {
        public string name = "泉";
        /// <summary>
        /// 类型
        /// </summary>
        public string BLIX0023 { get; set; }
    }

    /// <summary>
    ///（陆地）水系类
    /// </summary>
    public class SX
    {
        /// <summary>
        /// 河流专属信息
        /// </summary>
        public HLZSXX hlzsxx { get; set; }
        /// <summary>
        /// 河湾专属信息
        /// </summary>
        public HEWZSXX hewzsxx { get; set; }
        /// <summary>
        /// 河口专属信息
        /// </summary>
        public HEKZSXX hkzsxx { get; set; }
        /// <summary>
        /// 湖泊专属信息
        /// </summary>
        public HPZSXX hpzsxx { get; set; }
        /// <summary>
        /// 陆地岛屿专属信息
        /// </summary>
        public LDDYZSXX lddyzsxx { get; set; }
        /// <summary>
        /// 泉专属信息
        /// </summary>
        public QZSXX qzsxx { get; set; }
    }
}