using JXGIS.JXTopsystem.Business.MPSearch;
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
    public class MPSearchController : Controller
    {
        /// <summary>
        /// 导出门牌的查询条件
        /// </summary>
        private static string DistrictID = null;
        private static string Name = null;
        private static int MPNumberType = 0;

        #region 住宅门牌
        public ContentResult SearchResidenceMP(int PageSize, int PageNum, string DistrictID, string Name, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = ResidenceMPSearch.SearchResidenceMP(PageSize, PageNum, DistrictID, Name, UseState);
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
        public ContentResult SearchResidenceMPByID(string ID)
        {
            RtObj rt = null;
            try
            {
                var r = ResidenceMPSearch.SearchResidenceMPByID(ID);
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
        public JsonResult GetConditionOfResidenceMP(string DistrictID, string Name, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                Session["_ResidenceMPDistrictID"] = DistrictID;
                Session["_ResidenceMPName"] = Name;
                Session["_ResidenceMPUseState"] = UseState;
                //MPSearchController.DistrictID = DistrictID;
                //MPSearchController.Name = Name;
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public ActionResult ExportResidenceMP()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                var Did = Session["_ResidenceMPDistrictID"].ToString();
                var Name = Session["_ResidenceMPName"].ToString();
                var UseState = (int)Session["_ResidenceMPUseState"];
                var ms = ResidenceMPSearch.ExportResidenceMP(Did, Name, UseState);
                Session["_ResidenceMPDistrictID"] = null;
                Session["_ResidenceMPName"] = null;
                Session["_ResidenceMPUseState"] = Enums.UseState.Enable;
                string fileName = $"住宅门牌_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xls";
                return File(ms, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 道路门牌
        public ContentResult SearchRoadMP(int PageSize, int PageNum, string DistrictID, string Name, int MPNumberType = 0, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = RoadMPSearch.SearchRoadMP(PageSize, PageNum, DistrictID, Name, MPNumberType, UseState);
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
        public ContentResult SearchRoadMPByID(string ID)
        {
            RtObj rt = null;
            try
            {
                var r = RoadMPSearch.SearchRoadMPByID(ID);
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
        public JsonResult GetConditionOfRoadMP(string DistrictID, string Name, int MPNumberType = 0, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                Session["_RoadMPDistrictID"] = DistrictID;
                Session["_RoadMPName"] = Name;
                Session["_RoadMPNumberType"] = MPNumberType;
                Session["_RoadMPUseState"] = UseState;
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public ActionResult ExportRoadMP()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                var Did = Session["_RoadMPDistrictID"].ToString();
                var Name = Session["_RoadMPName"].ToString();
                var mpnumbertype = (int)Session["_RoadMPNumberType"];
                var UseState = (int)Session["_RoadMPUseState"];
                var ms = RoadMPSearch.ExportRoadMP(Did, Name, mpnumbertype, UseState);
                Session["_RoadMPDistrictID"] = null;
                Session["_RoadMPName"] = null;
                Session["_RoadMPNumberType"] = 0;
                Session["_RoadMPUseState"] = Enums.UseState.Enable;
                string fileName = $"道路门牌_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xls";
                return File(ms, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 农村门牌
        public ContentResult SearchCountryMP(int PageSize, int PageNum, string DistrictID, string Name, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = CountryMPSearch.SearchCountryMP(PageSize, PageNum, DistrictID, Name, UseState);
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
        public ContentResult SearchCountryMPID(string ID)
        {
            RtObj rt = null;
            try
            {
                var r = CountryMPSearch.SearchCountryMPByID(ID);
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
        public JsonResult GetConditionOfCountryMP(string DistrictID, string Name, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                Session["_CountryMPDistrictID"] = DistrictID;
                Session["_CountryMPName"] = Name;
                Session["_CountryMPUseState"] = UseState;
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public ActionResult ExportCountryMP()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                var Did = Session["_CountryMPDistrictID"].ToString();
                var Name = Session["_CountryMPName"].ToString();
                var UseState = (int)Session["_CountryMPUseState"];
                var ms = CountryMPSearch.ExportCountryMP(Did, Name, UseState);
                Session["_CountryMPDistrictID"] = null;
                Session["_CountryMPName"] = null;
                Session["_CountryMPUseState"] = Enums.UseState.Enable;
                string fileName = $"农村门牌_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xls";
                return File(ms, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}