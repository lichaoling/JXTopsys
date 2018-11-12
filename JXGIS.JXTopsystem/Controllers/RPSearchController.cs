using ICSharpCode.SharpZipLib.Zip;
using JXGIS.JXTopsystem.App_Start;
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
        public ContentResult SearchRP(int PageSize, int PageNum, string DistrictID, string RoadName, string Intersection, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, DateTime? start, DateTime? end, int? startCode, int? endCode, int UseState = Enums.UseState.Enable)
        {
            RtObj rt = null;
            try
            {
                var r = RPSearchUtils.SearchRP(PageSize, PageNum, DistrictID, RoadName, Intersection, Model, Size, Material, Manufacturers, FrontTagline, BackTagline, start, end, startCode, endCode, UseState);
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

        [LoggerFilter(Description = "二维码图片下载")]
        public ContentResult DownloadQRCodeJpgs(List<RPDetails> rps)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                string QRFilePath = Path.Combine(FileController.uploadBasePath, FileController.RPQRCodeRelativePath);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = null;
                using (ZipFile file = ZipFile.Create(ms))
                {
                    file.BeginUpdate();
                    file.NameTransform = new MyNameTransfom();//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。
                    foreach (var rp in rps)
                    {
                        string fileName = rp.Code + ".jpg";
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
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
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