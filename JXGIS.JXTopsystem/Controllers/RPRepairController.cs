using JXGIS.JXTopsystem.Business.RPRepair;
using JXGIS.JXTopsystem.Models.Extends;
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
        public ContentResult SearchRPRepairByID(string ID, int RPRange = Enums.RPRange.All)
        {
            RtObj rt = null;
            try
            {
                var r = RPRepairUtils.SearchRPRepairByID(ID, RPRange);
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
        public ContentResult GetNewRPRepair(string RPID)
        {
            RtObj rt = null;
            try
            {
                var r = RPRepairUtils.GetNewRPRepair(RPID);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult SearchRPRepairDetailByID(string RepairID)
        {
            RtObj rt = null;
            try
            {
                var r = RPRepairUtils.SearchRPRepairDetailByID(RepairID);
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

        public JsonResult RepairOrChangeRP(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RPRepairUtils.RepairOrChangeRP(oldDataJson);
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
