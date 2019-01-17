using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business.RPBusinessStatistic;
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
    public class RPBusinessStatisticController : Controller
    {
        [LoggerFilter(Description = "获取路牌数量统计")]
        public ContentResult GetRPNumTJ(int PageSize, int PageNum, DateTime? start, DateTime? end, string DistrictID, string CommunityName, string RoadName, string Model, string Manufacturers, string Material, string Size)
        {
            RtObj rt = null;
            try
            {
                var r = RPStatisticUtils.GetRPNumTJ(PageSize, PageNum, start, end, DistrictID, CommunityName, RoadName, Model, Manufacturers, Material, Size);
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
        [LoggerFilter(Description = "获取路牌维修统计")]
        public ContentResult GetRPRepairTJ(int PageSize, int PageNum, string DistrictID, string CommunityName, string RepairMode, string RepairParts, string RepairContent, string RepairFactory, DateTime? FinishTimeStart, DateTime? FinishTimeEnd, int RepairedCount = -1, int isFinishRepair = Enums.Complete.All)
        {
            RtObj rt = null;
            try
            {
                var r = RPStatisticUtils.GetRPRepairTJ(PageSize, PageNum, DistrictID, CommunityName, RepairMode, RepairedCount, RepairParts, RepairContent, RepairFactory, isFinishRepair, FinishTimeStart, FinishTimeEnd);
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


        public JsonResult GetConditionOfRPRepairTJ(string DistrictID, string CommunityName, string RepairMode, string RepairParts, string RepairContent, string RepairFactory, DateTime? FinishTimeStart, DateTime? FinishTimeEnd, int RepairedCount = -1, int isFinishRepair = Enums.Complete.All)
        {
            RtObj rt = null;
            try
            {
                Session["_RPRepairTJDistrictID"] = DistrictID;
                Session["_RPRepairTJCommunityName"] = CommunityName;
                Session["_RPRepairTJRepairMode"] = RepairMode;
                Session["_RPRepairTJRepairParts"] = RepairParts;
                Session["_RPRepairTJRepairContent"] = RepairContent;
                Session["_RPRepairTJRepairFactory"] = RepairFactory;
                Session["_RPRepairTJFinishTimeStart"] = FinishTimeStart;
                Session["_RPRepairTJFinishTimeEnd"] = FinishTimeEnd;
                Session["_RPRepairTJRepairedCount"] = RepairedCount;
                Session["_RPRepairTJisFinishRepair"] = isFinishRepair;
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "导出路牌维护统计")]
        public ActionResult ExportRPRepairTJ()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();

                var DistrictID = Session["_RPRepairTJDistrictID"] != null ? Session["_RPRepairTJDistrictID"].ToString() : null;
                var CommunityName = Session["_RPRepairTJCommunityName"] != null ? Session["_RPRepairTJCommunityName"].ToString() : null;
                var RepairMode = Session["_RPRepairTJRepairMode"] != null ? Session["_RPRepairTJRepairMode"].ToString() : null;
                var RepairParts = Session["_RPRepairTJRepairParts"] != null ? Session["_RPRepairTJRepairParts"].ToString() : null;
                var RepairContent = Session["_RPRepairTJRepairContent"] != null ? Session["_RPRepairTJRepairContent"].ToString() : null;
                var RepairFactory = Session["_RPRepairTJRepairFactory"] != null ? Session["_RPRepairTJRepairFactory"].ToString() : null;
                var FinishTimeStart = Session["_RPRepairTJFinishTimeStart"] != null ? (DateTime?)Session["_RPRepairTJFinishTimeStart"] : null;
                var FinishTimeEnd = Session["_RPRepairTJFinishTimeEnd"] != null ? (DateTime?)Session["_RPRepairTJFinishTimeEnd"] : null;
                var RepairedCount = Session["_RPRepairTJRepairedCount"] != null ? (int)Session["_RPRepairTJRepairedCount"] : -1;
                var isFinishRepair = Session["_RPRepairTJisFinishRepair"] != null ? (int)Session["_RPRepairTJisFinishRepair"] : 2;
                
                var ms = RPStatisticUtils.ExportRPRepairTJ(DistrictID, CommunityName, RepairMode, RepairedCount, RepairParts, RepairContent, RepairFactory, isFinishRepair, FinishTimeStart, FinishTimeEnd);
                Session["_RPRepairTJDistrictID"] = null;
                Session["_RPRepairTJCommunityName"] = null;
                Session["_RPRepairTJRepairMode"] = null;
                Session["_RPRepairTJRepairParts"] = null;
                Session["_RPRepairTJRepairContent"] = null;
                Session["_RPRepairTJRepairFactory"] = null;
                Session["_RPRepairTJFinishTimeStart"] = null;
                Session["_RPRepairTJFinishTimeEnd"] = null;
                Session["_RPRepairTJRepairedCount"] = null;
                Session["_RPRepairTJisFinishRepair"] = null;
                string fileName = $"路牌维护统计{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
                return File(ms, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }
        }
    }
}