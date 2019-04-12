using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class SPItemFileUtils
    {
        //public static string baseUrl = SystemUtils.BaseUrl;
        //public static string fileFolderName = "Files";
        //public static string fileDirectory = HttpContext.Current.Server.MapPath($"~/{fileFolderName}");//SystemUtils.BasePath;

        public static FILE SaveFile(HttpPostedFileBase file,
            string bussinessType,
            string formId,
            string certificateType)
        {
            FILE f = null;
            var tFile = new FileInfo(file.FileName);
            var fileOName = tFile.Name;
            var fileEx = tFile.Extension;

            var fileID = Guid.NewGuid().ToString();
            var fileName = fileID + fileEx;
            var fileDir = Path.Combine(StaticVariable.basePathSBFile, bussinessType, formId, certificateType);
            var fileFullPath = Path.Combine(fileDir, fileName);
            if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);

            file.SaveAs(fileFullPath);

            using (var db = SystemUtils.NewEFDbContext)
            {
                f = new FILE()
                {
                    ID = fileID,
                    FormID = formId,
                    BusinessType = bussinessType,
                    CertificateType = certificateType,
                    FileName = fileName,
                    FileOrginalName = fileOName,
                    CreateTime = DateTime.Now,
                    IsValid = 1
                };
                db.FILE.Add(f);
                db.SaveChanges();
            }
            return f;
        }

        public static void DeleteFile(string fileID)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var f = db.FILE.Find(fileID);
                if (f == null) throw new Error("未找到指定数据");
                f.IsValid = 0;
                db.SaveChanges();
            }
        }

        public static List<SPFile> GetFiles(string formID, string certificateType)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var files = db.FILE.Where(f => f.FormID == formID && f.CertificateType == certificateType && f.IsValid == 1).ToList();
                return (from f in files
                        select new SPFile()
                        {
                            id = f.ID,
                            name = f.FileOrginalName,
                            url = GetUrl(f)
                        }).ToList();
            }
        }

        public static string GetUrl(FILE file)
        {
            return $"{StaticVariable.basePathSBFile}/{file.BusinessType}/{file.FormID}/{file.CertificateType}/{file.FileName}";
        }
    }
    public class SPFile
    {
        public string id { get; set; }

        public string name { get; set; }

        public string url { get; set; }
    }
}