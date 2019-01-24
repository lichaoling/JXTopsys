using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.DataPush
{
    /// <summary>
    /// 群众自治组织专属信息
    /// </summary>
    public class QZZZZZZSXX
    {
        public  string name = "群众自治组织";
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string BNOX0021 { get; set; }
    }
    /// <summary>
    /// 群众自治类
    /// </summary>
    public class QZZZ
    {
        /// <summary>
        /// 群众自治组织专属信息
        /// </summary>
        public QZZZZZZSXX qzzzzzzsxx { get; set; }
    }
}