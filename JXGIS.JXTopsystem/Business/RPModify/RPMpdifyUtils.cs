using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPModify
{
    public class RPMpdifyUtils
    {
        public static void ModifyRP(RP oldData, RP newData, List<string> files, List<string> FileIDs)
        {
            if (!DistrictUtils.CheckPermission(newData.NeighborhoodsID))
                throw new Exception("无权操作其他镇街数据！");

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                #region 新增
                if (oldData == null)
                {
                    var CountyCode = SystemUtils.Districts.Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = SystemUtils.Districts.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var year = DateTime.Now.Year.ToString();
                    //地址编码  不带流水号，流水号由数据库触发器生成
                    var AddressCoding = CountyCode + NeighborhoodsCode + year;
                    var Position = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    //创建时间
                    var CreateTime = DateTime.Now.Date;
                    //使用状态
                    var State = Enums.UseState.Enable;
                    //GUID
                    var guid = Guid.NewGuid().ToString();
                    //Files && CodeFile
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        //保存路牌照片并更新路牌附件表
                        var Files = HttpContext.Current.Request.Files.GetMultiple(Enums.RPFilesType.RPImages);
                        var directory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "RP", guid);
                        SaveRPFiles(Files, directory, Enums.RPFilesType.RPImages, guid, dbContext, newData.Code);

                        //保存二维码图片
                        var codeFiles = HttpContext.Current.Request.Files.GetMultiple(Enums.RPFilesType.RPCodeImage);
                        var codeDirectory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "RP", "CodeFile");
                        SaveRPFiles(codeFiles, codeDirectory, Enums.RPFilesType.RPCodeImage, guid, dbContext, newData.Code);
                    }
                    newData.ID = guid;
                    newData.AddressCoding = AddressCoding;
                    newData.Position = Position;
                    newData.RepairedCount = 0;
                    newData.FinishRepaire = Enums.RPRepairFinish.Yes;
                    newData.State = State;
                    newData.CreateTime = CreateTime;
                    newData.CreateUser = LoginUtils.CurrentUser.UserName;
                }
                #endregion
                #region 修改
                else
                {
                    var query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == oldData.ID);
                    if (query.Count() == 0)
                        throw new Exception("该条路牌已被注销，请重新查询并编辑！");
                    dbContext.RP.Remove(query.First());

                    newData.ID = oldData.ID;
                    //单元空间位置
                    newData.Position = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : oldData.Position;
                    //修改时间
                    newData.LastModifyTime = DateTime.Now.Date;
                    newData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                    newData.State = oldData.State;
                    //修改路牌照片
                    var AddedFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.RPFilesType.RPImages);
                    var directory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "RP", oldData.ID);
                    UpdateRPFiles(FileIDs, AddedFiles, oldData.ID, directory, dbContext);
                    //修改二维码图片
                    var ModifyCodeFile = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.RPFilesType.RPCodeImage);
                    if (ModifyCodeFile[0].ContentLength > 0)
                    {
                        string codeDirectory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "RP", "CodeFile");
                        File.Delete(Path.Combine(codeDirectory, oldData.Code + ".jpg"));
                        SaveRPFiles(ModifyCodeFile, codeDirectory, Enums.RPFilesType.RPCodeImage, oldData.ID, dbContext, newData.Code);
                    }
                }
                #endregion
                dbContext.RP.Add(newData);
                dbContext.SaveChanges();
            }
        }

        private static void UpdateRPFiles(List<string> fileIDs, IList<HttpPostedFile> addedFiles, string RPID, string directory, SqlDBContext dbContext)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (fileIDs != null && fileIDs.Count() > 0)
            {
                string sql = $@"update [TopSystemDB].[dbo].[RPOFUPLOADFILES]  
                                    set [State]={Enums.UseState.Delete} 
                                    where [ID] not in ({string.Join(",", fileIDs)}) 
                                    and [State]={Enums.UseState.Enable} 
                                    and [RPID]='{RPID}'";
                var rt = dbContext.Database.ExecuteSqlCommand(sql);
            }
            SaveRPFiles(addedFiles, directory, Enums.RPFilesType.RPImages, RPID, dbContext, null);
        }

        private static void SaveRPFiles(IList<HttpPostedFile> files, string directory, string imageType, string guid, SqlDBContext dbContext, string Code)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (imageType == Enums.RPFilesType.RPImages)
            {
                foreach (var file in files)
                {
                    if (file.ContentLength > 0)
                    {
                        var id = Guid.NewGuid().ToString();
                        string filename = file.FileName;
                        string extension = Path.GetExtension(filename);
                        string newfilename = id + extension;
                        string fullPath = Path.Combine(directory, newfilename);
                        MemoryStream m = new MemoryStream();
                        FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
                        BinaryWriter w = new BinaryWriter(fs);
                        w.Write(m.ToArray());
                        fs.Close();
                        m.Close();
                        RPOfUploadFiles data = new RPOfUploadFiles();
                        data.ID = id;
                        data.RPID = guid;
                        data.Name = filename;
                        data.FileType = extension;
                        data.State = Enums.UseState.Enable;
                        dbContext.RPOfUploadFiles.Add(data);
                    }
                }
            }
            else
            {
                var file = files[0];
                if (file.ContentLength > 0)
                {
                    string filename = file.FileName;
                    string extension = Path.GetExtension(filename);
                    string newfilename = Code + extension;
                    string fullPath = Path.Combine(directory, newfilename);
                    MemoryStream m = new MemoryStream();
                    FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
                    BinaryWriter w = new BinaryWriter(fs);
                    w.Write(m.ToArray());
                    fs.Close();
                    m.Close();
                }
            }

        }
    }
}