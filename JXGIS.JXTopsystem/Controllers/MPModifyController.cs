using JXGIS.JXTopsystem.Business.MPCertificate;
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
        public JsonResult ModifyResidenceMP(MPOfResidence newData, string oldDataJson, List<string> FCZIDs, List<string> TDZIDs, List<string> BDCZIDs, List<string> HJIDs)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.ModifyResidenceMP(newData, oldDataJson, FCZIDs, TDZIDs, BDCZIDs, HJIDs);
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
        public JsonResult UploadResidenceMP()
        {
            RtObj rt = null;
            try
            {
                var file = this.Request.Files;
                if (file == null || file.Count == 0)
                    throw new Exception("文件不存在，请重新上传！");
                MPModifyUtils.UploadResidenceMP(file[0]);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public ContentResult GetUploadResidenceMP(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var r = MPModifyUtils.GetUploadResidenceMP(PageSize, PageNum);
                rt = new RtObj(r);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        /// <summary>
        /// 将没有错的上传的数据更新到数据库中,返回门牌制作汇总表
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateResidenceMP()
        {
            RtObj rt = null;
            try
            {
                var r = MPModifyUtils.UpdateResidenceMP();
                rt = new RtObj(r);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult CancelOrDelResidenceMP(string ID, int UseState)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.CancelOrDelResidenceMP(ID, UseState);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion

        #region 道路门牌
        public JsonResult ModifyRoadMP(MPOfRoad newData, string oldDataJson, List<string> FCZIDs, List<string> TDZIDs, List<string> YYZZIDs)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.ModifyRoadMP(newData, oldDataJson, FCZIDs, TDZIDs, YYZZIDs);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult UploadRoadMP()
        {
            RtObj rt = null;
            try
            {
                var file = this.Request.Files;
                if (file == null || file.Count == 0)
                    throw new Exception("文件不存在，请重新上传！");
                MPModifyUtils.UploadRoadMP(file[0]);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public ContentResult GetUploadRoadMP(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var r = MPModifyUtils.GetUploadRoadMP(PageSize, PageNum);
                rt = new RtObj(r);

            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public JsonResult UpdateRoadMP()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.UpdateRoadMP();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult CancelOrDelRoadMP(string ID, int UseState)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.CancelOrDelRoadMP(ID, UseState);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        #endregion

        #region 农村门牌
        public JsonResult ModifyCountryMP(MPOfCountry newData, string oldDataJson, List<string> TDZIDs, List<string> QQZIDs)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.ModifyCountryMP(newData, oldDataJson, TDZIDs, QQZIDs);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult UploadCountryMP()
        {
            RtObj rt = null;
            try
            {
                var file = this.Request.Files;
                if (file == null || file.Count == 0)
                    throw new Exception("文件不存在，请重新上传！");
                MPModifyUtils.UploadCountryMP(file[0]);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public ContentResult GetUploadCountryMP(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var r = MPModifyUtils.GetUploadCountryMP(PageSize, PageNum);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public JsonResult UpdateCountryMP()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.UpdateCountryMP();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult CancelOrDelCountryMP(string ID, int UseState)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPModifyUtils.CancelOrDelCountryMP(ID, UseState);
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

        #region 地名证明和门牌证浏览打印
        public JsonResult MPCertificateQuery(string ID, int MPType, int CertificateType)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPCertificateUtils.MPCertificateQuery(ID, MPType, CertificateType);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        public JsonResult MPCertificatePrint(string ID, int MPType, int CertificateType)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                MPCertificateUtils.MPCertificatePrint(ID, MPType, CertificateType);
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