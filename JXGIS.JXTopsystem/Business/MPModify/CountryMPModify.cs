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
        public static void ModifyCountryMP(MPOfCountry newData, string oldDataJson)
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
                    #region 农村门牌查重
                    if (!CheckCountryMPIsAvailable(newData.CountyID, newData.NeighborhoodsID, newData.CommunityName, newData.ViligeName, newData.MPNumber, newData.HSNumber))
                    {
                        throw new Exception("该农村门牌号已经存在，请检查后重新输入！");
                    }
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+自然村名称+门牌号码+户室号
                    var CountyName = newData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = newData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = newData.CommunityName;
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.ViligeName + newData.MPNumber + "号" + newData.HSNumber == null ? string.Empty : newData.HSNumber + "室";
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = dbContext.District.Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = dbContext.District.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Country.Value.ToString();
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
                    #region 检查这个行政区下自然村名称是否在字典表中存在，若不存在就新增
                    var ViligeDic = new ViligeDic();
                    ViligeDic.CountyID = newData.CountyID;
                    ViligeDic.NeighborhoodsID = newData.NeighborhoodsID;
                    ViligeDic.CommunityName = newData.CommunityName;
                    ViligeDic.ViligeName = newData.ViligeName;
                    newData.ViligeID = DicUtils.AddViligeDic(ViligeDic);
                    #endregion

                    //对这条数据进行默认赋值
                    newData.ID = Guid.NewGuid().ToString();
                    newData.AddressCoding = AddressCoding;
                    newData.MPPosition = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    newData.StandardAddress = StandardAddress;
                    newData.AddType = Enums.MPAddType.LX;
                    newData.MPProduce = newData.MPProduce == null ? Enums.MPProduce.NO : newData.MPProduce;
                    newData.MPProduceComplete = Enums.Complete.NO;
                    newData.MPZPrintComplete = Enums.Complete.NO;
                    newData.DZZMPrintComplete = Enums.Complete.NO;
                    newData.State = Enums.UseState.Enable;
                    newData.MPMail = newData.MPMail == null ? Enums.MPMail.No : newData.MPMail;
                    newData.CreateTime = DateTime.Now.Date;
                    newData.CreateUser = LoginUtils.CurrentUser.UserName;
                    dbContext.MPOfCountry.Add(newData);
                }
                #endregion
                #region 修改
                else
                {
                    var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<MPOfCountry>(oldDataJson);
                    var targetData = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                    if (targetData == null)
                        throw new Exception("该条数据已被注销，请重新查询并编辑！");
                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);

                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckCountryMPIsAvailable(targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.ViligeName, targetData.MPNumber, targetData.HSNumber))
                        throw new Exception("该农村门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+自然村名称+门牌号码+户室号
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.ViligeName + newData.MPNumber + "号" + newData.HSNumber == null ? string.Empty : newData.HSNumber + "室";
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

                    targetData.MPPosition = targetData.Lng != null && targetData.Lat != null ? (DbGeography.FromText($"POINT({targetData.Lng},{targetData.Lat})")) : targetData.MPPosition;
                    targetData.StandardAddress = StandardAddress;
                    targetData.LastModifyTime = DateTime.Now.Date;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                }
                #endregion
                dbContext.SaveChanges();
            }
        }
        public static void UploadCountryMP(HttpPostedFileBase file)
        {
            Stream fs = file.InputStream;
            Workbook wb = new Workbook(fs);
            if (wb == null || wb.Worksheets.Count == 0)
                throw new Exception("上传文件不包含有效数据！");

            Worksheet ws = wb.Worksheets[0];
            int rowCount = ws.Cells.Rows.Count;
            int columnCount = ws.Cells.Columns.Count;

            if (columnCount < 15)
                throw new Exception("数据列不完整！");

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var PLID = Guid.NewGuid().ToString();
                for (int i = 1; i < rowCount; i++)
                {
                    var error = new CountryMPErrors();
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
                    var ViligeName = row[4].Value == null ? null : row[4].Value.ToString().Trim();
                    var MPNumber = row[5].Value == null ? null : row[5].Value.ToString().Trim();
                    var OriginalMPAddress = row[6].Value == null ? null : row[6].Value.ToString().Trim();
                    var HSNumber = row[7].Value == null ? null : row[7].Value.ToString().Trim();
                    var PropertyOwner = row[8].Value == null ? null : row[8].Value.ToString().Trim();
                    var BZTime = row[9].Value == null ? null : row[9].Value.ToString().Trim();
                    var MPSize = row[10].Value == null ? null : row[10].Value.ToString().Trim();
                    var Applicant = row[11].Value == null ? null : row[11].Value.ToString().Trim();
                    var ApplicantPhone = row[12].Value == null ? null : row[12].Value.ToString().Trim();
                    var Postcode = row[13].Value == null ? null : row[13].Value.ToString().Trim();
                    var SBDW = row[14].Value == null ? null : row[14].Value.ToString().Trim();

                    string CountyID = $"嘉兴市.{CountyName}";
                    string NeighborhoodsID = $"{CountyID}.{NeighborhoodsName}";
                    var BZTimeBZ = DateTime.Now.Date;

                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
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
                    if (string.IsNullOrEmpty(ViligeName))
                    {
                        error.ErrorMessages.Add("自然村名称为空");
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
                    #region 农村门牌查重
                    if (!CheckCountryMPIsAvailable(CountyID, NeighborhoodsID, CommunityName, ViligeName, MPNumber, HSNumber))
                    {
                        throw new Exception("该农村门牌号已经存在，请检查后重新输入！");
                    }
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
                    #region 标准地址拼接 市辖区+镇街道+村社区+自然村名称+门牌号码+户室号
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + ViligeName + MPNumber + "号" + HSNumber == null ? string.Empty : HSNumber + "室";
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = CountyID;
                    CommunityDic.NeighborhoodsID = NeighborhoodsID;
                    CommunityDic.CommunityName = CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区下自然村名称是否在字典表中存在，若不存在就新增
                    var ViligeDic = new ViligeDic();
                    ViligeDic.CountyID = CountyID;
                    ViligeDic.NeighborhoodsID = NeighborhoodsID;
                    ViligeDic.CommunityName = CommunityName;
                    ViligeDic.ViligeName = ViligeName;
                    var ViligeID = DicUtils.AddViligeDic(ViligeDic);
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
                        var CountyCode = dbContext.District.Where(t => t.ID == CountyID).Select(t => t.Code).FirstOrDefault();
                        var NeighborhoodsCode = dbContext.District.Where(t => t.ID == NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        var mpCategory = SystemUtils.Config.MPCategory.Country.Value.ToString();
                        var year = DateTime.Now.Year.ToString();
                        AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
                        #endregion
                        var data = new MPOfCountry();
                        data.ID = Guid.NewGuid().ToString();
                        data.AddressCoding = AddressCoding;
                        data.CountyID = CountyID;
                        data.NeighborhoodsID = NeighborhoodsID;
                        data.CommunityName = CommunityName;
                        data.ViligeID = ViligeID;
                        data.ViligeName = ViligeName;
                        data.MPNumber = MPNumber;
                        data.OriginalMPAddress = OriginalMPAddress;
                        data.MPSize = MPSize;
                        data.HSNumber = HSNumber;
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

                        dbContext.MPOfCountry.Add(data);
                    }
                    #endregion

                    #region 批量更新
                    else
                    {
                        var data = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddressCoding == AddressCoding).First();
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
                        data.ViligeID = ViligeID;
                        data.ViligeName = ViligeName;
                        data.MPNumber = MPNumber;
                        data.OriginalMPAddress = OriginalMPAddress;
                        data.MPSize = MPSize;
                        data.HSNumber = HSNumber;
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
                    q.CancelTime = DateTime.Now.Date;
                    q.CancelUser = LoginUtils.CurrentUser.UserName;
                }
                dbContext.SaveChanges();
            }
        }
        public static bool CheckCountryMPIsAvailable(string CountyID, string NeighborhoodsID, string CommunityName, string ViligeName, string MPNumber, string HSNumber)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityName == CommunityName).Where(t => t.ViligeName == ViligeName).Where(t => t.MPNumber == MPNumber).Where(t => t.HSNumber == HSNumber).Count();
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