﻿using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Business.MPModify;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
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

        public ContentResult getDistrictTree()
        {
            RtObj rt = null;
            try
            {
                var tree = DistrictUtils.getDistrictTree();
                rt = new RtObj(tree);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult getDistrictTreeFromDistrict()
        {
            RtObj rt = null;
            try
            {
                var tree = DistrictUtils.getDistrictTreeFromDistrict(LoginUtils.CurrentUser.DistrictID);
                rt = new RtObj(tree);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        /// <summary>
        /// 根据获取类型和当前用户数据权限从当前表中获取行政区划后组织成树
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ContentResult getDistrictTreeFromData(int type)
        {
            RtObj rt = null;
            try
            {
                var tree = DistrictUtils.getDistrictTreeFromData(type, LoginUtils.CurrentUser.DistrictID);
                rt = new RtObj(tree);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult CheckPermission(string CommunityID)
        {
            RtObj rt = null;
            try
            {

                var isVisible = DistrictUtils.CheckPermission(CommunityID);
                rt = new RtObj(isVisible);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public JsonResult AddCounty(string CountyName, string Code)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.AddCounty(CountyName, Code);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult AddNeighborhoods(string CountyName, string NeighborhoodsName, string Code)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.AddNeighborhoods(CountyName, NeighborhoodsName, Code);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult DeleteCounty(string CountyName)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.DeleteCounty(CountyName);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

        public JsonResult DeleteNeighborhoods(string NeighborhoodsName)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.DeleteNeighborhoods(NeighborhoodsName);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

        #region 地名标志
        public ContentResult GetDMBZ()
        {
            RtObj rt = null;
            try
            {
                var data = DicUtils.GetDMBZ();
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetMPType()
        {
            RtObj rt = null;
            try
            {
                var data = DicUtils.GetMPType();
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetMPSizeByMPType(int mpType)
        {
            RtObj rt = null;
            try
            {
                var sizes = DicUtils.GetMPSizeByMPType(mpType);
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
                var sizes = DicUtils.GetPostcodeByDID(CountyID, NeighborhoodsID, CommunityID);
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
                DicUtils.AddPostcode(postDic);
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
                DicUtils.ModifyPostcode(postDic);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetPostcodes(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var result = DicUtils.GetPostcodes(PageSize, PageNum);
                rt = new RtObj(result);
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
        public ContentResult getNamesFromData(int type, string CountyID, string NeighborhoodsID, string CommunityName)
        {
            RtObj rt = null;
            try
            {
                var names = new List<string>();
                if (type == Enums.TypeInt.Residence)
                    names = DicUtils.getResidenceNamesFromData(CountyID, NeighborhoodsID, CommunityName);
                else if (type == Enums.TypeInt.Road || type == Enums.TypeInt.RP)
                    names = DicUtils.getRoadNamesFromData(CountyID, NeighborhoodsID, CommunityName, type);
                else if (type == Enums.TypeInt.Country)
                    names = DicUtils.getViligeNamesFromData(CountyID, NeighborhoodsID, CommunityName);
                else if (type == Enums.TypeInt.Community)
                    names = DicUtils.getCommunityNamesFromData(type, NeighborhoodsID);
                else
                    throw new Exception("获取类型不正确");
                rt = new RtObj(names);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        /// <summary>
        /// 根据获取类型从字典表中获取小区名称、社区名称、道路名称或者自然村名称
        /// </summary>
        /// <param name="CountyID"></param>
        /// <param name="NeighbourhoodID"></param>
        /// <param name="CommunityName"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ContentResult getNamesFromDic(int type, string CountyID, string NeighborhoodsID, string CommunityName)
        {
            RtObj rt = null;
            try
            {
                var names = new List<string>();
                if (type == Enums.TypeInt.Residence)
                    names = DicUtils.getResidenceNamesFromDic(CountyID, NeighborhoodsID, CommunityName);
                else if (type == Enums.TypeInt.Road)
                    names = DicUtils.getRoadNamesFromDic(CountyID, NeighborhoodsID, CommunityName);
                else if (type == Enums.TypeInt.Country)
                    names = DicUtils.getViligeNamesFromDic(CountyID, NeighborhoodsID, CommunityName);
                else if (type == Enums.TypeInt.Community)
                    names = DicUtils.getCommunityNamesFromDic(CountyID, NeighborhoodsID);
                else
                    throw new Exception("获取类型不正确");
                rt = new RtObj(names);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult AddRoadDic(RoadDic roadDic)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                DicUtils.AddRoadDic(roadDic);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult AddResidenceDic(ResidenceDic residenceDic)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                DicUtils.AddResidenceDic(residenceDic);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult AddViligeDic(ViligeDic viligeDic)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                DicUtils.AddViligeDic(viligeDic);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetRoadInfosFromDic(string NeighborhoodsID, string CommunityName, string RoadName)
        {
            RtObj rt = null;
            try
            {
                var data = DicUtils.GetRoadInfosFromDic(NeighborhoodsID, CommunityName, RoadName);
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        #endregion 道路字典

        public ContentResult GetDirectionFromDic()
        {
            RtObj rt = null;
            try
            {
                var data = DicUtils.GetDirectionFromDic();
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetRepairContentFromDic()
        {
            RtObj rt = null;
            try
            {
                var data = DicUtils.GetRepairContentFromDic();
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult GetRPBZDataFromDic(string Category)
        {
            RtObj rt = null;
            try
            {
                var datas = DicUtils.GetRPBZDataFromDic(Category);
                rt = new RtObj(datas);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetRPBZDataFromData()
        {
            RtObj rt = null;
            try
            {
                var datas = DicUtils.GetRPBZDataFromData();
                rt = new RtObj(datas);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public JsonResult AddRPBZData(string Category, string Data)
        {
            RtObj rt = null;
            try
            {
                DicUtils.AddRPBZData(Category, Data);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }


        public ContentResult GetUserWindows()
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

        public ContentResult GetCreateUsers()
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


        /// <summary>
        /// 获取GUID
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGUID()
        {
            RtObj rt = null;
            try
            {
                var guid = BaseUtils.GetGUID();
                rt = new RtObj(guid);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
    }
}