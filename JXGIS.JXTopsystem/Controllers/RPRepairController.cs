using JXGIS.JXTopsystem.Business.RPRepair;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class RPRepairController : Controller
    {
        public ContentResult SearchRPRepairByID(string ID)
        {
            RtObj rt = null;
            try
            {
                var r = RPRepairUtils.SearchRPRepairByID(ID);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Content(s);
        }

        public JsonResult RepairOrChangeRP(string ID, string Model, string Size, string Material, string Manufacturers, Models.Entities.RPRepair rpRepairInfo, int repairMode)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RPRepairUtils.RepairOrChangeRP(ID, Model, Size, Material, Manufacturers, rpRepairInfo, repairMode);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

        public JsonResult AddRPRepairContent(string content)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RPRepairUtils.AddRPRepairContent(content);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
    }
}
