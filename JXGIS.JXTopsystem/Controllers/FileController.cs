using JXGIS.JXTopsystem.App_Start;
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
                    relativePath = StaticVariable.residenceMPRelativePath;
                    break;
                case "ROAD":
                    relativePath = StaticVariable.roadMPRelativePath;
                    break;
                case "COUNTRY":
                    relativePath = StaticVariable.countryMPRelativePath;
                    break;
                case "RPBZPHOTO":
                    relativePath = StaticVariable.RPBZPhotoRelativePath;
                    break;
                case "RPREPAIRPHOTO":
                    relativePath = StaticVariable.RPRepairPhotoRelativePath;
                    break;
                default:
                    throw new Exception("未知的文件目录");
            }
            relativePath = Path.Combine(relativePath, ID);
            string fullPath = Path.Combine(StaticVariable.basePath, relativePath);

            if (FileType.ToUpper() == "RPREPAIRPHOTO")
            {
                fullPath = Path.Combine(fullPath, RepairType);
                relativePath = Path.Combine(relativePath, RepairType);
            }
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            string rPath = Path.Combine(relativePath, fileID + fileEx);
            string fPath = Path.Combine(fullPath, fileID + fileEx);
            string trPath = Path.Combine(relativePath, "t-" + fileID + fileEx);
            string tfPath = Path.Combine(fullPath, "t-" + fileID + fileEx);

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
        [LoggerFilter(Description = "根据ID、文件类型、证件类型上传附件")]
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

                    if (IsPicture(fileName))//如果是图片，就保存缩略图
                    {
                        // 保存缩略图片
                        Image image = Image.FromStream(file.InputStream);
                        image = PictureUtils.GetHvtThumbnail(image, 200);
                        image.Save(paths.TFullPath);
                    }

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

        public class Photos
        {
            public string Id { get; set; }

            public string Photo { get; set; }

            public int Code { get; set; }

            public int Year { get; set; }
        }

        //public ActionResult ImportTest()
        //{
        //    using (var db = SystemUtils.NewEFDbContext)
        //    {
        //        using (var ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(6000000000)))
        //        {

        //            var photoPath = @"E:\Projects\09 嘉兴地名\路牌导入\路牌数据\2016-2018数据整理\{0}\{1}";

        //            var ps = db.Database.SqlQuery<Photos>("select rp.ID,t.* from photos t left join rp on rp.Code=t.Code").ToList();
        //            foreach (var p in ps)
        //            {
        //                var photo = p.Photo;
        //                if (!string.IsNullOrEmpty(photo))
        //                {
        //                    var photos = photo.Split(';');
        //                    foreach (var pts in photos)
        //                    {
        //                        var filePath = string.Format(photoPath, p.Year, pts);

        //                        if (System.IO.File.Exists(filePath))
        //                        {
        //                            var ID = p.Id;
        //                            var fileName = new FileInfo(filePath).Name;
        //                            var fileID = System.Guid.NewGuid().ToString();
        //                            var fileEx = new FileInfo(fileName).Extension;
        //                            var paths = GetUploadFilePath("RPBZPHOTO", ID, fileID, fileName, null);
        //                            System.IO.File.Copy(filePath, paths.FullPath);

        //                            using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open))
        //                            {
        //                                // 保存缩略图片
        //                                Image image = Image.FromStream(fs);
        //                                image = PictureUtils.GetHvtThumbnail(image, 200);
        //                                image.Save(paths.TFullPath);
        //                            }

        //                            // 保存到数据库中
        //                            // 文件ID，门牌记录的ID，图片相对路径，缩略图相对路径，文件名称等
        //                            RPOfUploadFiles data = new RPOfUploadFiles();
        //                            data.ID = fileID;
        //                            data.Name = fileName;
        //                            data.RPID = ID;
        //                            data.FileEx = fileEx;
        //                            data.State = Enums.UseState.Enable;
        //                            db.RPOfUploadFiles.Add(data);

        //                        }
        //                    }
        //                }
        //            }
        //            db.SaveChanges();
        //            ts.Complete();
        //        }
        //    }
        //    return null;
        //}

        [LoggerFilter(Description = "根据ID、文件类型删除附件")]
        public ActionResult RemovePicture(string ID, string FileType)
        {
            RtObj rt = null;
            try
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    var MPTypes = new List<string>() { "RESIDENCE", "ROAD", "COUNTRY" };
                    if (MPTypes.Contains(FileType.ToUpper()))
                    {
                        var query = db.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (query == null)
                            throw new Exception("该图片已经被删除！");

                        var paths = GetUploadFilePath(FileType, query.MPID, query.ID, query.Name, null);
                        DeleteFileByPath(paths);
                        db.MPOfUploadFiles.Remove(query);
                        //query.State = Enums.UseState.Delete;
                    }
                    else if (FileType.ToUpper() == "RPBZPHOTO")
                    {
                        var query = db.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (query == null)
                            throw new Exception("该图片已经被删除！");

                        var paths = GetUploadFilePath(FileType, query.RPID, query.ID, query.Name, null);
                        DeleteFileByPath(paths);
                        db.RPOfUploadFiles.Remove(query);
                        //query.State = Enums.UseState.Delete;
                    }
                    else if (FileType.ToUpper() == "RPREPAIRPHOTO")
                    {
                        var query = db.RPPepairUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (query == null)
                            throw new Exception("该图片已经被删除！");

                        var paths = GetUploadFilePath(FileType, query.RPRepairID, query.ID, query.Name, query.RepairType);
                        DeleteFileByPath(paths);
                        db.RPPepairUploadFiles.Remove(query);
                        //query.State = Enums.UseState.Delete;
                    }
                    db.SaveChanges();
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
        [LoggerFilter(Description = "根据ID获取所有附件地址")]
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


        // 判断文件是否是图片
        public bool IsPicture(string fileName)
        {
            string strFilter = ".jpeg|.gif|.jpg|.png|.bmp|.pic|.tiff|.ico|.iff|.lbm|.mag|.mac|.mpt|.opt|";
            char[] separtor = { '|' };
            string[] tempFileds = StringSplit(strFilter, separtor);
            foreach (string str in tempFileds)
            {
                if (str.ToUpper() == fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")).ToUpper()) { return true; }
            }
            return false;
        }
        // 通过字符串，分隔符返回string[]数组 
        public string[] StringSplit(string s, char[] separtor)
        {
            string[] tempFileds = s.Trim().Split(separtor); return tempFileds;
        }

        public static void DeleteFileByPath(Paths paths)
        {
            System.IO.File.Delete(paths.FullPath);
            System.IO.File.Delete(paths.TFullPath);
            var directory = Path.GetDirectoryName(paths.FullPath);
            DirectoryInfo root = new DirectoryInfo(directory);
            FileInfo[] files = root.GetFiles();
            if (files.Count() == 0)
                Directory.Delete(directory);
        }
    }
}