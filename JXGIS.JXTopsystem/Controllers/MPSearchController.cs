using JXGIS.JXTopsystem.Business.MPSearch;
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
        public ContentResult SearchResidenceMP(int PageSize, int PageNum, string DistrictID, string Name)
        {
            RtObj rt = null;
            try
            {
                var r = MPSearchUtils.SearchResidenceMP(PageSize, PageNum, DistrictID, Name);
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
                var r = MPSearchUtils.SearchResidenceMPByID(ID);
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
        public JsonResult GetConditionOfResidenceMP(string DistrictID, string Name)
        {
            RtObj rt = null;
            try
            {
                Session["_ResidenceMPDistrictID"] = DistrictID;
                Session["_ResidenceMPName"] = Name;
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
                var ms = MPSearchUtils.ExportResidenceMP(Did, Name);
                Session["_ResidenceMPDistrictID"] = null;
                Session["_ResidenceMPName"] = null;
                //MPSearchController.DistrictID = null;
                //MPSearchController.Name = null;
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
        public ContentResult SearchRoadMP(int PageSize, int PageNum, string DistrictID, string Name, int MPNumberType = 0)
        {
            RtObj rt = null;
            try
            {
                var r = MPSearchUtils.SearchRoadMP(PageSize, PageNum, DistrictID, Name, MPNumberType);
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
                var r = MPSearchUtils.SearchRoadMPByID(ID);
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
        public JsonResult GetConditionOfRoadMP(string DistrictID, string Name, int MPNumberType = 0)
        {
            RtObj rt = null;
            try
            {
                Session["_RoadMPDistrictID"] = DistrictID;
                Session["_RoadMPName"] = Name;
                Session["_RoadMPNumberType"] = MPNumberType;

                //MPSearchController.DistrictID = DistrictID;
                //MPSearchController.Name = Name;
                //MPSearchController.MPNumberType = MPNumberType;
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
                var ms = MPSearchUtils.ExportRoadMP(Did, Name, mpnumbertype);
                Session["_RoadMPDistrictID"] = null;
                Session["_RoadMPName"] = null;
                Session["_RoadMPNumberType"] = 0;
                //MPSearchController.DistrictID = null;
                //MPSearchController.Name = null;
                //MPSearchController.MPNumberType = 0;
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
        public ContentResult SearchCountryMP(int PageSize, int PageNum, string DistrictID, string Name)
        {
            RtObj rt = null;
            try
            {
                var r = MPSearchUtils.SearchCountryMP(PageSize, PageNum, DistrictID, Name);
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
                var r = MPSearchUtils.SearchCountryMPByID(ID);
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
        public JsonResult GetConditionOfCountryMP(string DistrictID, string Name)
        {
            RtObj rt = null;
            try
            {
                Session["_CountryMPDistrictID"] = DistrictID;
                Session["_CountryMPName"] = Name;
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
        public ActionResult ExportCountryMP()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                var Did = Session["_CountryMPDistrictID"].ToString();
                var Name = Session["_CountryMPName"].ToString();
                var ms = MPSearchUtils.ExportCountryMP(Did, Name);
                Session["_CountryMPDistrictID"] = null;
                Session["_CountryMPName"] = null;
                //MPSearchController.DistrictID = null;
                //MPSearchController.Name = null;
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