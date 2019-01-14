using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business.PlaceName;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class PlaceNameController : Controller
    {
        #region 专业设施地名备案查询
        public ContentResult GetDMTypesFromData(string DistrictID, string ZYSSType)
        {
            RtObj rt = null;
            try
            {
                var r = PlaceNameSearch.GetDMTypesFromData(DistrictID, ZYSSType);
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

        [LoggerFilter(Description = "查询专业设施地名")]
        public ContentResult SearchPlaceName(int PageSize, int PageNum, string ZYSSType, string DistrictID, string CommunityName, string DMType, string Name, string ZGDW, DateTime? start, DateTime? end)
        {
            RtObj rt = null;
            try
            {
                var r = PlaceNameSearch.SearchPlaceName(PageSize, PageNum, ZYSSType, DistrictID, CommunityName, DMType, Name, ZGDW, start, end);
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
        [LoggerFilter(Description = "查询专业设施地名详情")]
        public ContentResult SearchPlaceNameByID(string ID)
        {
            RtObj rt = null;
            try
            {
                var r = PlaceNameSearch.SearchPlaceNameByID(ID);
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
        public JsonResult GetConditionOfPlaceName(string ZYSSType, string DistrictID, string CommunityName, string DMType, string Name, string ZGDW, DateTime? start, DateTime? end)
        {
            RtObj rt = null;
            try
            {
                Session["_PlaceName_ZYSSType"] = ZYSSType;
                Session["_PlaceName_DistrictID"] = DistrictID;
                Session["_PlaceName_CommunityName"] = CommunityName;
                Session["_PlaceName_DMType"] = DMType;
                Session["_PlaceName_Name"] = Name;
                Session["_PlaceName_ZGDW"] = ZGDW;
                Session["_PlaceName_start"] = start;
                Session["_PlaceName_end"] = end;
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "导出专业设施地名查询结果")]
        public ActionResult ExportPlaceName()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                var ZYSSType = Session["_PlaceName_ZYSSType"] != null ? Session["_PlaceName_ZYSSType"].ToString() : null;
                var DistrictID = Session["_PlaceName_DistrictID"] != null ? Session["_PlaceName_DistrictID"].ToString() : null;
                var CommunityName = Session["_PlaceName_CommunityName"] != null ? Session["_PlaceName_CommunityName"].ToString() : null;
                var DMType = Session["_PlaceName_DMType"] != null ? Session["_PlaceName_DMType"].ToString() : null;
                var Name = Session["_RoadMPShopName"] != null ? Session["_RoadMPShopName"].ToString() : null;
                var ZGDW = Session["_PlaceName_ZGDW"] != null ? Session["_PlaceName_ZGDW"].ToString() : null;
                var start = Session["_PlaceName_start"] != null ? (DateTime?)Session["_PlaceName_start"] : null;
                var end = Session["_PlaceName_end"] != null ? (DateTime?)Session["_PlaceName_end"] : null;

                var ms = PlaceNameSearch.ExportPlaceName(-1, -1, ZYSSType, DistrictID, CommunityName, DMType, Name, ZGDW, start, end);
                Session["_PlaceName_ZYSSType"] = null;
                Session["_PlaceName_DistrictID"] = null;
                Session["_PlaceName_CommunityName"] = null;
                Session["_PlaceName_DMType"] = null;
                Session["_PlaceName_Name"] = null;
                Session["_PlaceName_ZGDW"] = null;
                Session["_PlaceName_start"] = null;
                Session["_PlaceName_end"] = null;
                string fileName = $"专业设施地名_{ZYSSType}{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
                return File(ms, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CancelPlaceName(List<string> IDs)
        {
            RtObj rt = null;
            try
            {
                PlaceNameSearch.CancelPlaceName(IDs);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        #endregion

        #region 专业设施地名备案
        [LoggerFilter(Description = "修改专业设施地名")]
        public JsonResult ModifyPlaceName(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                PlaceNameSearch.ModifyPlaceName(oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion
    }
}