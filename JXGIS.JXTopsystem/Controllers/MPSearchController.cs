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

        #region 住宅门牌
        public ContentResult SearchResidenceMP(int PageSize, int PageNum, string DistrictID, string ResidenceName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = ResidenceMPSearch.SearchResidenceMP(PageSize, PageNum, DistrictID, ResidenceName, AddressCoding, PropertyOwner, StandardAddress, UseState);
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
        public JsonResult GetConditionOfResidenceMP(string DistrictID, string ResidenceName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                Session["_ResidenceMPDistrictID"] = DistrictID;
                Session["_ResidenceMPName"] = ResidenceName;
                Session["_ResidenceMPAddressCoding"] = AddressCoding;
                Session["_ResidenceMPPropertyOwner"] = PropertyOwner;
                Session["_ResidenceMPStandardAddress"] = StandardAddress;
                Session["_ResidenceMPUseState"] = UseState;
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
                var ResidenceName = Session["_ResidenceMPName"].ToString();
                var AddressCoding = Session["_ResidenceMPAddressCoding"].ToString();
                var PropertyOwner = Session["_ResidenceMPPropertyOwner"].ToString();
                var StandardAddress = Session["_ResidenceMPStandardAddress"].ToString();
                var UseState = (int)Session["_ResidenceMPUseState"];

                var ms = ResidenceMPSearch.ExportResidenceMP(Did, ResidenceName, AddressCoding, PropertyOwner, StandardAddress, UseState);
                Session["_ResidenceMPDistrictID"] = null;
                Session["_ResidenceMPName"] = null;
                Session["_ResidenceMPAddressCoding"] = null;
                Session["_ResidenceMPPropertyOwner"] = null;
                Session["_ResidenceMPStandardAddress"] = null;
                Session["_ResidenceMPUseState"] = null;
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
        public ContentResult SearchRoadMP(int PageSize, int PageNum, string DistrictID, string RoadName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState = Enums.UseState.Enable, int MPNumberType = 0)
        {
            RtObj rt = null;
            try
            {
                var r = RoadMPSearch.SearchRoadMP(PageSize, PageNum, DistrictID, RoadName, MPNumberType, AddressCoding, PropertyOwner, StandardAddress, UseState);
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
        public JsonResult GetConditionOfRoadMP(string DistrictID, string RoadName, string AddressCoding, string PropertyOwner, string StandardAddress, int MPNumberType = 0, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                Session["_RoadMPDistrictID"] = DistrictID;
                Session["_RoadMPRoadName"] = RoadName;
                Session["_RoadMPAddressCoding"] = AddressCoding;
                Session["_RoadMPPropertyOwner"] = PropertyOwner;
                Session["_RoadMPStandardAddress"] = StandardAddress;
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
                var DistrictID = Session["_RoadMPDistrictID"].ToString();
                var RoadName = Session["_RoadMPRoadName"].ToString();
                var AddressCoding = Session["_RoadMPAddressCoding"].ToString();
                var PropertyOwner = Session["_RoadMPPropertyOwner"].ToString();
                var StandardAddress = Session["_RoadMPStandardAddress"].ToString();
                var MPNumberType = (int)Session["_RoadMPNumberType"];
                var UseState = (int)Session["_RoadMPUseState"];
                var ms = RoadMPSearch.ExportRoadMP(DistrictID, RoadName, MPNumberType, AddressCoding, PropertyOwner, StandardAddress, UseState);
                Session["_RoadMPDistrictID"] = null;
                Session["_RoadMPRoadName"] = null;
                Session["_RoadMPAddressCoding"] = null;
                Session["_RoadMPPropertyOwner"] = null;
                Session["_RoadMPStandardAddress"] = null;
                Session["_RoadMPNumberType"] = null;
                Session["_RoadMPUseState"] = null;
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
        public ContentResult SearchCountryMP(int PageSize, int PageNum, string DistrictID, string ViligeName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = CountryMPSearch.SearchCountryMP(PageSize, PageNum, DistrictID, ViligeName, AddressCoding, PropertyOwner, StandardAddress, UseState);
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
        public JsonResult GetConditionOfCountryMP(string DistrictID, string ViligeName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                Session["_CountryMPDistrictID"] = DistrictID;
                Session["_CountryMPViligeName"] = ViligeName;
                Session["_CountryMPAddressCoding"] = AddressCoding;
                Session["_CountryMPPropertyOwner"] = PropertyOwner;
                Session["_CountryMPStandardAddress"] = StandardAddress;
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
                var DistrictID = Session["_CountryMPDistrictID"].ToString();
                var ViligeName = Session["_CountryMPName"].ToString();
                var AddressCoding = Session["_CountryMPAddressCoding"].ToString();
                var PropertyOwner = Session["_CountryMPPropertyOwner"].ToString();
                var StandardAddress = Session["_CountryMPStandardAddress"].ToString();
                var UseState = (int)Session["_CountryMPUseState"];
                var ms = CountryMPSearch.ExportCountryMP(DistrictID, ViligeName, AddressCoding, PropertyOwner, StandardAddress, UseState);
                Session["_CountryMPDistrictID"] = null;
                Session["_CountryMPViligeName"] = null;
                Session["_CountryMPAddressCoding"] = null;
                Session["_CountryMPPropertyOwner"] = null;
                Session["_CountryMPStandardAddress"] = null;
                Session["_CountryMPUseState"] = null;
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