using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Business.MPModify;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
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
    public class CommonController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region 行政区划相关
        [LoggerFilter(Description = "获取行政区划表中的行政区划树")]
        public ContentResult GetDistrictTree()
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
        [LoggerFilter(Description = "根据现有权限获取行政区划表中的行政区划树")]
        public ContentResult getDistrictTreeFromDistrict()
        {
            RtObj rt = null;
            try
            {
                var tree = DistrictUtils.getDistrictTreeFromDistrict(LoginUtils.CurrentUser.DistrictIDList);
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
        [LoggerFilter(Description = "根据现有权限获取三类门牌表中的行政区划树")]
        public ContentResult getDistrictTreeFromData(int type = Enums.TypeInt.MP)
        {
            RtObj rt = null;
            try
            {
                var tree = DistrictUtils.getDistrictTreeFromData(type, LoginUtils.CurrentUser.DistrictIDList);
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
        [LoggerFilter(Description = "修改行政区划")]
        public JsonResult ModifyDist(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.ModifyDist(oldDataJson);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "删除行政区划")]
        public JsonResult DeleteDist(DistrictDetails dis)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.DeleteDist(dis);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "获取行政区划表")]
        public JsonResult SearchDist()
        {
            RtObj rt = null;
            try
            {
                var r = DistrictUtils.SearchDist();
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "查看一个行政区划详情")]
        public JsonResult SearchDistByID(string id)
        {
            RtObj rt = null;
            try
            {
                var r = DistrictUtils.SearchDistByID(id);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "获取所有区县")]
        public JsonResult GetCountys()
        {
            RtObj rt = null;
            try
            {
                var r = DistrictUtils.GetCountys();
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion 行政区划相关

        #region 地名标志
        [LoggerFilter(Description = "获取分类后的地名标志")]
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
        [LoggerFilter(Description = "获取地名标志")]
        public ContentResult GetDMBZFromDic()
        {
            RtObj rt = null;
            try
            {
                var data = DicUtils.GetDMBZFromDic();
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "查看地名标志详情")]
        public ContentResult GetDMBZFromDicByID(int id)
        {
            RtObj rt = null;
            try
            {
                var data = DicUtils.GetDMBZFromDicByID(id);
                rt = new RtObj(data);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        
        [LoggerFilter(Description = "获取地名标志字典表中的门牌类型")]
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
        [LoggerFilter(Description = "根据门牌类型获取地名标志字典表中的门牌大小")]
        public ContentResult GetMPSizeByMPType(int? mpType)
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
        [LoggerFilter(Description = "修改地名标志")]
        public ContentResult ModifyDMBZ(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                DicUtils.ModifyDMBZ(oldDataJson);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "删除地名标志")]
        public ContentResult DeleteDMBZ(DMBZDic dmbz)
        {
            RtObj rt = null;
            try
            {
                DicUtils.DeleteDMBZ(dmbz);
                rt = new RtObj();
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
        [LoggerFilter(Description = "从邮政编码字典表中获取行政区划树")]
        public ContentResult getDistrictTreeFromPostcodeData()
        {
            RtObj rt = null;
            try
            {
                var d = DicUtils.getDistrictTreeFromPostcodeData();
                rt = new RtObj(d);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "根据乡镇ID从邮政编码字典表中获取社区名")]
        public ContentResult getCommunityNames(string NeighborhoodsID)
        {
            RtObj rt = null;
            try
            {
                var d = DicUtils.getCommunityNames(NeighborhoodsID);
                rt = new RtObj(d);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "根据行政区划获取邮政编码")]
        public ContentResult GetPostcodeByDID(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            RtObj rt = null;
            try
            {
                var sizes = DicUtils.GetPostcodeByDID(CountyID, NeighborhoodsID, CommunityName);
                rt = new RtObj(sizes);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "修改邮政编码")]
        public ContentResult ModifyPostcode(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                DicUtils.ModifyPostcode(oldDataJson);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "删除邮政编码")]
        public ContentResult DeletePostcode(PostcodeDic post)
        {
            RtObj rt = null;
            try
            {
                DicUtils.DeletePostcode(post);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "根据行政区划获取邮政编码表")]
        public ContentResult GetPostcodes(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            RtObj rt = null;
            try
            {
                var result = DicUtils.GetPostcodes(CountyID, NeighborhoodsID, CommunityName);
                rt = new RtObj(result);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "查看邮政编码详情")]
        public ContentResult GetPostcodeByID(int id)
        {
            RtObj rt = null;
            try
            {
                var result = DicUtils.GetPostcodeByID(id);
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

        #region 从数据表中和字典表获取小区名、道路名、自然村名、社区名
        /// <summary>
        /// 获取小区名称
        /// </summary>
        /// <param name="CountyID"></param>
        /// <param name="NeighborhoodsID"></param>
        /// <param name="CommunityName"></param>
        /// <returns></returns>
        [LoggerFilter(Description = "根据行政区划从三类门牌表中获取小区名称")]
        public ContentResult getResidenceNamesFromData(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            RtObj rt = null;
            try
            {
                var names = DicUtils.getResidenceNamesFromData(CountyID, NeighborhoodsID, CommunityName);
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
        /// 获取道路名称
        /// </summary>
        /// <param name="type">2道路门牌中的道路，5路牌表中的道路</param>
        /// <param name="CountyID"></param>
        /// <param name="NeighborhoodsID"></param>
        /// <param name="CommunityName"></param>
        /// <returns></returns>
        [LoggerFilter(Description = "根据行政区划从道路门牌或路牌表中获取道路名称")]
        public ContentResult getRoadNamesFromData(int type, string CountyID, string NeighborhoodsID, string CommunityName)
        {
            RtObj rt = null;
            try
            {
                var names = DicUtils.getRoadNamesFromData(CountyID, NeighborhoodsID, CommunityName, type);
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
        /// 获取自然村名称
        /// </summary>
        /// <param name="type"></param>
        /// <param name="CountyID"></param>
        /// <param name="NeighborhoodsID"></param>
        /// <param name="CommunityName"></param>
        /// <returns></returns>
        [LoggerFilter(Description = "根据行政区划从农村门牌表中获取自然村名称")]
        public ContentResult getViligeNamesFromData(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            RtObj rt = null;
            try
            {
                var names = DicUtils.getViligeNamesFromData(CountyID, NeighborhoodsID, CommunityName);
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
        /// 获取社区名称
        /// </summary>
        /// <param name="type">1住宅门牌表 2道路门牌表  3农村门牌表 5路牌表</param>
        /// <param name="CountyID"></param>
        /// <param name="NeighborhoodsID"></param>
        /// <param name="CommunityName"></param>
        /// <returns></returns>
        [LoggerFilter(Description = "根据行政区划从三类门牌或路牌表中获取社区名称")]
        public ContentResult getCommunityNamesFromData(int type, string NeighborhoodsID)
        {
            RtObj rt = null;
            try
            {
                var names = DicUtils.getCommunityNamesFromData(type, NeighborhoodsID);
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
        [LoggerFilter(Description = "根据行政区划从现有数数据表中获取小区、道路、社区、自然村等名称")]
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
        [LoggerFilter(Description = "根据行政区划从道路字典表中获取道路信息")]
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
        #endregion 从数据表中和字典表获取小区名、道路名、自然村名、社区名

        #region 添加道路信息、小区名、自然村信息到字典表
        [LoggerFilter(Description = "添加新的道路信息到道路字典表")]
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
        [LoggerFilter(Description = "添加新的小区到小区字典表")]
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
        [LoggerFilter(Description = "根据新的自然村到自然村字典表")]
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
        #endregion 添加道路信息、小区名、自然村信息到字典表

        #region 路牌相关
        [LoggerFilter(Description = "从路牌标志表中获取路牌方向")]
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
        [LoggerFilter(Description = "从路牌标志表中获取维修内容")]
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
        [LoggerFilter(Description = "根据类型从路牌标志表中获取路牌标志信息")]
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
        [LoggerFilter(Description = "从路牌标志表中获取所有路牌标志信息")]
        public ContentResult GetRPBZFromDic()
        {
            RtObj rt = null;
            try
            {
                var datas = DicUtils.GetRPBZFromDic();
                rt = new RtObj(datas);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "查看路牌标志详情")]
        public ContentResult GetRPBZFromDicByID(int id) {
            RtObj rt = null;
            try
            {
                var datas = DicUtils.GetRPBZFromDicByID(id);
                rt = new RtObj(datas);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "从路牌标志表中获取所有路牌标志中的类型")]
        public ContentResult GetRPCategory()
        {
            RtObj rt = null;
            try
            {
                var datas = DicUtils.GetRPCategory();
                rt = new RtObj(datas);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "从路牌表中获取所有路牌标志的信息")]
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
        [LoggerFilter(Description = "修改路牌标志信息")]
        public JsonResult ModifyRPBZ(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                DicUtils.ModifyRPBZ(oldDataJson);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "删除路牌标志信息")]
        public JsonResult DeleteRPBZ(RPBZDic rpbz)
        {
            RtObj rt = null;
            try
            {
                DicUtils.DeleteRPBZ(rpbz);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

        #endregion 路牌相关

        [LoggerFilter(Description = "根据现有权限获取所有窗口类型")]
        public ContentResult GetUserWindows()
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.GetWindows(LoginUtils.CurrentUser.DistrictIDList);
                rt = new RtObj(data);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "根据窗口获取权限内可查看的所有用户")]
        public ContentResult GetCreateUsers(string window)
        {
            RtObj rt = null;
            try
            {
                var createUsers = DistrictUtils.GetCreateUsers(LoginUtils.CurrentUser.DistrictIDList, window);
                rt = new RtObj(createUsers);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        #region 角色管理
        [LoggerFilter(Description = "获取权限表中的行政区划")]
        public ContentResult GetDistrictTreeFromRole()
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.GetDistrictTreeFromRole();
                rt = new RtObj(data);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "修改权限表")]
        public ContentResult ModifyRole(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.ModifyRole(oldDataJson);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "删除权限")]
        public ContentResult DeleteRole(SysRole role)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.DeleteRole(role);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "查询所有角色")]
        public ContentResult SearchRole()
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.SearchRole();
                rt = new RtObj(data);

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
        [LoggerFilter(Description = "查看某个角色详情")]
        public ContentResult SearchRoleByID(string RoleID)
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.SearchRoleByID(RoleID);
                rt = new RtObj(data);

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
        [LoggerFilter(Description = "查询所有权限")]
        public ContentResult SearchPrivilige()
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.SearchPrivilige();
                rt = new RtObj(data);

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
        //[LoggerFilter(Description = "获取权限表中的所有窗口类型")]
        //public ContentResult GetWindows()
        //{
        //    RtObj rt = null;
        //    try
        //    {
        //        var data = DistrictUtils.GetWindows();
        //        rt = new RtObj(data);

        //    }
        //    catch (Exception ex)
        //    {
        //        rt = new RtObj(ex);
        //    }
        //    var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
        //    return Content(s);
        //}
        //[LoggerFilter(Description = "获取权限表中的所有角色名称")]
        //public ContentResult GetRoleNames()
        //{
        //    RtObj rt = null;
        //    try
        //    {
        //        var data = DistrictUtils.GetRoleNames();
        //        rt = new RtObj(data);

        //    }
        //    catch (Exception ex)
        //    {
        //        rt = new RtObj(ex);
        //    }
        //    var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
        //    return Content(s);
        //}
        #endregion

        [LoggerFilter(Description = "修改用户")]
        public ContentResult ModifyUser(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.ModifyUser(oldDataJson);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "删除用户")]
        public ContentResult DeleteUser(SysUser user)
        {
            RtObj rt = null;
            try
            {
                DistrictUtils.DeleteUser(user);
                rt = new RtObj();

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "获取所有用户列表")]
        public ContentResult SearchUser()
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.SearchUser();
                rt = new RtObj(data);

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
        [LoggerFilter(Description = "查看用户详情")]
        public ContentResult SearchUserByID(string id)
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.SearchUserByID(id);
                rt = new RtObj(data);

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

        [LoggerFilter(Description = "获取所有角色")]
        public ContentResult GetRoleList()
        {
            RtObj rt = null;
            try
            {
                var data = DistrictUtils.GetRoleList();
                rt = new RtObj(data);

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
        [LoggerFilter(Description = "获取新得GUID")]
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