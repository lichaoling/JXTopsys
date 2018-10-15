using JXGIS.JXTopsystem.Business.RPSearch;
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
    public class RPSearchController : Controller
    {
        public ContentResult SearchRP(int PageSize, int PageNum, string DistrictID, string RoadName, string Intersection, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, DateTime start, DateTime end, int UseState=Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = RPSearchUtils.SearchRP(PageSize, PageNum, DistrictID, RoadName, Intersection, Model, Size, Material, Manufacturers, FrontTagline, BackTagline, start, end, UseState);
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

        public ContentResult SearchRPByID(string RPID)
        {
            RtObj rt = null;
            try
            {
                var r = RPSearchUtils.SearchRPByID(RPID);
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