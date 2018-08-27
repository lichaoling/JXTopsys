using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
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
        public ContentResult getNamesByDistrict(string CountyID, string NeighbourhoodID, string CommunityID, int MPType)
        {
            RtObj rt = null;
            try
            {
                var names = DistrictUtils.getNamesByDistrict(CountyID, NeighbourhoodID, CommunityID, MPType);
                rt = new RtObj(names);

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

        #region 地名标志
        public ContentResult GetMPSizeByMPType(int mpType)
        {
            RtObj rt = null;
            try
            {
                var sizes = DictUtils.GetMPSizeByMPType(mpType);
                rt = new RtObj(sizes);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        #endregion 地名标志

        #region 邮编
        public ContentResult GetPostcodeByDID(string CountyID, string NeighborhoodsID, string CommunityID)
        {
            RtObj rt = null;
            try
            {
                var sizes = DictUtils.GetPostcodeByDID(CountyID, NeighborhoodsID, CommunityID);
                rt = new RtObj(sizes);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult AddPostcode(PostcodeDic postDic)
        {
            RtObj rt = null;
            try
            {
                DictUtils.AddPostcode(postDic);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult ModifyPostcode(PostcodeDic postDic)
        {
            RtObj rt = null;
            try
            {
                DictUtils.ModifyPostcode(postDic);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        #endregion 邮编

        #region 道路字典
        public ContentResult ModifyRoadDic(RoadDic roadDic)
        {
            RtObj rt = null;
            try
            {
                DictUtils.ModifyRoadDic(roadDic);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        #endregion 道路字典


        public ContentResult GetRPBZData(string Category)
        {
            RtObj rt = null;
            try
            {
                var datas = DictUtils.GetRPBZData(Category);
                rt = new RtObj(datas);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult getUserWindows()
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.getWindows(LoginUtils.CurrentUser.DistrictID);
                rt = new RtObj(data);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult getCreateUsers()
        {
            RtObj rt = null;
            try
            {
                var createUsers = DistrictUtils.getCreateUsers(LoginUtils.CurrentUser.DistrictID);
                rt = new RtObj(createUsers);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
    }
}