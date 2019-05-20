using Aspose.Cells;
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
    public class RoadMPModify
    {
        /// <summary>
        /// 一条数据的新增或者修改
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        public static void ModifyRoadMP(string oldDataJson)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<MPOfRoad>(oldDataJson);
                var targetData = db.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                if (targetData == null) //新增
                {
                    targetData = new MPOfRoad();
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Error("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckRoadMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.RoadName, targetData.MPNumber))
                        throw new Error("该道路门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Road.Value.ToString();
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
                    #region 检查这个行政区划道路名称、起止点、编制规则是否存在，若不存在就新增
                    var roadDic = new RoadDic();
                    roadDic.CountyID = targetData.CountyID;
                    roadDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    roadDic.CommunityName = targetData.CommunityName;
                    roadDic.RoadName = targetData.RoadName;
                    roadDic.RoadStart = targetData.RoadStart;
                    roadDic.RoadEnd = targetData.RoadEnd;
                    roadDic.BZRules = targetData.BZRules;
                    targetData.RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion
                    #region 门牌号码类型 单双号判断赋值
                    if (!string.IsNullOrEmpty(targetData.MPNumber))
                    {
                        int num = 0;
                        bool result = int.TryParse(targetData.MPNumber, out num);
                        if (result)
                            targetData.MPNumberType = num % 2 == 1 ? Enums.MPNumberType.Odd : Enums.MPNumberType.Even;
                        else
                            targetData.MPNumberType = Enums.MPNumberType.Other;
                    }
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+道路名称+门牌号码
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + targetData.RoadName + targetData.MPNumber + "号";
                    targetData.StandardAddress = StandardAddress;
                    #endregion
                    //targetData.MPPosition = (targetData.Lng != 0 && targetData.Lat != 0 && targetData.Lng != null && targetData.Lat != null) ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : null;
                    targetData.AddType = Enums.MPAddType.LX;
                    targetData.MPProduce = targetData.MPProduce == null ? Enums.MPProduce.NO : targetData.MPProduce;
                    targetData.MPZPrintComplete = Enums.Complete.NO;
                    targetData.DZZMPrintComplete = Enums.Complete.NO;
                    targetData.State = Enums.UseState.Enable;
                    targetData.CreateTime = DateTime.Now;
                    targetData.CreateUser = LoginUtils.CurrentUser.UserName;
                    targetData.SBLY = Enums.SBLY.zj;
                    db.MPOfRoad.Add(targetData);
                }
                else //修改
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Error("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckRoadMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.RoadName, targetData.MPNumber))
                        throw new Error("该道路门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.ID = Guid.NewGuid().ToString();
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
                    roadDic.RoadStart = targetData.RoadStart;
                    roadDic.RoadEnd = targetData.RoadEnd;
                    roadDic.BZRules = targetData.BZRules;
                    targetData.RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion
                    #region 门牌号码类型 单双号判断赋值
                    if (!string.IsNullOrEmpty(targetData.MPNumber))
                    {
                        int num = 0;
                        bool result = int.TryParse(targetData.MPNumber, out num);
                        if (result)
                            targetData.MPNumberType = num % 2 == 1 ? Enums.MPNumberType.Odd : Enums.MPNumberType.Even;
                        else
                            targetData.MPNumberType = Enums.MPNumberType.Other;
                    }
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+道路名称+门牌号码
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + targetData.RoadName + targetData.MPNumber + "号";
                    targetData.StandardAddress = StandardAddress;
                    #endregion
                    //targetData.MPPosition = (targetData.Lng != null && targetData.Lat != null && targetData.Lng != 0 && targetData.Lat != 0) ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : targetData.MPPosition;
                    targetData.LastModifyTime = DateTime.Now;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                    BaseUtils.UpdateAddressCode(null, targetData, null, null, Enums.TypeInt.Road);

                    if (targetData.DataPushStatus == 1)
                    {
                        targetData.DataPushStatus = 0;
                    }
                }
                db.SaveChanges();
            }
        }

        public static void CancelRoadMP(List<string> ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => ID.Contains(t.ID)).ToList();
                if (ID.Count != query.Count)
                    throw new Error("部分门牌数据已被注销，请重新查询！");
                foreach (var q in query)
                {
                    q.State = Enums.UseState.Cancel;
                    q.CancelTime = DateTime.Now;
                    q.CancelUser = LoginUtils.CurrentUser.UserName;

                    if (q.DataPushStatus == 1)
                        q.DataPushStatus = 0;
                }
                dbContext.SaveChanges();
            }
        }
        public static bool CheckRoadMPIsAvailable(string ID, string CountyID, string NeighborhoodsID, string CommunityName, string RoadName, string MPNumber)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID != ID).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityName == CommunityName).Where(t => t.RoadName == RoadName).Where(t => t.MPNumber == MPNumber).Count();
                return count == 0;
            }
        }

        //public static Dictionary<string, object> GetUploadRoadMP(int PageSize, int PageNum)
        //{
        //    List<RoadMPDetails> rows = null;
        //    int totalCount = 0;
        //    if (temp[LoginUtils.CurrentUser.UserName][mpKey] != null)
        //    {
        //        rows = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<RoadMPDetails>).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
        //        totalCount = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<RoadMPDetails>).Count;
        //    }
        //    return new Dictionary<string, object> {
        //        { "Data",rows},
        //        { "Count",totalCount},
        //        { "Errors",(temp[LoginUtils.CurrentUser.UserName][errorKey] as List<RoadMPErrors>)},
        //        { "Warnings",(temp[LoginUtils.CurrentUser.UserName][warningKey] as List<string>)}
        //    };
        //}
        //public static SummarySheet UpdateRoadMP()
        //{
        //    var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<RoadMPDetails>;
        //    var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<RoadMPErrors>;
        //    if (errors.Count > 0)
        //        throw new Error("数据包含错误信息，请先检查数据！");
        //    if (mps.Count() == 0 || mps == null)
        //        throw new Error("无可导入数据！");
        //    #region****************没有错误后将导入的数据更新到住宅门牌数据库中
        //    foreach (var mp in mps)
        //    {
        //        MPOfRoad m = new MPOfRoad
        //        {
        //            CountyID = mp.CountyID,
        //            NeighborhoodsID = mp.NeighborhoodsID,
        //            CommunityID = mp.CommunityID,
        //            //RoadID = mp.RoadID,
        //            RoadName = mp.RoadName,
        //            MPNumberRange = mp.MPNumberRange,
        //            MPNumber = mp.MPNumber,
        //            ReservedNumber = mp.ReservedNumber,
        //            OriginalNumber = mp.OriginalNumber,
        //            ShopName = mp.ShopName,
        //            ResidenceName = mp.ResidenceName,
        //            PropertyOwner = mp.PropertyOwner,
        //            CreateTime = mp.CreateTime,
        //            MPSize = mp.MPSize,
        //            Applicant = mp.Applicant,
        //            ApplicantPhone = mp.ApplicantPhone,
        //            LXMPProduce = mp.LXMPProduce
        //        };
        //        ModifyRoadMP(m, null, null, null, null);
        //    }
        //    temp.Remove(LoginUtils.CurrentUser.UserName);
        //    #endregion
        //    #region*************批量导入成功后立刻进行门牌制作，更新到门牌制作表中，再给出门牌制作汇总表********************
        //    using (var dbContext = SystemUtils.NewEFDbContext)
        //    {
        //        var bigMPsize = dbContext.DMBZDic.Where(t => t.Type == "大门牌").Select(t => t.Size).ToList();
        //        var Road = (from t in mps
        //                    group t by new
        //                    {
        //                        t.CountyID,
        //                        t.NeighborhoodsID,
        //                        t.CommunityID,
        //                        t.RoadName
        //                    } into g
        //                    select new Record
        //                    {
        //                        CountyID = g.Key.CountyID,
        //                        NeighborhoodsID = g.Key.NeighborhoodsID,
        //                        CommunityID = g.Key.CommunityID,
        //                        Name = g.Key.RoadName,
        //                        Type = "道路",
        //                        Count = 0
        //                    }).ToList();
        //        var DLMP = (from t in mps
        //                    group t by new
        //                    {
        //                        t.CountyID,
        //                        t.NeighborhoodsID,
        //                        t.CommunityID,
        //                        t.RoadName,
        //                        t.MPNumber,
        //                        t.MPSize
        //                    } into g
        //                    select new Record
        //                    {
        //                        CountyID = g.Key.CountyID,
        //                        NeighborhoodsID = g.Key.NeighborhoodsID,
        //                        CommunityID = g.Key.CommunityID,
        //                        Name = g.Key.RoadName,
        //                        Type = g.Key.MPNumber,
        //                        Size = g.Key.MPSize,
        //                        Count = g.Count()
        //                    }).OrderBy(t => t.Type).ToList();

        //        DLMP.Add((from t in DLMP
        //                  group t by new
        //                  {
        //                      t.CountyID,
        //                      t.NeighborhoodsID,
        //                      t.CommunityID,
        //                      t.Name,
        //                  } into g
        //                  select new Record
        //                  {
        //                      CountyID = g.Key.CountyID,
        //                      NeighborhoodsID = g.Key.NeighborhoodsID,
        //                      CommunityID = g.Key.CommunityID,
        //                      Name = g.Key.Name,
        //                      Type = "道路门牌合计",
        //                      Count = g.Sum(t => t.Count)
        //                  }).First());

        //        List<Models.Entities.MPProduce> mpPros = new List<Models.Entities.MPProduce>();
        //        foreach (var road in Road)
        //        {
        //            Models.Entities.MPProduce mpPro = new Models.Entities.MPProduce();
        //            mpPro.ID = Guid.NewGuid().ToString();
        //            mpPro.CommunityID = road.CommunityID;
        //            mpPro.NeighborhoodsID = road.NeighborhoodsID;
        //            mpPro.CountyID = road.CountyID;
        //            mpPro.MPType = Enums.MPType.Road;
        //            mpPro.CreateTime = DateTime.Now.Date;
        //            mpPro.CreateUser = LoginUtils.CurrentUser.UserName;
        //            mpPro.BigMPCount = DLMP.Where(t => t.CountyID == road.CountyID).Where(t => t.NeighborhoodsID == road.NeighborhoodsID).Where(t => t.CommunityID == road.CommunityID).Where(t => t.Name == road.Name).Where(t => bigMPsize.Contains(t.Size)).Count();
        //            mpPro.TotalCount = DLMP.Where(t => t.CountyID == road.CountyID).Where(t => t.NeighborhoodsID == road.NeighborhoodsID).Where(t => t.CommunityID == road.CommunityID).Where(t => t.Name == road.Name).Where(t => t.Type == "道路门牌合计").Select(t => t.Count).First();
        //            mpPro.SmallMPCount = mpPro.TotalCount - mpPro.BigMPCount;
        //            mpPros.Add(mpPro);
        //        }
        //        dbContext.MPProduce.AddRange(mpPros);
        //        dbContext.SaveChanges();
        //        SummarySheet data = new SummarySheet();
        //        data.StandardName = Road;
        //        return data;
        //    }
        //    #endregion
        //}
    }
}