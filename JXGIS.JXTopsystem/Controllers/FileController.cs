using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class FileController : Controller
    {
        private static readonly string uploadBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

        // 住宅门牌上传相对路径
        private static readonly string residenceMPRelativePath = Path.Combine("MP", Enums.MPTypeStr.ResidenceMP);
        // 道路门牌上传相对路径
        private static readonly string roadMPRelativePath = Path.Combine("MP", Enums.MPTypeStr.RoadMP);
        // 农村门牌上传相对路径
        private static readonly string countryMPRelativePath = Path.Combine("MP", Enums.MPTypeStr.CountryMP);

        public class Paths
        {
            // 相对路径
            public string RelativePath { get; set; }
            // 绝对路径
            public string FullPath { get; set; }
            // 缩略图相对路径
            public string TRelativePath { get; set; }
            // 缩略图绝对路径
            public string TFullPath { get; set; }
        }

        private Paths GetUploadFilePath(string MPType, string MPid, string fileEx)
        {
            string relativePath = string.Empty;
            switch (MPType.ToUpper())
            {
                case "RESIDENCE":
                    relativePath = residenceMPRelativePath;
                    break;
                case "ROAD":
                    relativePath = roadMPRelativePath;
                    break;
                case "COUNTRY":
                    relativePath = countryMPRelativePath;
                    break;
                default:
                    throw new Exception("未知的文件目录");
            }

            string savePath = Path.Combine(uploadBasePath, relativePath, MPid);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string fileID = System.Guid.NewGuid().ToString();
            string rPath = Path.Combine(relativePath, MPid, fileID + fileEx);
            string fPath = Path.Combine(savePath, fileID + fileEx);
            string trPath = Path.Combine(relativePath, MPid, "t-" + fileID + fileEx);
            string tfPath = Path.Combine(savePath, "t-" + fileID + fileEx);

            return new Paths()
            {
                FullPath = fPath,
                RelativePath = rPath,
                TFullPath = tfPath,
                TRelativePath = trPath
            };
        }

        public ActionResult UploadPicture(string MPid, string MPType, string DocType)
        {
            RtObj rt = null;
            try
            {
                HttpPostedFileBase file = null;
                if (this.Request.Files != null && this.Request.Files.Count != 0)
                {
                    // 获取并保存图片
                    file = this.Request.Files[0];
                    var fileName = file.FileName;
                    var fileID = System.Guid.NewGuid().ToString();
                    var fileEx = new FileInfo(fileName).Extension;
                    var paths = GetUploadFilePath(MPType, MPid, fileEx);
                    // 保存图片
                    file.SaveAs(paths.FullPath);
                    // 保存缩略图片
                    Image image = Image.FromStream(file.InputStream);
                    image = GetHvtThumbnail(image, 200);
                    image.Save(paths.TFullPath);

                    // 保存到数据库中
                    // 文件ID，门牌记录的ID，图片相对路径，缩略图相对路径，文件名称等
                    using (var dbContext = SystemUtils.NewEFDbContext)
                    {
                        MPOfUploadFiles data = new Models.Entities.MPOfUploadFiles();
                        data.ID = fileID;
                        data.Name = fileName;
                        data.DocType = DocType;
                        data.MPID = MPid;
                        data.FileType = fileEx;
                        data.State = Enums.UseState.Enable;
                        dbContext.MPOfUploadFiles.Add(data);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public Image GetHvtThumbnail(Image image, int max)
        {
            int oh = image.Height;
            int ow = image.Width;

            int h = 0;
            int w = 0;

            if (oh > ow)
            {
                h = max;
                w = (int)((double)ow / oh * h);
            }
            else
            {
                w = max;
                h = (int)((double)oh / ow * w);
            }

            Bitmap m_hovertreeBmp = new Bitmap(w, h);
            Graphics m_HvtGr = Graphics.FromImage(m_hovertreeBmp);
            m_HvtGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            m_HvtGr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            m_HvtGr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Rectangle rectDestination = new Rectangle(0, 0, w, h);

            m_HvtGr.DrawImage(image, rectDestination, 0, 0, ow, oh, GraphicsUnit.Pixel);
            return m_hovertreeBmp;
        }

        public ActionResult RemovePicture(string ID)
        {
            RtObj rt = null;
            try
            {
                using (var dbContext = SystemUtils.NewEFDbContext)
                {
                    var query = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                    if (query == null)
                        throw new Exception("该图片已经被删除！");
                    query.State = Enums.UseState.Delete;
                    dbContext.SaveChanges();
                }

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