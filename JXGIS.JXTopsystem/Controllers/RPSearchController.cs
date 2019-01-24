using ICSharpCode.SharpZipLib.Zip;
using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.RPSearch;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class RPSearchController : Controller
    {
        [LoggerFilter(Description = "查询路牌")]
        public ContentResult SearchRP(int PageSize, int PageNum, string DistrictID, string CommunityName, string RoadName, string Intersection, string Direction, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, DateTime? start, DateTime? end, int? startCode, int? endCode, int? RepairState, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = RPSearchUtils.SearchRP(PageSize, PageNum, DistrictID, CommunityName, RoadName, Intersection, Direction, Model, Size, Material, Manufacturers, FrontTagline, BackTagline, start, end, startCode, endCode, RepairState, UseState);
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


        public JsonResult GetConditionOfRP(string DistrictID, string CommunityName, string RoadName, string Intersection, string Direction, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, DateTime? start, DateTime? end, int? startCode, int? endCode, int? RepairState, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                Session["_RPDistrictID"] = DistrictID;
                Session["_RPRoadName"] = RoadName;
                Session["_RPIntersection"] = Intersection;
                Session["_RPModel"] = Model;
                Session["_RPSize"] = Size;
                Session["_RPMaterial"] = Material;
                Session["_RPManufacturers"] = Manufacturers;
                Session["_RPFrontTagline"] = FrontTagline;
                Session["_RPBackTagline"] = BackTagline;
                Session["_RPstart"] = start;
                Session["_RPend"] = end;
                Session["_RPstartCode"] = startCode;
                Session["_RPendCode"] = endCode;
                Session["_RPUseState"] = UseState;
                Session["_RPRepairState"] = RepairState;

                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

        [LoggerFilter(Description = "导出路牌")]
        public ActionResult ExportRP()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                var DistrictID = Session["_RPDistrictID"] != null ? Session["_RPDistrictID"].ToString() : null;
                var CommunityName = Session["_RPCommunityName"] != null ? Session["_RPCommunityName"].ToString() : null;
                var RoadName = Session["_RPRoadName"] != null ? Session["_RPRoadName"].ToString() : null;
                var Intersection = Session["_RPIntersection"] != null ? Session["_RPIntersection"].ToString() : null;
                var Direction = Session["_RPDirection"] != null ? Session["_RPDirection"].ToString() : null;
                var Model = Session["_RPModel"] != null ? Session["_RPModel"].ToString() : null;
                var Size = Session["_RPSize"] != null ? Session["_RPSize"].ToString() : null;
                var Material = Session["_RPMaterial"] != null ? Session["_RPMaterial"].ToString() : null;
                var Manufacturers = Session["_RPManufacturers"] != null ? Session["_RPManufacturers"].ToString() : null;
                var FrontTagline = Session["_RPFrontTagline"] != null ? Session["_RPFrontTagline"].ToString() : null;
                var BackTagline = Session["_RPBackTagline"] != null ? Session["_RPBackTagline"].ToString() : null;
                var start = Session["_RPstart"] != null ? (DateTime?)Session["_RPstart"] : null;
                var end = Session["_RPend"] != null ? (DateTime?)Session["_RPend"] : null;
                var startCode = Session["_RPstartCode"] != null ? (int?)Session["_RPstartCode"] : null;
                var endCode = Session["_RPendCode"] != null ? (int?)Session["_RPendCode"] : null;
                var UseState = Session["_RPUseState"] != null ? (int)Session["_RPUseState"] : Enums.UseState.Enable;
                var RepairState = Session["_RPRepairState"] != null ? (int)Session["_RPRepairState"] : 2;

                var ms = RPSearchUtils.ExportRP(DistrictID, CommunityName, RoadName, Intersection, Direction, Model, Size, Material, Manufacturers, FrontTagline, BackTagline, start, end, startCode, endCode, RepairState, UseState);

                Session["_RPDistrictID"] = null;
                Session["_RPCommunityName"] = null;
                Session["_RPRoadName"] = null;
                Session["_RPIntersection"] = null;
                Session["_RPDirection"] = null;
                Session["_RPModel"] = null;
                Session["_RPSize"] = null;
                Session["_RPMaterial"] = null;
                Session["_RPManufacturers"] = null;
                Session["_RPFrontTagline"] = null;
                Session["_RPBackTagline"] = null;
                Session["_RPstart"] = null;
                Session["_RPend"] = null;
                Session["_RPstartCode"] = null;
                Session["_RPendCode"] = null;
                Session["_RPUseState"] = null;
                Session["_RPRepairState"] = null;
                string fileName = $"路牌_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xls";
                return File(ms, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetConditionOfRPIDS(List<string> rpids)
        {
            RtObj rt = null;
            try
            {
                Session["_RPids"] = rpids;

                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
        [LoggerFilter(Description = "根据选中的路牌下载二维码图片")]
        public ContentResult DownloadQRCodeJpgs()
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                var RPids = Session["_RPids"] != null ? (List<string>)Session["_RPids"] : new List<string>();

                string QRFilePath = Path.Combine(StaticVariable.basePath, StaticVariable.RPQRCodeRelativePath);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = null;
                using (ZipFile file = ZipFile.Create(ms))
                {
                    file.BeginUpdate();
                    file.NameTransform = new MyNameTransfom();//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。
                    foreach (var id in RPids)
                    {
                        var rp = SystemUtils.NewEFDbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == id).FirstOrDefault();
                        if (rp != null)
                        {
                            string fileName = rp.Code + ".jpg";
                            string sourceFile = Path.Combine(QRFilePath, fileName);
                            file.Add(sourceFile);
                        }
                    }
                    file.CommitUpdate();
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                }
                Session["_RPids"] = null;

                Response.AddHeader("content-disposition", "attachment;filename=二维码" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip");
                Response.BinaryWrite(buffer);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }



        [LoggerFilter(Description = "根据起止二维码编号下载二维码图片")]
        public ActionResult DownloadQRCodeJpgsByCode(int startCode, int endCode)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                string QRFilePath = Path.Combine(StaticVariable.basePath, StaticVariable.RPQRCodeRelativePath);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = null;
                using (ZipFile file = ZipFile.Create(ms))
                {
                    file.BeginUpdate();
                    file.NameTransform = new MyNameTransfom();//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。
                    for (var i = startCode; i <= endCode; i++)
                    {
                        string fileName = i + ".jpg";
                        string sourceFile = Path.Combine(QRFilePath, fileName);
                        file.Add(sourceFile);
                    }
                    file.CommitUpdate();
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                }
                Response.AddHeader("content-disposition", "attachment;filename=二维码" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip");
                Response.BinaryWrite(buffer);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt, JsonRequestBehavior.AllowGet);
        }


        [LoggerFilter(Description = "编辑一条路牌信息")]
        public ContentResult SearchRPByID(string RPID)
        {
            RtObj rt = null;
            try
            {
                var r = RPSearchUtils.SearchRPByID(RPID);
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
    }
}