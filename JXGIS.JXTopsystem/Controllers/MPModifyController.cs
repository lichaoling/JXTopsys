using JXGIS.JXTopsystem.Business.MPPrintUtils;
using JXGIS.JXTopsystem.Business.MPModify;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using JXGIS.JXTopsystem.App_Start;

namespace JXGIS.JXTopsystem.Controllers
{
    public class MPModifyController : Controller
    {
        #region 住宅门牌
        [LoggerFilter(Description = "修改住宅门牌")]
        public JsonResult ModifyResidenceMP(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                ResidenceMPModify.ModifyResidenceMP(oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        /// <summary>
        /// 上传数据
        /// </summary>
        /// <returns></returns>
        [LoggerFilter(Description = "验证住宅门牌是否可用")]
        public JsonResult CheckResidenceMPIsAvailable(string ID, string CountyID, string NeighborhoodsID, string CommunityName, string ResidenceName, string MPNumber, string Dormitory, string HSNumber, string LZNumber, string DYNumber)
        {
            RtObj rt = null;
            try
            {
                var b = ResidenceMPModify.CheckResidenceMPIsAvailable(ID, CountyID, NeighborhoodsID, CommunityName, ResidenceName, MPNumber, Dormitory, HSNumber, LZNumber, DYNumber);
                rt = new RtObj(b);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "注销一个住宅门牌")]
        public JsonResult CancelResidenceMP(List<string> ID)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                ResidenceMPModify.CancelResidenceMP(ID);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion

        #region 道路门牌
        [LoggerFilter(Description = "修改道路门牌")]
        public JsonResult ModifyRoadMP(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RoadMPModify.ModifyRoadMP(oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "验证道路门牌是否可用")]
        public JsonResult CheckRoadMPIsAvailable(string ID, string CountyID, string NeighborhoodsID, string CommunityName, string RoadName, string MPNumber)
        {
            RtObj rt = null;
            try
            {
                var b = RoadMPModify.CheckRoadMPIsAvailable(ID, CountyID, NeighborhoodsID, CommunityName, RoadName, MPNumber);
                rt = new RtObj(b);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "注销一个道路门牌")]
        public JsonResult CancelRoadMP(List<string> ID)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RoadMPModify.CancelRoadMP(ID);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion

        #region 农村门牌
        [LoggerFilter(Description = "修改农村门牌")]
        public JsonResult ModifyCountryMP(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                CountryMPModify.ModifyCountryMP(oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "验证农村门牌是否可用")]
        public JsonResult CheckCountryMPIsAvailable(string ID, string CountyID, string NeighborhoodsID, string CommunityName, string ViligeName, string MPNumber, string HSNumber)
        {
            RtObj rt = null;
            try
            {
                var b = CountryMPModify.CheckCountryMPIsAvailable(ID, CountyID, NeighborhoodsID, CommunityName, ViligeName, MPNumber, HSNumber);
                rt = new RtObj(b);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "注销一个农村门牌")]
        public JsonResult CancelCountryMP(List<string> ID)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                CountryMPModify.CancelCountryMP(ID);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion

        #region 地名证明和门牌证打印
        [LoggerFilter(Description = "门牌证或地名证明")]
        public JsonResult MPCertificate(List<string> IDs, string MPType, string CertificateType)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPPrintUtils.MPCertificate(IDs, MPType, CertificateType);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "地址证明打印")]
        public ActionResult DZZMPrint(List<string> IDs, string MPType)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                //List<string> IDs = ID.Split(',').ToList();
                var ms = MPPrintUtils.DZZMPrint(IDs, MPType);
                string fileName = $"地址证明_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.pdf";
                return File(ms, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }

        }
        [LoggerFilter(Description = "门牌证打印")]
        public ActionResult MPZPrint(List<string> IDs, string MPType)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                //List<string> IDs = ID.Split(',').ToList();
                var ms = MPPrintUtils.MPZPrint(IDs, MPType);
                string fileName = $"门牌证_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.pdf";
                return File(ms, "application/pdf", fileName);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion

        //测试用
        public ActionResult test1()
        {
            RtObj rt = null;
            try
            {
                rt = new Models.Extends.RtObj.RtObj();
                //var FCZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple("FCZFiles");
                //var TDZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple("TDZFiles");
                //var BDCZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple("BDCZFiles");
                //var HJFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple("HJFiles");

                //var guid = Guid.NewGuid().ToString();

                //var ResidenceMPFile_FCZ = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "ResidenceMP", guid, "FCZ");
                //var ResidenceMPFile_TDZ = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "ResidenceMP", guid, "FCZ");
                //var ResidenceMPFile_BDCZ = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "ResidenceMP", guid, "FCZ");
                //var ResidenceMPFile_HJ = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "ResidenceMP", guid, "FCZ");

                //var FCZList = MPModifyUtils.SaveFiles(FCZFiles, ResidenceMPFile_FCZ);
                //var TDZList = MPModifyUtils.SaveFiles(TDZFiles, ResidenceMPFile_TDZ);
                //var BDCZList = MPModifyUtils.SaveFiles(BDCZFiles, ResidenceMPFile_BDCZ);
                //var HJList = MPModifyUtils.SaveFiles(HJFiles, ResidenceMPFile_HJ);

                //var FCZFileNames = string.Join(",", FCZList);//文件名称
                //var TDZFileNames = string.Join(",", TDZList);//文件名称
                //var BDCZFileNames = string.Join(",", BDCZList);//文件名称
                //var HJFileNames = string.Join(",", HJList);//文件名称
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

    }
}