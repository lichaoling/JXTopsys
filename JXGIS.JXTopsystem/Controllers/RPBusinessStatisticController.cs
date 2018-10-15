using JXGIS.JXTopsystem.Business.RPBusinessStatistic;
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
        public ContentResult GetRPNumTJ(int PageSize, int PageNum, DateTime start, DateTime end, string DistrictID, string CommunityName, string RoadName, string Model, string Material, string Size)
        {
            RtObj rt = null;
            try
            {
                var r = RPStatisticUtils.GetRPNumTJ(PageSize, PageNum, start, end, DistrictID, CommunityName, RoadName, Model, Material, Size);
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
        public ContentResult GetRPRepairTJ(int PageSize, int PageNum, string DistrictID, string CommunityName, int RepairMode, int RepairedCount, string RepairParts, string RepairContent, string RepairFactory, int isFinishRepair, string FinishTimeStart, string FinishTimeEnd)
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
    }
}