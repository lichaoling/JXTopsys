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

namespace JXGIS.JXTopsystem.Controllers
{
    public class MPModifyController : Controller
    {
        #region 住宅门牌
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
        public JsonResult ModifyRoadMP(MPOfRoad newData, string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RoadMPModify.ModifyRoadMP(newData, oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult CheckRoadMPIsAvailable(string CountyID, string NeighborhoodsID, string CommunityName, string RoadName, string MPNumber)
        {
            RtObj rt = null;
            try
            {
                var b = RoadMPModify.CheckRoadMPIsAvailable(CountyID, NeighborhoodsID, CommunityName, RoadName, MPNumber);
                rt = new RtObj(b);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
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
        public JsonResult ModifyCountryMP(MPOfCountry newData, string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                CountryMPModify.ModifyCountryMP(newData, oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult CheckCountryMPIsAvailable(string CountyID, string NeighborhoodsID, string CommunityName, string ViligeName, string MPNumber, string HSNumber)
        {
            RtObj rt = null;
            try
            {
                var b = CountryMPModify.CheckCountryMPIsAvailable(CountyID, NeighborhoodsID, CommunityName, ViligeName, MPNumber, HSNumber);
                rt = new RtObj(b);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult CancelCountryMP(List<string> ID)
        {
            RtObj rt = null;
            try
            {
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
        public JsonResult MPCertificatePrint(List<string> IDs, int MPType, int CertificateType)
        {
            RtObj rt = null;
            try
            {
                MPPrintUtils.MPCertificatePrint(IDs, MPType, CertificateType);
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