using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class CommonController : Controller
    {
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }

        public ContentResult GetDistrictsTree(List<string> districtIDs)
        {
            RtObj rt = null;
            try
            {
                var tree = DistrictUtils.getDistrictTree(districtIDs);
                rt = new RtObj(tree);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetUserDistrictsTree()
        {
            RtObj rt = null;
            try
            {
                var tree = DistrictUtils.getDistrictTree(LoginUtils.CurrentUser.DistrictID);
                rt = new RtObj(tree);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        //public ContentResult GetRoads(int PageSize, int PageNum, string Name = null)
        //{
        //    RtObj rt = null;
        //    try
        //    {
        //        var data = RoadUtils.GetRoads(PageSize, PageNum, Name);
        //        rt = new RtObj(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        rt = new RtObj(ex);
        //    }
        //    var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
        //    return Content(s);
        //}

        //public ContentResult GetRoadByID(string roadID)
        //{
        //    RtObj rt = null;
        //    try
        //    {
        //        var data = RoadUtils.GetRoadByID(roadID);
        //        rt = new RtObj(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        rt = new RtObj(ex);
        //    }
        //    var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
        //    return Content(s);
        //}
        public ContentResult GetRoadsByDistrict(string CountyID, string NeighborhoodsID, string CommunityID)
        {
            RtObj rt = null;
            try
            {
                var data = RoadUtils.GetRoadsByDistrict(CountyID, NeighborhoodsID, CommunityID);
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public JsonResult CheckPermission(string CommunityID)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                DistrictUtils.CheckPermission(CommunityID);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
    }
}