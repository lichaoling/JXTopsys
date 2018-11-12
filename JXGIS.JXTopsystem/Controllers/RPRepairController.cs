using JXGIS.JXTopsystem.App_Start;
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
        [LoggerFilter(Description = "查询一条路牌的所有维修记录")]
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
        [LoggerFilter(Description = "查询一条维修记录详情")]
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
        [LoggerFilter(Description = "删除一条维修记录")]
        public ContentResult DeleteRPRepairByID(string RepairID)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RPRepairUtils.DeleteRPRepairByID(RepairID);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "修改一条维修记录")]
        public ContentResult ModifyRPRepair(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                RPRepairUtils.ModifyRPRepair(oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "路牌报修")]
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
    }
}
