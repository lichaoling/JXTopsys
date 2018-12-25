using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Controllers;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPModify
{
    public class RPModifyUtils
    {
        public static void ModifyRP(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<RP>(oldDataJson);
                var targetData = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                if (targetData == null) //新增
                {
                    targetData = new RP();
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var year = DateTime.Now.Year.ToString();
                    var AddressCoding = CountyCode + NeighborhoodsCode + year;
                    targetData.AddressCoding = AddressCoding;
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区划道路名称、起止点、编制规则是否存在，若不存在就新增
                    var roadDic = new RoadDic();
                    roadDic.CountyID = targetData.CountyID;
                    roadDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    roadDic.CommunityName = targetData.CommunityName;
                    roadDic.RoadName = targetData.RoadName;
                    roadDic.Intersection = targetData.Intersection;
                    roadDic.BZRules = targetData.BZRules;
                    roadDic.StartEndNum = targetData.StartEndNum;
                    targetData.RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion
                    targetData.Position = (targetData.Lng != null && targetData.Lat != null) ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : null;
                    targetData.RepairedCount = 0;
                    targetData.FinishRepaire = Enums.RPRepairFinish.Yes;
                    targetData.State = Enums.UseState.Enable;
                    targetData.CreateTime = DateTime.Now;
                    targetData.CreateUser = LoginUtils.CurrentUser.UserName;



                    //生成二维码图和缩略图并保存
                    //var tt = dbContext.RP.Max(t => t.Code);
                    targetData.Code = (int)dbContext.Database.SqlQuery<Int64>("select next value for sqc_rp_qrcode").FirstOrDefault();
                    SaveQRCodeController saveQRCodeController = new SaveQRCodeController();
                    saveQRCodeController.SaveQRCodeImgs(targetData.Code);

                    //string strCode = $"http://www.cristbin.com/DMQuery_wx/LPYH/BaseInfo?Id={targetData.Code}";
                    //QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                    //QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.H);
                    //QRCode qrcode = new QRCode(qrCodeData);
                    //Bitmap qrCodeImage = qrcode.GetGraphic(10, Color.Black, Color.White, null, 960, 960, false);
                    //Image img = qrCodeImage;

                    ////位图宽高
                    //int Margin = 80;
                    //int width = img.Width + Margin;
                    //int height = img.Height + Margin;
                    //Bitmap BitmapResult = new Bitmap(width, height);
                    //Graphics Grp = Graphics.FromImage(BitmapResult);
                    //SolidBrush b = new SolidBrush(Color.White);//这里修改颜色
                    //Grp.FillRectangle(b, 0, 0, width, height);
                    //System.Drawing.Rectangle Rec = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                    ////向矩形框内填充Img
                    //Grp.DrawImage(img, Margin / 2, Margin / 2, Rec, GraphicsUnit.Pixel);
                    ////返回位图文件
                    //Grp.Dispose();
                    //GC.Collect();

                    //string filePath = Path.Combine(FileController.uploadBasePath, FileController.RPQRCodeRelativePath);
                    //string fileName = targetData.Code + ".jpg";
                    //string fileNameThum = "t-" + targetData.Code + ".jpg";
                    //string path = Path.Combine(filePath, fileName);
                    //string pathThum = Path.Combine(filePath, fileNameThum);
                    //if (!Directory.Exists(filePath))
                    //{
                    //    Directory.CreateDirectory(filePath);
                    //}
                    //BitmapResult.Save(path);
                    //Image imgThum = PictureUtils.GetHvtThumbnail(BitmapResult, 200);
                    //imgThum.Save(pathThum);
                    dbContext.RP.Add(targetData);
                }
                else //修改
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区划道路名称、起止点、编制规则是否存在，若不存在就新增
                    var roadDic = new RoadDic();
                    roadDic.CountyID = targetData.CountyID;
                    roadDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    roadDic.CommunityName = targetData.CommunityName;
                    roadDic.RoadName = targetData.RoadName;
                    roadDic.Intersection = targetData.Intersection;
                    roadDic.BZRules = targetData.BZRules;
                    roadDic.StartEndNum = targetData.StartEndNum;
                    targetData.RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion
                    targetData.Position = targetData.Lng != null && targetData.Lat != null ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : targetData.Position;
                    targetData.LastModifyTime = DateTime.Now;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                    BaseUtils.UpdateAddressCode(null, null, null, targetData, Enums.TypeInt.RP);
                }
                dbContext.SaveChanges();
            }
        }

        public static void CancelRP(List<string> IDs)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => IDs.Contains(t.ID)).ToList();
                if (IDs.Count != query.Count)
                    throw new Exception("部分门牌数据已被注销，请重新查询！");
                foreach (var q in query)
                {
                    q.State = Enums.UseState.Cancel;
                    q.CancelTime = DateTime.Now;
                    q.CancelUser = LoginUtils.CurrentUser.UserName;
                }
                dbContext.SaveChanges();
            }
        }
        //private static void UpdateRPFiles(List<string> fileIDs, IList<HttpPostedFile> addedFiles, string RPID, string directory, SqlDBContext dbContext)
        //{
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    if (fileIDs != null && fileIDs.Count() > 0)
        //    {
        //        string sql = $@"update [TopSystemDB].[dbo].[RPOFUPLOADFILES]  
        //                            set [State]={Enums.UseState.Delete} 
        //                            where [ID] not in ({string.Join(",", fileIDs)}) 
        //                            and [State]={Enums.UseState.Enable} 
        //                            and [RPID]='{RPID}'";
        //        var rt = dbContext.Database.ExecuteSqlCommand(sql);
        //    }
        //    SaveRPFiles(addedFiles, directory, Enums.RPFileType.BZPhoto, RPID, dbContext, null);
        //}

        //private static void SaveRPFiles(IList<HttpPostedFile> files, string directory, string imageType, string guid, SqlDBContext dbContext, string Code)
        //{
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    if (imageType == Enums.RPFileType.BZPhoto)
        //    {
        //        foreach (var file in files)
        //        {
        //            if (file.ContentLength > 0)
        //            {
        //                var id = Guid.NewGuid().ToString();
        //                string filename = file.FileName;
        //                string extension = Path.GetExtension(filename);
        //                string newfilename = id + extension;
        //                string fullPath = Path.Combine(directory, newfilename);
        //                MemoryStream m = new MemoryStream();
        //                FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
        //                BinaryWriter w = new BinaryWriter(fs);
        //                w.Write(m.ToArray());
        //                fs.Close();
        //                m.Close();
        //                RPOfUploadFiles data = new RPOfUploadFiles();
        //                data.ID = id;
        //                data.RPID = guid;
        //                data.Name = filename;
        //                data.FileEx = extension;
        //                data.State = Enums.UseState.Enable;
        //                dbContext.RPOfUploadFiles.Add(data);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var file = files[0];
        //        if (file.ContentLength > 0)
        //        {
        //            string filename = file.FileName;
        //            string extension = Path.GetExtension(filename);
        //            string newfilename = Code + extension;
        //            string fullPath = Path.Combine(directory, newfilename);
        //            MemoryStream m = new MemoryStream();
        //            FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
        //            BinaryWriter w = new BinaryWriter(fs);
        //            w.Write(m.ToArray());
        //            fs.Close();
        //            m.Close();
        //        }
        //    }

        //}
    }
}