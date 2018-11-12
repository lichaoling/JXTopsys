using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business.ArchiveFile;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class ArchiveFileController : Controller
    {
        [LoggerFilter(Description = "门牌编制及地址证明事项电子文件文档")]
        public ActionResult ArchiveMPFile(DateTime? start, DateTime? end)
        {
            RtObj rt = null;
            try
            {
                ArchiveFile.ArchiveMPFile(start, end);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
    }
}