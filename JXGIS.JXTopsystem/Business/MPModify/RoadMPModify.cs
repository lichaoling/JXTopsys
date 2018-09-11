using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
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
        public static void ModifyRoadMP(MPOfRoad newData, string oldDataJson)
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
                    #region 重复性检查
                    if (!CheckRoadMPIsAvailable(newData.CountyID, newData.NeighborhoodsID, newData.CommunityName, newData.RoadName, newData.MPNumber))
                        throw new Exception("该道路门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+道路名称+门牌号码
                    var CountyName = newData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = newData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = newData.CommunityName;
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.RoadName + newData.MPNumber + "号";
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Road.Value.ToString();
                    var year = DateTime.Now.Year.ToString();
                    var AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
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
                    roadDic.RoadStart = newData.RoadStart;
                    roadDic.RoadEnd = newData.RoadEnd;
                    roadDic.BZRules = newData.BZRules;
                    newData.RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion
                    #region 门牌号码类型 单双号判断赋值
                    if (!string.IsNullOrEmpty(newData.MPNumber))
                    {
                        int num = 0;
                        bool result = int.TryParse(newData.MPNumber, out num);
                        if (result)
                            newData.MPNumberType = num % 2 == 1 ? Enums.MPNumberType.Odd : Enums.MPNumberType.Even;
                        else
                            newData.MPNumberType = Enums.MPNumberType.Other;
                    }
                    #endregion

                    //对这条数据进行默认赋值
                    newData.ID = Guid.NewGuid().ToString();
                    newData.AddressCoding = AddressCoding;
                    newData.MPPosition = (newData.Lng != 0 && newData.Lat != 0) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    newData.StandardAddress = StandardAddress;
                    newData.AddType = Enums.MPAddType.LX;
                    newData.MPProduce = newData.MPProduce == null ? Enums.MPProduce.NO : newData.MPProduce;
                    newData.MPProduceComplete = Enums.Complete.NO;
                    newData.MPZPrintComplete = Enums.Complete.NO;
                    newData.DZZMPrintComplete = Enums.Complete.NO;
                    newData.State = Enums.UseState.Enable;
                    newData.CreateTime = DateTime.Now.Date; ;
                    newData.CreateUser = LoginUtils.CurrentUser.UserName;
                    dbContext.MPOfRoad.Add(newData);
                }
                #endregion
                #region 修改
                else
                {
                    var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<MPOfRoad>(oldDataJson);
                    var targetData = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                    if (targetData == null)
                        throw new Exception("该条数据已被注销，请重新查询并编辑！");
                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);

                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckRoadMPIsAvailable(targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.RoadName, targetData.MPNumber))
                        throw new Exception("该道路门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+道路名称+门牌号码
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.RoadName + targetData.MPNumber + "号";
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

                    targetData.MPPosition = targetData.Lng != null && targetData.Lat != null ? (DbGeography.FromText($"POINT({targetData.Lng},{targetData.Lat})")) : targetData.MPPosition;
                    targetData.StandardAddress = StandardAddress;
                    targetData.LastModifyTime = DateTime.Now.Date;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;

                    BaseUtils.UpdateAddressCode(null, targetData, null, null, Enums.TypeInt.Road);
                }
                #endregion
                dbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 检查某条路的门牌号是否重复
        /// </summary>
        /// <param name="MPNumber"></param>
        /// <param name="RoadID"></param>
        /// <returns></returns>
        public static void UploadRoadMP(HttpPostedFileBase file)
        {
            Stream fs = file.InputStream;
            Workbook wb = new Workbook(fs);
            if (wb == null || wb.Worksheets.Count == 0)
                throw new Exception("上传文件不包含有效数据！");

            Worksheet ws = wb.Worksheets[0];
            int rowCount = ws.Cells.Rows.Count;
            int columnCount = ws.Cells.Columns.Count;

            if (columnCount < 21)
                throw new Exception("数据列不完整！");

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var PLID = Guid.NewGuid().ToString();
                for (int i = 1; i < rowCount; i++)
                {
                    var error = new RoadMPErrors();
                    string success = string.Empty;
                    Row row = ws.Cells.Rows[i];
                    if (row.IsBlank)
                    {
                        error.Index = i;
                        error.Title = string.Format("第{0}行有误", i);
                        error.ErrorMessages.Add($"第{i}行是空行");
                        continue;
                    }

                    var AddressCoding = row[0].Value == null ? null : row[0].Value.ToString().Trim();
                    var CountyName = row[1].Value == null ? null : row[1].Value.ToString().Trim();
                    var NeighborhoodsName = row[2].Value == null ? null : row[2].Value.ToString().Trim();
                    var CommunityName = row[3].Value == null ? null : row[3].Value.ToString().Trim();
                    var RoadName = row[4].Value == null ? null : row[4].Value.ToString().Trim();
                    var RoadStart = row[5].Value == null ? null : row[5].Value.ToString().Trim();
                    var RoadEnd = row[6].Value == null ? null : row[6].Value.ToString().Trim();
                    var BZRules = row[7].Value == null ? null : row[7].Value.ToString().Trim();
                    var MPNumberRange = row[8].Value == null ? null : row[8].Value.ToString().Trim();
                    var MPNumber = row[9].Value == null ? null : row[9].Value.ToString().Replace(" ", "");
                    var ReservedNumber = row[10].Value == null ? null : row[10].Value.ToString().Trim();
                    var OriginalMPAddress = row[11].Value == null ? null : row[11].Value.ToString().Trim();
                    var ShopName = row[12].Value == null ? null : row[12].Value.ToString().Trim();
                    var ResidenceName = row[13].Value == null ? null : row[13].Value.ToString().Trim();
                    var PropertyOwner = row[14].Value == null ? null : row[14].Value.ToString().Trim();
                    var BZTime = row[15].Value == null ? null : row[15].Value.ToString().Trim();
                    var MPSize = row[16].Value == null ? null : row[16].Value.ToString().Trim();
                    var Applicant = row[17].Value == null ? null : row[17].Value.ToString().Trim();
                    var ApplicantPhone = row[18].Value == null ? null : row[18].Value.ToString().Trim();
                    var Postcode = row[19].Value == null ? null : row[19].Value.ToString().Trim();
                    var SBDW = row[20].Value == null ? null : row[20].Value.ToString().Trim();

                    string CountyID = $"嘉兴市.{CountyName}";
                    string NeighborhoodsID = $"{CountyID}.{NeighborhoodsName}";
                    var BZTimeBZ = DateTime.Now.Date;

                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(NeighborhoodsID))
                        error.ErrorMessages.Add("无权操作其他镇街数据");
                    #endregion
                    #region 非空检查
                    if (string.IsNullOrEmpty(CountyName))
                    {
                        error.ErrorMessages.Add("市辖区名称为空");
                    }
                    if (string.IsNullOrEmpty(NeighborhoodsName))
                    {
                        error.ErrorMessages.Add("镇街道名称为空");
                    }
                    if (string.IsNullOrEmpty(RoadName))
                    {
                        error.ErrorMessages.Add("道路名称为空");
                    }
                    if (string.IsNullOrEmpty(MPNumber))
                    {
                        error.ErrorMessages.Add("门牌号不能为空");
                    }
                    if (string.IsNullOrEmpty(MPSize))
                    {
                        error.ErrorMessages.Add("门牌规格不能为空");
                    }
                    #endregion
                    #region 道路门牌查重
                    if (!CheckRoadMPIsAvailable(CountyID, NeighborhoodsID, CommunityName, RoadName, MPNumber))
                        error.ErrorMessages.Add("门牌号已经存在");
                    #endregion
                    #region 编制日期的格式检查
                    if (!string.IsNullOrEmpty(BZTime))
                    {
                        if (BaseUtils.CheckIsDatetime(ref BZTime))
                            BZTimeBZ = Convert.ToDateTime(BZTime);
                        else
                            error.ErrorMessages.Add("编制日期不是标准日期格式");
                    }
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+道路名称+门牌号码
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + RoadName + MPNumber + "号";
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = CountyID;
                    CommunityDic.NeighborhoodsID = NeighborhoodsID;
                    CommunityDic.CommunityName = CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区划道路名称、起止点、编制规则是否存在，若不存在就新增
                    var roadDic = new RoadDic();
                    roadDic.CountyID = CountyID;
                    roadDic.NeighborhoodsID = NeighborhoodsID;
                    roadDic.CommunityName = CommunityName;
                    roadDic.RoadName = RoadName;
                    roadDic.RoadStart = RoadStart;
                    roadDic.RoadEnd = RoadEnd;
                    roadDic.BZRules = BZRules;
                    var RoadID = DicUtils.AddRoadDic(roadDic);
                    #endregion
                    #region 门牌号码类型 单双号判断赋值
                    var MPNumberType = Enums.MPNumberType.Other;
                    if (!string.IsNullOrEmpty(MPNumber))
                    {
                        int num = 0;
                        bool result = int.TryParse(MPNumber, out num);
                        if (result)
                            MPNumberType = num % 2 == 1 ? Enums.MPNumberType.Odd : Enums.MPNumberType.Even;
                    }
                    #endregion

                    #region 批量新增
                    if (string.IsNullOrEmpty(AddressCoding))
                    {
                        if (error.ErrorMessages.Count > 0)
                        {
                            error.Index = i;
                            error.Title = string.Format("第{0}行有误", i);
                            //输出错误信息后接着下一条数据新增
                            continue;
                        }
                        #region 地址编码前10位拼接
                        var CountyCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == CountyID).Select(t => t.Code).FirstOrDefault();
                        var NeighborhoodsCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        var mpCategory = SystemUtils.Config.MPCategory.Road.Value.ToString();
                        var year = DateTime.Now.Year.ToString();
                        AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
                        #endregion
                        var data = new MPOfRoad();
                        data.ID = Guid.NewGuid().ToString();
                        data.AddressCoding = AddressCoding;
                        data.CountyID = CountyID;
                        data.NeighborhoodsID = NeighborhoodsID;
                        data.CommunityName = CommunityName;
                        data.RoadID = RoadID;
                        data.RoadName = RoadName;
                        data.ShopName = ShopName;
                        data.RoadStart = RoadStart;
                        data.RoadEnd = RoadEnd;
                        data.BZRules = BZRules;
                        data.ResidenceName = ResidenceName;
                        data.MPNumberRange = MPNumberRange;
                        data.MPNumber = MPNumber;
                        data.MPNumberType = MPNumberType;
                        data.ReservedNumber = ReservedNumber;
                        data.OriginalMPAddress = OriginalMPAddress;
                        data.MPSize = MPSize;
                        data.AddType = Enums.MPAddType.PL;
                        data.PLID = PLID;
                        data.MPProduce = Enums.MPProduce.Yes;
                        data.MPProduceComplete = Enums.Complete.NO;
                        data.MPMail = Enums.MPMail.No;
                        data.Postcode = Postcode;
                        data.PropertyOwner = PropertyOwner;
                        data.StandardAddress = StandardAddress;
                        data.Applicant = Applicant;
                        data.ApplicantPhone = ApplicantPhone;
                        data.SBDW = SBDW;
                        data.State = Enums.UseState.Enable;
                        data.CreateTime = DateTime.Now.Date;
                        data.CreateUser = LoginUtils.CurrentUser.UserName;
                        data.MPZPrintComplete = Enums.Complete.NO;
                        data.DZZMPrintComplete = Enums.Complete.NO;

                        dbContext.MPOfRoad.Add(data);
                    }
                    #endregion
                    #region 批量更新
                    else
                    {
                        var data = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddressCoding == AddressCoding).First();
                        if (data == null)
                            error.ErrorMessages.Add("地址编码有误");
                        if (error.ErrorMessages.Count > 0)
                        {
                            error.Index = i;
                            error.Title = string.Format("第{0}行有误", i);
                            //输出错误信息后接着下一条数据新增
                            continue;
                        }
                        data.CountyID = CountyID;
                        data.NeighborhoodsID = NeighborhoodsID;
                        data.CommunityName = CommunityName;
                        data.RoadID = RoadID;
                        data.RoadName = RoadName;
                        data.ShopName = ShopName;
                        data.RoadStart = RoadStart;
                        data.RoadEnd = RoadEnd;
                        data.BZRules = BZRules;
                        data.ResidenceName = ResidenceName;
                        data.MPNumberRange = MPNumberRange;
                        data.MPNumber = MPNumber;
                        data.MPNumberType = MPNumberType;
                        data.ReservedNumber = ReservedNumber;
                        data.OriginalMPAddress = OriginalMPAddress;
                        data.MPSize = MPSize;
                        data.Postcode = Postcode;
                        data.PropertyOwner = PropertyOwner;
                        data.StandardAddress = StandardAddress;
                        data.Applicant = Applicant;
                        data.ApplicantPhone = ApplicantPhone;
                        data.SBDW = SBDW;
                        data.LastModifyTime = DateTime.Now.Date;
                        data.LastModifyUser = LoginUtils.CurrentUser.UserName;
                    }
                    #endregion
                    success = $"第{i}行导入成功";
                }
                dbContext.SaveChanges();
            }
        }

        public static void CancelRoadMP(List<string> ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => ID.Contains(t.ID)).ToList();
                if (ID.Count != query.Count)
                    throw new Exception("部分门牌数据已被注销，请重新查询！");
                foreach (var q in query)
                {
                    q.State = Enums.UseState.Cancel;
                    q.CancelTime = DateTime.Now.Date;
                    q.CancelUser = LoginUtils.CurrentUser.UserName;
                }
                dbContext.SaveChanges();
            }
        }
        public static bool CheckRoadMPIsAvailable(string CountyID, string NeighborhoodsID, string CommunityName, string RoadName, string MPNumber)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityName == CommunityName).Where(t => t.RoadName == RoadName).Where(t => t.MPNumber == MPNumber).Count();
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
        //        throw new Exception("数据包含错误信息，请先检查数据！");
        //    if (mps.Count() == 0 || mps == null)
        //        throw new Exception("无可导入数据！");
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