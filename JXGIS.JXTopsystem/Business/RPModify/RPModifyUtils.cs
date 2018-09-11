﻿using JXGIS.JXTopsystem.Business.Common;
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
        public static void ModifyRP(RP newData, string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                #region 新增
                if (string.IsNullOrEmpty(oldDataJson))
                {
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(newData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var year = DateTime.Now.Year.ToString();
                    var AddressCoding = CountyCode + NeighborhoodsCode + year;
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = newData.CountyID;
                    CommunityDic.NeighborhoodsID = newData.NeighborhoodsID;
                    CommunityDic.CommunityName = newData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区划道路名称、起止点、编制规则是否存在，若不存在就新增
                    var roadDic = new RoadDic();
                    roadDic.CountyID = newData.CountyID;
                    roadDic.NeighborhoodsID = newData.NeighborhoodsID;
                    roadDic.CommunityName = newData.CommunityName;
                    roadDic.RoadName = newData.RoadName;
                    roadDic.Intersection = newData.Intersection;
                    roadDic.BZRules = newData.BZRules;
                    roadDic.StartEndNum = newData.StartEndNum;
                    newData.RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion
                    //对这条数据进行默认赋值
                    newData.ID = Guid.NewGuid().ToString();
                    newData.AddressCoding = AddressCoding;
                    newData.Position = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    newData.RepairedCount = 0;
                    newData.FinishRepaire = Enums.RPRepairFinish.Yes;
                    newData.State = Enums.UseState.Enable;
                    newData.CreateTime = DateTime.Now.Date;
                    newData.CreateUser = LoginUtils.CurrentUser.UserName;
                    //生成二维码图和缩略图
                    newData.Code = dbContext.RP.Max(t => t.Code) + 1;
                    string strCode = "http://www.walys.com";
                    QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrcode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false);
                    Image img = qrCodeImage;
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", Enums.TypeStr.RP, Enums.RPFileType.QRCode);
                    string fileName = newData.Code + ".jpg";
                    string fileNameThum = "t-" + newData.Code + ".jpg";
                    string path = Path.Combine(filePath, fileName);
                    string pathThum = Path.Combine(filePath, fileNameThum);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    qrCodeImage.Save(path);
                    Image imgThum = PictureUtils.GetHvtThumbnail(img, 200);
                    imgThum.Save(pathThum);
                    dbContext.RP.Add(newData);
                }
                #endregion
                #region 修改
                else
                {
                    var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<RP>(oldDataJson);
                    var targetData = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                    if (targetData == null)
                        throw new Exception("该条路牌已被注销，请重新查询并编辑！");
                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
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
                    roadDic.RoadName = newData.RoadName;
                    roadDic.Intersection = targetData.Intersection;
                    roadDic.BZRules = targetData.BZRules;
                    roadDic.StartEndNum = targetData.StartEndNum;
                    targetData.RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion

                    targetData.Position = targetData.Lng != null && targetData.Lat != null ? (DbGeography.FromText($"POINT({targetData.Lng},{targetData.Lat})")) : targetData.Position;
                    targetData.LastModifyTime = DateTime.Now.Date;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;

                    BaseUtils.UpdateAddressCode(null, null, null, targetData, Enums.TypeInt.RP);
                }
                #endregion
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