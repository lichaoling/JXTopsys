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
    public class CountryMPModify
    {
        public static void ModifyCountryMP(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<MPOfCountry>(oldDataJson);
                var targetData = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                if (targetData == null) //新增
                {
                    targetData = new MPOfCountry();
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckCountryMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.ViligeName, targetData.MPNumber, targetData.HSNumber))
                        throw new Exception("该农村门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == targetData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Country.Value.ToString();
                    var year = targetData.BZTime == null ? DateTime.Now.Year.ToString() : ((DateTime)(targetData.BZTime)).Year.ToString();
                    var AddressCoding = CountyCode + NeighborhoodsCode + year + mpCategory;
                    targetData.AddressCoding = AddressCoding;
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区下自然村名称是否在字典表中存在，若不存在就新增
                    var ViligeDic = new ViligeDic();
                    ViligeDic.CountyID = targetData.CountyID;
                    ViligeDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    ViligeDic.CommunityName = targetData.CommunityName;
                    ViligeDic.ViligeName = targetData.ViligeName;
                    targetData.ViligeID = DicUtils.AddViligeDic(ViligeDic);
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+自然村名称+门牌号码+户室号
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var HSNumber1 = targetData.HSNumber == null ? string.Empty : targetData.HSNumber + "室";
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + CommunityName + targetData.ViligeName + targetData.MPNumber + "号" + HSNumber1;
                    targetData.StandardAddress = StandardAddress;
                    #endregion
                    targetData.MPPosition = (targetData.Lng != null && targetData.Lat != null) ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : null;
                    targetData.AddType = Enums.MPAddType.LX;
                    targetData.MPProduce = targetData.MPProduce == null ? Enums.MPProduce.NO : targetData.MPProduce;
                    //targetData.MPProduceComplete = Enums.Complete.NO;
                    targetData.MPZPrintComplete = Enums.Complete.NO;
                    targetData.DZZMPrintComplete = Enums.Complete.NO;
                    targetData.State = Enums.UseState.Enable;
                    targetData.MPMail = targetData.MPMail == null ? Enums.MPMail.No : targetData.MPMail;
                    targetData.CreateTime = DateTime.Now;
                    targetData.CreateUser = LoginUtils.CurrentUser.UserName;
                    dbContext.MPOfCountry.Add(targetData);
                }
                else //修改
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckCountryMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.ViligeName, targetData.MPNumber, targetData.HSNumber))
                        throw new Exception("该农村门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区下自然村名称是否在字典表中存在，若不存在就新增
                    var ViligeDic = new ViligeDic();
                    ViligeDic.CountyID = targetData.CountyID;
                    ViligeDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    ViligeDic.CommunityName = targetData.CommunityName;
                    ViligeDic.ViligeName = targetData.ViligeName;
                    targetData.ViligeID = DicUtils.AddViligeDic(ViligeDic);
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+自然村名称+门牌号码+户室号
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var HSNumber1 = targetData.HSNumber == null ? string.Empty : targetData.HSNumber + "室";
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + CommunityName + targetData.ViligeName + targetData.MPNumber + "号" + HSNumber1;
                    targetData.StandardAddress = StandardAddress;
                    #endregion
                    targetData.MPPosition = (targetData.Lng != null && targetData.Lat != null) ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : targetData.MPPosition;
                    targetData.LastModifyTime = DateTime.Now;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                    BaseUtils.UpdateAddressCode(null, null, targetData, null, Enums.TypeInt.Country);
                }
                dbContext.SaveChanges();
            }
        }

        public static void CancelCountryMP(List<string> ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => ID.Contains(t.ID)).ToList();
                if (ID.Count != query.Count)
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
        public static bool CheckCountryMPIsAvailable(string ID, string CountyID, string NeighborhoodsID, string CommunityName, string ViligeName, string MPNumber, string HSNumber)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID != ID).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityName == CommunityName).Where(t => t.ViligeName == ViligeName).Where(t => t.MPNumber == MPNumber).Where(t => t.HSNumber == HSNumber).Count();
                return count == 0;
            }

        }

        //public static Dictionary<string, object> GetUploadCountryMP(int PageSize, int PageNum)
        //{
        //    List<CountryMPDetails> rows = null;
        //    int totalCount = 0;
        //    if (temp[LoginUtils.CurrentUser.UserName][mpKey] != null)
        //    {
        //        rows = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
        //        totalCount = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>).Count;
        //    }
        //    return new Dictionary<string, object> {
        //        { "Data",rows},
        //        { "Count",totalCount},
        //        { "Errors",(temp[LoginUtils.CurrentUser.UserName][errorKey] as List<CountryMPErrors>)},
        //        { "Warnings",(temp[LoginUtils.CurrentUser.UserName][warningKey] as List<string>)}
        //    };
        //}
        //public static SummarySheet UpdateCountryMP()
        //{
        //    var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>;
        //    var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<CountryMPErrors>;
        //    if (errors.Count > 0)
        //        throw new Exception("数据包含错误信息，请先检查数据！");
        //    if (mps.Count() == 0 || mps == null)
        //        throw new Exception("无可导入数据！");

        //    #region****************没有错误后将导入的数据更新到住宅门牌数据库中
        //    foreach (var mp in (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>))
        //    {
        //        MPOfCountry m = new MPOfCountry
        //        {
        //            CountyID = mp.CountyID,
        //            NeighborhoodsID = mp.NeighborhoodsID,
        //            CommunityID = mp.CommunityID,
        //            ViligeName = mp.ViligeName,
        //            MPNumber = mp.MPNumber,
        //            OriginalNumber = mp.OriginalNumber,
        //            HSNumber = mp.HSNumber,
        //            PropertyOwner = mp.PropertyOwner,
        //            CreateTime = mp.CreateTime,
        //            MPSize = mp.MPSize,
        //            Applicant = mp.Applicant,
        //            ApplicantPhone = mp.ApplicantPhone,
        //            LXMPProduce = mp.LXMPProduce
        //        };
        //        ModifyCountryMP(m, null, null, null);
        //    }
        //    temp.Remove(LoginUtils.CurrentUser.UserName);
        //    #endregion
        //    #region*************批量导入成功后立刻进行门牌制作，更新到门牌制作表中，再给出门牌制作汇总表********************
        //    using (var dbContext = SystemUtils.NewEFDbContext)
        //    {
        //        var bigMPsize = dbContext.DMBZDic.Where(t => t.Type == "大门牌").Select(t => t.Size).ToList();
        //        var Vilige = (from t in mps
        //                      group t by new
        //                      {
        //                          t.CountyID,
        //                          t.NeighborhoodsID,
        //                          t.CommunityID,
        //                          t.ViligeName
        //                      } into g
        //                      select new Record
        //                      {
        //                          CountyID = g.Key.CountyID,
        //                          NeighborhoodsID = g.Key.NeighborhoodsID,
        //                          CommunityID = g.Key.CommunityID,
        //                          Name = g.Key.ViligeName,
        //                          Type = "自然村",
        //                          Count = 0
        //                      }).ToList();
        //        var NCMP = (from t in mps
        //                    group t by new
        //                    {
        //                        t.CountyID,
        //                        t.NeighborhoodsID,
        //                        t.CommunityID,
        //                        t.ViligeName,
        //                        t.MPNumber,
        //                    } into g
        //                    select new Record
        //                    {
        //                        CountyID = g.Key.CountyID,
        //                        NeighborhoodsID = g.Key.NeighborhoodsID,
        //                        CommunityID = g.Key.CommunityID,
        //                        Name = g.Key.ViligeName,
        //                        Type = g.Key.MPNumber,
        //                        Count = g.Count()
        //                    }).OrderBy(t => t.Type).ToList();

        //        NCMP.Add((from t in NCMP
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
        //                      Type = "农村门牌合计",
        //                      Count = g.Sum(t => t.Count)
        //                  }).First());

        //        List<Models.Entities.MPProduce> mpPros = new List<Models.Entities.MPProduce>();
        //        foreach (var vilige in Vilige)
        //        {
        //            Models.Entities.MPProduce mpPro = new Models.Entities.MPProduce();
        //            mpPro.ID = Guid.NewGuid().ToString();
        //            mpPro.CommunityID = vilige.CommunityID;
        //            mpPro.NeighborhoodsID = vilige.NeighborhoodsID;
        //            mpPro.CountyID = vilige.CountyID;
        //            mpPro.MPType = Enums.MPType.Country;
        //            mpPro.CreateTime = DateTime.Now.Date;
        //            mpPro.CreateUser = LoginUtils.CurrentUser.UserName;
        //            mpPro.CountryMPCount = NCMP.Where(t => t.CountyID == vilige.CountyID).Where(t => t.NeighborhoodsID == vilige.NeighborhoodsID).Where(t => t.CommunityID == vilige.CommunityID).Where(t => t.Name == vilige.Name).Where(t => t.Type == "农村门牌合计").Select(t => t.Count).First();
        //            mpPro.TotalCount = mpPro.CountryMPCount;
        //            mpPros.Add(mpPro);
        //        }
        //        dbContext.MPProduce.AddRange(mpPros);
        //        dbContext.SaveChanges();
        //        SummarySheet data = new SummarySheet();
        //        data.StandardName = Vilige;
        //        return data;
        //    }
        //    #endregion
        //    //HttpContext.Current.Session["_CountryMP"] = null;
        //    //HttpContext.Current.Session["_CountryMPErrors"] = null;
        //    //HttpContext.Current.Session["_CountryMPWarning"] = null;
        //}
    }
}