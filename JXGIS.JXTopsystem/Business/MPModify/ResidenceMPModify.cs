﻿using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

namespace JXGIS.JXTopsystem.Business.MPModify
{
    public class ResidenceMPModify
    {
        /// <summary>
        /// 一条数据的新增或者修改
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        ///  <param name="CurrentFileIDs">存储四个证件的所有ids</param>
        public static void ModifyResidenceMP(string oldDataJson)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<MPOfResidence>(oldDataJson);
                var targetData = db.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                if (targetData == null) //新增
                {
                    targetData = new MPOfResidence();
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Error("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckResidenceMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.ResidenceName, targetData.MPNumber, targetData.Dormitory, targetData.HSNumber, targetData.LZNumber, targetData.DYNumber))
                        throw new Error("该住宅门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                    var year = targetData.BZTime == null ? DateTime.Now.Year.ToString() : ((DateTime)(targetData.BZTime)).Year.ToString();
                    var dm = CountyCode + NeighborhoodsCode + year + mpCategory;
                    SqlParameter idx = new SqlParameter("@idx", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                    db.Database.ExecuteSqlCommand("exec getcode @code,@idx output;", new SqlParameter("@code", dm), idx);
                    var index = (int)idx.Value;
                    targetData.AddressCoding = dm + index.ToString().PadLeft(5, '0');
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.ID = Guid.NewGuid().ToString();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区下小区名称是否在字典表中存在，若不存在就新增
                    var ResidenceDic = new ResidenceDic();
                    ResidenceDic.CountyID = targetData.CountyID;
                    ResidenceDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    ResidenceDic.CommunityName = targetData.CommunityName;
                    ResidenceDic.ResidenceName = targetData.ResidenceName;
                    targetData.ResidenceID = DicUtils.AddResidenceDic(ResidenceDic);
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var MPNumber1 = string.IsNullOrEmpty(targetData.MPNumber) ? "" : targetData.MPNumber + "号";
                    var LZNumber1 = string.IsNullOrEmpty(targetData.LZNumber) ? "" : targetData.LZNumber + "幢";
                    var DYNumber1 = string.IsNullOrEmpty(targetData.DYNumber) ? "" : targetData.DYNumber + "单元";
                    var HSNumber1 = string.IsNullOrEmpty(targetData.HSNumber) ? "" : targetData.HSNumber + "室";
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + targetData.ResidenceName + LZNumber1 + MPNumber1 + DYNumber1 + HSNumber1;
                    targetData.StandardAddress = StandardAddress;
                    #endregion
                    //targetData.MPPosition = (targetData.MPPositionX != 0 && targetData.MPPositionY != 0 && targetData.MPPositionX != null && targetData.MPPositionY != null) ? (DbGeography.FromText($"POINT({targetData.MPPositionX} {targetData.MPPositionY})")) : null;
                    targetData.AddType = Enums.MPAddType.LX;
                    targetData.MPProduce = Enums.MPProduce.NO;
                    targetData.MPZPrintComplete = Enums.Complete.NO;
                    targetData.DZZMPrintComplete = Enums.Complete.NO;
                    targetData.State = Enums.UseState.Enable;
                    targetData.CreateTime = DateTime.Now;
                    targetData.CreateUser = LoginUtils.CurrentUser.UserID;
                    targetData.SBLY = Enums.SBLY.zj;
                    db.MPOfResidence.Add(targetData);
                }
                else //修改
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Error("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckResidenceMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.ResidenceName, targetData.MPNumber, targetData.Dormitory, targetData.HSNumber, targetData.LZNumber, targetData.DYNumber))
                        throw new Error("该住宅门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.ID = Guid.NewGuid().ToString();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区下小区名称是否在字典表中存在，若不存在就新增
                    var ResidenceDic = new ResidenceDic();
                    ResidenceDic.CountyID = targetData.CountyID;
                    ResidenceDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    ResidenceDic.CommunityName = targetData.CommunityName;
                    ResidenceDic.ResidenceName = targetData.ResidenceName;
                    targetData.ResidenceID = DicUtils.AddResidenceDic(ResidenceDic);
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var MPNumber1 = string.IsNullOrEmpty(targetData.MPNumber) ? "" : targetData.MPNumber + "号";
                    var LZNumber1 = string.IsNullOrEmpty(targetData.LZNumber) ? "" : targetData.LZNumber + "幢";
                    var DYNumber1 = string.IsNullOrEmpty(targetData.DYNumber) ? "" : targetData.DYNumber + "单元";
                    var HSNumber1 = string.IsNullOrEmpty(targetData.HSNumber) ? "" : targetData.HSNumber + "室";
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + targetData.ResidenceName + LZNumber1 + MPNumber1 + DYNumber1 + HSNumber1;
                    targetData.StandardAddress = StandardAddress;
                    #endregion
                    //targetData.DYPosition = (targetData.Lng != 0 && targetData.Lat != 0 && targetData.Lng != null && targetData.Lat != null) ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : targetData.DYPosition;
                    targetData.LastModifyTime = DateTime.Now;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserID;
                    BaseUtils.UpdateAddressCode(targetData, null, null, null, Enums.TypeInt.Residence);

                    if (targetData.DataPushStatus == 1)
                    {
                        targetData.DataPushStatus = 0;
                    }
                }
                db.SaveChanges();
            }
        }
        public static void CancelResidenceMP(List<string> ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => ID.Contains(t.ID)).ToList();
                if (ID.Count != query.Count)
                    throw new Error("部分门牌数据已被注销，请重新查询！");
                foreach (var q in query)
                {
                    q.State = Enums.UseState.Cancel;
                    q.CancelTime = DateTime.Now;
                    q.CancelUser = LoginUtils.CurrentUser.UserID;

                    if (q.DataPushStatus == 1)
                        q.DataPushStatus = 0;
                }
                dbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 检查住宅门牌是否重复 区+镇街+社区+小区+楼幢+单元+户室  或者  区+镇街+社区+道路+门牌+宿舍名+户室号  两个不能重复
        /// </summary>
        /// <param name="CountyID"></param>
        /// <param name="NeighborhoodsID"></param>
        /// <param name="CommunityID"></param>
        /// <param name="RoadID"></param>
        /// <param name="MPNumber"></param>
        /// <param name="Dormitory"></param>
        /// <param name="HSNumber"></param>
        /// <param name="ResidenceName"></param>
        /// <param name="LZNumber"></param>
        /// <param name="DYNumber"></param>
        /// <returns></returns>
        public static bool CheckResidenceMPIsAvailable(string ID, string CountyID, string NeighborhoodsID, string CommunityName, string ResidenceName, string MPNumber, string Dormitory, string HSNumber, string LZNumber, string DYNumber)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID != ID).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityName == CommunityName).Where(t => t.ResidenceName == ResidenceName).Where(t => t.MPNumber == MPNumber).Where(t => t.LZNumber == LZNumber).Where(t => t.DYNumber == DYNumber).Where(t => t.HSNumber == HSNumber).Count();
                return count == 0;
            }
        }


        ///// <summary>
        ///// 获取所有上传的数据和错误
        ///// </summary>
        ///// <param name="PageSize"></param>
        ///// <param name="PageNum"></param>
        ///// <returns></returns>
        //public static Dictionary<string, object> GetUploadResidenceMP(int PageSize, int PageNum)
        //{
        //    List<ResidenceMPDetails> rows = null;
        //    int totalCount = 0;
        //    var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<ResidenceMPDetails>;
        //    var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<ResidenceMPErrors>;
        //    var warnings = temp[LoginUtils.CurrentUser.UserName][warningKey] as List<string>;
        //    if (mps != null)
        //    {
        //        rows = mps.Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
        //        totalCount = mps.Count;
        //    }
        //    return new Dictionary<string, object> {
        //        { "Data",rows},
        //        { "Count",totalCount},
        //        { "Errors",errors},
        //        { "Warnings",warnings}
        //    };
        //}
        ///// <summary>
        ///// 将没有错误的数据更新到数据库 并进行批量门牌制作 返回制作汇总表
        ///// </summary>
        //public static SummarySheet UpdateResidenceMP()
        //{
        //    var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<ResidenceMPDetails>;
        //    var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<ResidenceMPErrors>;
        //    if (errors.Count > 0)
        //        throw new Error("数据包含错误信息，请先检查数据！");
        //    if (mps.Count() == 0 || mps == null)
        //        throw new Error("无可导入数据！");
        //    #region****************没有错误后将导入的数据更新到住宅门牌数据库中
        //    foreach (var mp in mps)
        //    {
        //        MPOfResidence m = new MPOfResidence
        //        {
        //            CountyID = mp.CountyID,
        //            NeighborhoodsID = mp.NeighborhoodsID,
        //            CommunityID = mp.CommunityID,
        //            ResidenceName = mp.ResidenceName,
        //            MPNumber = mp.MPNumber,
        //            Dormitory = mp.Dormitory,
        //            LZNumber = mp.LZNumber,
        //            DYNumber = mp.DYNumber,
        //            HSNumber = mp.HSNumber,
        //            PropertyOwner = mp.PropertyOwner,
        //            Applicant = mp.Applicant,
        //            ApplicantPhone = mp.ApplicantPhone,
        //            Postcode = mp.Postcode,
        //            SBDW = mp.SBDW,
        //            BZTime = mp.BZTime
        //        };
        //        ModifyResidenceMP(m, null, null, null, null, null);
        //    }
        //    temp.Remove(LoginUtils.CurrentUser.UserName);
        //    #endregion
        //    #region*************批量导入成功后立刻进行门牌制作，更新到门牌制作表中，再给出门牌制作汇总表********************
        //    var Residence = (from t in mps
        //                     group t by new
        //                     {
        //                         t.CountyID,
        //                         t.NeighborhoodsID,
        //                         t.CommunityID,
        //                         t.ResidenceName
        //                     } into g
        //                     select new Record
        //                     {
        //                         CountyID = g.Key.CountyID,
        //                         NeighborhoodsID = g.Key.NeighborhoodsID,
        //                         CommunityID = g.Key.CommunityID,
        //                         Name = g.Key.ResidenceName,
        //                         Type = "小区",
        //                         Count = 0
        //                     }).ToList();
        //    var LZMP = (from t in mps
        //                group t by new
        //                {
        //                    t.CountyID,
        //                    t.NeighborhoodsID,
        //                    t.CommunityID,
        //                    t.ResidenceName,
        //                    t.LZNumber
        //                } into g
        //                select new Record
        //                {
        //                    CountyID = g.Key.CountyID,
        //                    NeighborhoodsID = g.Key.NeighborhoodsID,
        //                    CommunityID = g.Key.CommunityID,
        //                    Name = g.Key.ResidenceName,
        //                    Type = g.Key.LZNumber,
        //                    Count = 2
        //                }).OrderBy(t => t.Type).ToList();

        //    LZMP.Add((from t in LZMP
        //              group t by new
        //              {
        //                  t.CountyID,
        //                  t.NeighborhoodsID,
        //                  t.CommunityID,
        //                  t.Name,
        //              } into g
        //              select new Record
        //              {
        //                  CountyID = g.Key.CountyID,
        //                  NeighborhoodsID = g.Key.NeighborhoodsID,
        //                  CommunityID = g.Key.CommunityID,
        //                  Name = g.Key.Name,
        //                  Type = "楼幢牌合计",
        //                  Count = g.Sum(t => t.Count)
        //              }).First());
        //    var dyDis = (from t in mps
        //                 select new ResidenceMPDetails
        //                 {
        //                     CountyID = t.CountyID,
        //                     NeighborhoodsID = t.NeighborhoodsID,
        //                     CommunityID = t.CommunityID,
        //                     ResidenceName = t.ResidenceName,
        //                     LZNumber = t.LZNumber,
        //                     DYNumber = t.DYNumber
        //                 }).Distinct();
        //    var DYMP = (from t in dyDis
        //                group t by new
        //                {
        //                    t.CountyID,
        //                    t.NeighborhoodsID,
        //                    t.CommunityID,
        //                    t.ResidenceName,
        //                    t.DYNumber
        //                } into g
        //                select new Record
        //                {
        //                    CountyID = g.Key.CountyID,
        //                    NeighborhoodsID = g.Key.NeighborhoodsID,
        //                    CommunityID = g.Key.CommunityID,
        //                    Name = g.Key.ResidenceName,
        //                    Type = g.Key.DYNumber,
        //                    Count = g.Count()
        //                }).OrderBy(t => t.Type).ToList();
        //    DYMP.Add((from t in DYMP
        //              group t by new
        //              {
        //                  t.CountyID,
        //                  t.NeighborhoodsID,
        //                  t.CommunityID,
        //                  t.Name,
        //              } into g
        //              select new Record
        //              {
        //                  CountyID = g.Key.CountyID,
        //                  NeighborhoodsID = g.Key.NeighborhoodsID,
        //                  CommunityID = g.Key.CommunityID,
        //                  Name = g.Key.Name,
        //                  Type = "单元牌合计",
        //                  Count = g.Sum(t => t.Count)
        //              }).First());

        //    var HSMP = (from t in mps
        //                group t by new
        //                {
        //                    t.CountyID,
        //                    t.NeighborhoodsID,
        //                    t.CommunityID,
        //                    t.ResidenceName,
        //                    t.HSNumber
        //                } into g
        //                select new Record
        //                {
        //                    CountyID = g.Key.CountyID,
        //                    NeighborhoodsID = g.Key.NeighborhoodsID,
        //                    CommunityID = g.Key.CommunityID,
        //                    Name = g.Key.ResidenceName,
        //                    Type = g.Key.HSNumber,
        //                    Count = g.Count()
        //                }).OrderBy(t => t.Type).ToList();

        //    HSMP.Add((from t in HSMP
        //              group t by new
        //              {
        //                  t.CountyID,
        //                  t.NeighborhoodsID,
        //                  t.CommunityID,
        //                  t.Name,
        //              } into g
        //              select new Record
        //              {
        //                  CountyID = g.Key.CountyID,
        //                  NeighborhoodsID = g.Key.NeighborhoodsID,
        //                  CommunityID = g.Key.CommunityID,
        //                  Name = g.Key.Name,
        //                  Type = "户室牌合计",
        //                  Count = g.Sum(t => t.Count)
        //              }).First());
        //    using (var dbContext = SystemUtils.NewEFDbContext)
        //    {
        //        List<Models.Entities.MPProduce> mpPros = new List<Models.Entities.MPProduce>();
        //        foreach (var residence in Residence)
        //        {
        //            Models.Entities.MPProduce mpPro = new Models.Entities.MPProduce();
        //            mpPro.ID = Guid.NewGuid().ToString();
        //            mpPro.CommunityID = residence.CommunityID;
        //            mpPro.NeighborhoodsID = residence.NeighborhoodsID;
        //            mpPro.CountyID = residence.CountyID;
        //            mpPro.MPType = Enums.MPType.Residence;
        //            mpPro.CreateTime = DateTime.Now.Date;
        //            mpPro.CreateUser = LoginUtils.CurrentUser.UserName;
        //            mpPro.LZMPCount = LZMP.Where(t => t.CountyID == residence.CountyID).Where(t => t.NeighborhoodsID == residence.NeighborhoodsID).Where(t => t.CommunityID == residence.CommunityID).Where(t => t.Name == residence.Name).Where(t => t.Type == "楼幢牌合计").Select(t => t.Count).First();
        //            mpPro.DYMPCount = DYMP.Where(t => t.CountyID == residence.CountyID).Where(t => t.NeighborhoodsID == residence.NeighborhoodsID).Where(t => t.CommunityID == residence.CommunityID).Where(t => t.Name == residence.Name).Where(t => t.Type == "单元牌合计").Select(t => t.Count).First();
        //            mpPro.HSMPCount = HSMP.Where(t => t.CountyID == residence.CountyID).Where(t => t.NeighborhoodsID == residence.NeighborhoodsID).Where(t => t.CommunityID == residence.CommunityID).Where(t => t.Name == residence.Name).Where(t => t.Type == "户室牌合计").Select(t => t.Count).First();
        //            mpPro.TotalCount = mpPro.LZMPCount + mpPro.DYMPCount + mpPro.HSMPCount;
        //            mpPros.Add(mpPro);
        //        }
        //        dbContext.MPProduce.AddRange(mpPros);
        //        dbContext.SaveChanges();
        //    }
        //    #endregion
        //    SummarySheet data = new SummarySheet();
        //    data.StandardName = Residence;
        //    data.LZMP = LZMP;
        //    data.DYMP = DYMP;
        //    data.HSMP = HSMP;
        //    return data;
        //}

    }
}