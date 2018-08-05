using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.RtObj
{
    /// <summary>
    /// 自定义错误类型，用于区分 可预期错误（自定义错误）和系统其它错误 throw new Error()
    /// </summary>
    public class Error:Exception
    {
        public Error(string erMsg) : base(erMsg)
        {

        }
    }
}