using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class ArchiveFileUtils
    {
        public class servicecode
        {
            public static string dmhz
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicecode.dmhz.ToString();
                }
            }
            public static string cjyj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicecode.cjyj.ToString();
                }
            }
            public static string mpzm
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicecode.mpzm.ToString();
                }
            }
            public static string dmzm
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicecode.dmzm.ToString();
                }
            }
        }
        public class servicetype
        {
            public static string bj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicetype.bj.ToString();
                }
            }
            public static string cnj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicetype.cnj.ToString();
                }
            }
            public static string qt
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicetype.qt.ToString();
                }
            }
        }
        public class servicename
        {
            public static string dmhz
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicename.dmhz.ToString();
                }
            }
            public static string cjyj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicename.cjyj.ToString();
                }
            }
            public static string mpzm
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicename.mpzm.ToString();
                }
            }
            public static string dmzm
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.servicename.dmzm.ToString();
                }
            }
        }

        public class deptname
        {
            public static string smzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.smzj.ToString();
                }
            }
            public static string nhqmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.nhqmzj.ToString();
                }
            }
            public static string xzqmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.xzqmzj.ToString();
                }
            }
            public static string hnsmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.hnsmzj.ToString();
                }
            }
            public static string phsmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.phsmzj.ToString();
                }
            }
            public static string txsmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.txsmzj.ToString();
                }
            }
            public static string jsxmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.jsxmzj.ToString();
                }
            }
            public static string hyxmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.hyxmzj.ToString();
                }
            }
            public static string sdmb
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptname.sdmb.ToString();
                }
            }
        }
        public class deptid
        {
            public static string smzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.smzj.ToString();
                }
            }
            public static string nhqmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.nhqmzj.ToString();
                }
            }
            public static string xzqmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.xzqmzj.ToString();
                }
            }
            public static string hnsmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.hnsmzj.ToString();
                }
            }
            public static string phsmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.phsmzj.ToString();
                }
            }
            public static string txsmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.txsmzj.ToString();
                }
            }
            public static string jsxmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.jsxmzj.ToString();
                }
            }
            public static string hyxmzj
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.hyxmzj.ToString();
                }
            }
            public static string sdmb
            {
                get
                {
                    return SystemUtils.Config.ArchiveFilesPara.deptid.sdmb.ToString();
                }
            }
        }

        public class eventtype
        {
            public static string mpzm = "其他行政权力";
            public static string dmhz = "行政确认";
            public static string cjyj = "其他行政权力";
            public static string dmzm = "公共服务";
        }

        public class eventtypeid
        {
            public static string mpzm = "10";
            public static string dmhz = "08";
            public static string cjyj = "10";
            public static string dmzm = "14";
        }
        public class eventtypebigid
        {
            public static string mpzm = "02875";
            public static string dmhz = "00320";
            public static string cjyj = "02442";
            public static string dmzm = "00325";
        }
        public static string eventtypesmallid = "000";
    }
}