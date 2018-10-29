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
        // GET: ArchiveFile
        public ActionResult ArchiveMPFile(DateTime? start, DateTime? end)
        {
            RtObj rt = null;
            try
            {
               ArchiveFile.ArchiveMPFile(start,end);
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