using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.Common;
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
        public static readonly string uploadBasePath = AppDomain.CurrentDomain.BaseDirectory;

        // 住宅门牌上传相对路径
        public static readonly string residenceMPRelativePath = Path.Combine("Files", Enums.TypeStr.MP, Enums.MPFileType.ResidenceMP);
        // 道路门牌上传相对路径
        public static readonly string roadMPRelativePath = Path.Combine("Files", Enums.TypeStr.MP, Enums.MPFileType.RoadMP);
        // 农村门牌上传相对路径
        public static readonly string countryMPRelativePath = Path.Combine("Files", Enums.TypeStr.MP, Enums.MPFileType.CountryMP);

        //路牌标志照片上传相对路径
        public static readonly string RPBZPhotoRelativePath = Path.Combine("Files", Enums.TypeStr.RP, Enums.RPFileType.BZPhoto);
        //路牌二维码照片上传相对路径
        public static readonly string RPQRCodeRelativePath = Path.Combine("Files", Enums.TypeStr.RP, Enums.RPFileType.QRCode);
        //路牌维修前后照片上传相对路径
        public static readonly string RPRepairPhotoRelativePath = Path.Combine("Files", Enums.TypeStr.RP, Enums.RPFileType.RepairPhoto);

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

            public string FileID { get; set; }
            public string Name { get; set; }
        }

        public static Paths GetUploadFilePath(string FileType, string ID, string fileID, string fileName, string RepairType)
        {
            string relativePath = string.Empty;
            var fileEx = new FileInfo(fileName).Extension;

            switch (FileType.ToUpper())
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
                case "RPBZPHOTO":
                    relativePath = RPBZPhotoRelativePath;
                    break;
                case "RPREPAIRPHOTO":
                    relativePath = RPRepairPhotoRelativePath;
                    break;
                default:
                    throw new Exception("未知的文件目录");
            }
            relativePath = Path.Combine(relativePath, ID);
            string savePath = Path.Combine(uploadBasePath, relativePath);

            if (FileType.ToUpper() == "RPREPAIRPHOTO")
            {
                savePath = Path.Combine(savePath, RepairType);
                relativePath = Path.Combine(relativePath, RepairType);
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string rPath = Path.Combine(relativePath, fileID + fileEx);
            string fPath = Path.Combine(savePath, fileID + fileEx);
            string trPath = Path.Combine(relativePath, "t-" + fileID + fileEx);
            string tfPath = Path.Combine(savePath, "t-" + fileID + fileEx);

            return new Paths()
            {
                FullPath = fPath,
                RelativePath = rPath,
                TFullPath = tfPath,
                TRelativePath = trPath,
                FileID = fileID,
                Name = fileName,
            };
        }

        public ActionResult UploadPicture(string ID, string FileType, string DocType, string RepairType = null)
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
                    var paths = GetUploadFilePath(FileType, ID, fileID, fileName, RepairType);
                    // 保存图片
                    file.SaveAs(paths.FullPath);
                    // 保存缩略图片
                    Image image = Image.FromStream(file.InputStream);
                    image = PictureUtils.GetHvtThumbnail(image, 200);
                    image.Save(paths.TFullPath);

                    // 保存到数据库中
                    // 文件ID，门牌记录的ID，图片相对路径，缩略图相对路径，文件名称等
                    using (var dbContext = SystemUtils.NewEFDbContext)
                    {
                        var MPTypes = new List<string>() { "RESIDENCE", "ROAD", "COUNTRY" };
                        if (MPTypes.Contains(FileType.ToUpper()))
                        {
                            MPOfUploadFiles data = new Models.Entities.MPOfUploadFiles();
                            data.ID = fileID;
                            data.Name = fileName;
                            data.DocType = DocType;
                            data.MPID = ID;
                            data.FileEx = fileEx;
                            data.State = Enums.UseState.Enable;
                            dbContext.MPOfUploadFiles.Add(data);
                        }
                        else if (FileType.ToUpper() == "RPBZPHOTO")
                        {
                            RPOfUploadFiles data = new RPOfUploadFiles();
                            data.ID = fileID;
                            data.Name = fileName;
                            data.RPID = ID;
                            data.FileEx = fileEx;
                            data.State = Enums.UseState.Enable;
                            dbContext.RPOfUploadFiles.Add(data);
                        }
                        else if (FileType.ToUpper() == "RPREPAIRPHOTO")
                        {
                            RPPepairUploadFiles data = new Models.Entities.RPPepairUploadFiles();
                            data.ID = fileID;
                            data.Name = fileName;
                            data.RPRepairID = ID;
                            data.FileEx = fileEx;
                            data.RepairType = RepairType;
                            data.State = Enums.UseState.Enable;
                            dbContext.RPPepairUploadFiles.Add(data);
                        }
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

        public ActionResult RemovePicture(string ID, string FileType)
        {
            RtObj rt = null;
            try
            {
                using (var dbContext = SystemUtils.NewEFDbContext)
                {
                    var MPTypes = new List<string>() { "RESIDENCE", "ROAD", "COUNTRY" };
                    if (MPTypes.Contains(FileType.ToUpper()))
                    {
                        var query = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (query == null)
                            throw new Exception("该图片已经被删除！");
                        query.State = Enums.UseState.Delete;
                    }
                    else if (FileType.ToUpper() == "RPBZPHOTO")
                    {
                        var query = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (query == null)
                            throw new Exception("该图片已经被删除！");
                        query.State = Enums.UseState.Delete;
                    }
                    else if (FileType.ToUpper() == "RPREPAIRPHOTO")
                    {
                        var query = dbContext.RPPepairUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (query == null)
                            throw new Exception("该图片已经被删除！");
                        query.State = Enums.UseState.Delete;
                    }
                    dbContext.SaveChanges();
                }
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        public ContentResult GetPictureUrls(string ID, string FileType, string DocType, string RepairType = null)
        {
            RtObj rt = null;
            try
            {
                using (var dbContext = SystemUtils.NewEFDbContext)
                {
                    string[] mpTypes = new string[] { "RESIDENCE", "ROAD", "COUNTRY" };
                    List<Paths> paths = new List<Paths>();
                    if (mpTypes.Contains(FileType.ToUpper()))
                    {
                        var files = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == ID).Where(t => t.DocType == DocType).ToList();
                        foreach (var f in files)
                        {
                            var p = GetUploadFilePath(FileType, ID, f.ID, f.Name, RepairType);
                            paths.Add(p);
                        }
                    }
                    else if (FileType.ToUpper() == "RPBZPHOTO")
                    {
                        var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == ID).ToList();
                        foreach (var f in files)
                        {
                            var p = GetUploadFilePath(FileType, ID, f.ID, f.Name, RepairType);
                            paths.Add(p);
                        }
                    }
                    else if (FileType.ToUpper() == "RPREPAIRPHOTO")
                    {
                        var files = dbContext.RPPepairUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RepairType == RepairType).Where(t => t.RPRepairID == ID).ToList();
                        foreach (var f in files)
                        {
                            var p = GetUploadFilePath(FileType, ID, f.ID, f.Name, RepairType);
                            paths.Add(p);
                        }
                    }
                    else
                        throw new Exception("未知的图片类型");
                    rt = new RtObj(paths);
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