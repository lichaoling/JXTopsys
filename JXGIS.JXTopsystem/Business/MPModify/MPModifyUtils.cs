using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
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

    public class MPModifyUtils
    {
        private static Dictionary<string, Dictionary<string, object>> temp = new Dictionary<string, Dictionary<string, object>>();
        private const string mpKey = "_MP", errorKey = "_MPErrors", warningKey = "_MPWarning";

        #region 住宅门牌
        /// <summary>
        /// 一条数据的新增或者修改
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        ///  <param name="CurrentFileIDs">存储四个证件的所有ids</param>
        public static void ModifyResidenceMP(MPOfResidence newData, string oldDataJson, List<string> FCZIDs, List<string> TDZIDs, List<string> BDCZIDs, List<string> HJIDs)
        {
            #region 权限检查
            if (!DistrictUtils.CheckPermission(newData.CommunityID))
                throw new Exception("无权操作其他镇街数据！");
            #endregion
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                #region 新增
                if (string.IsNullOrEmpty(oldDataJson))
                {
                    #region 基本检查+重复性检查
                    //if (string.IsNullOrEmpty(newData.ResidenceName))
                    //{
                    //    throw new Exception("小区名不能为空！");
                    //}

                    //if (!string.IsNullOrEmpty(newData.MPNumber))
                    //{
                    //    newData.MPNumber = newData.MPNumber.Replace(" ", "");
                    //    if (!CheckIsNumber(newData.MPNumber))
                    //        throw new Exception("门牌号不是纯数字！");
                    //}

                    //if (!string.IsNullOrEmpty(newData.LZNumber))
                    //{
                    //    newData.LZNumber = newData.LZNumber.Replace(" ", "");
                    //    if (!CheckIsNumber(newData.LZNumber))
                    //        throw new Exception("楼幢号不是纯数字！");
                    //}

                    //if (!string.IsNullOrEmpty(newData.DYNumber))
                    //{
                    //    newData.DYNumber = newData.DYNumber.Replace(" ", "");
                    //    if (!CheckIsNumber(newData.DYNumber))
                    //        throw new Exception("单元号不是纯数字！");
                    //}

                    //if (string.IsNullOrEmpty(newData.HSNumber))
                    //{
                    //    throw new Exception("户室号为空！");
                    //}
                    //else
                    //{
                    //    newData.HSNumber = newData.HSNumber.Replace(" ", "");
                    //    if (!CheckIsNumber(newData.HSNumber))
                    //        throw new Exception("户室号不是纯数字！");
                    //}
                    //if (!string.IsNullOrEmpty(newData.ApplicantPhone))
                    //{
                    //    if (!CheckIsPhone(newData.ApplicantPhone))
                    //        throw new Exception("申办人联系电话不是正确的号码格式！");
                    //}
                    if (!CheckResidenceMPIsAvailable(newData.CountyID, newData.NeighborhoodsID, newData.CommunityID, newData.ResidenceName, newData.MPNumber, newData.Dormitory, newData.HSNumber, newData.LZNumber, newData.DYNumber))
                        throw new Exception("该户室牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 标准地址拼接
                    //拼接标准住宅门牌地址，先获取对应的道路门牌的标准地址，再拼接好宿舍名、幢号、单元号、户室号
                    var CountyName = SystemUtils.Districts.Where(t => t.ID == newData.CountyID).Select(t => t.Name).FirstOrDefault();
                    var NeighborhoodsName = SystemUtils.Districts.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Name).FirstOrDefault();
                    var CommunityName = SystemUtils.Districts.Where(t => t.ID == newData.CommunityID).Select(t => t.Name).FirstOrDefault();
                    //市辖区/镇街道/村社区/小区名/门牌号/宿舍名/幢号/单元号/房室号
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.ResidenceName + newData.MPNumber == null ? "" : newData.MPNumber + "号" + newData.Dormitory + newData.LZNumber == null ? "" : newData.LZNumber + "幢" + newData.DYNumber == null ? "" : newData.DYNumber + "单元" + newData.HSNumber == null ? "" : newData.HSNumber + "室";
                    #endregion
                    #region 地址编码前10位拼接
                    var guid = Guid.NewGuid().ToString();
                    var CountyCode = SystemUtils.Districts.Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = SystemUtils.Districts.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                    var year = DateTime.Now.Year.ToString();
                    ////不再使用，用数据库触发器
                    //var ResidenceSql = $@"select max(cast (right([AddressCoding],5) as int)) from [TopSystemDB].[dbo].[MPOFRESIDENCE]";
                    //var maxCode = dbContext.Database.SqlQuery<int>(ResidenceSql).FirstOrDefault();
                    //var Code = (maxCode + 1).ToString().PadLeft(5, '0');//向左补齐0，共5位
                    //地址编码  不带流水号，流水号由数据库触发器生成
                    var AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
                    #endregion
                    //单元空间位置
                    var DYPosition = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    //创建时间
                    var CreateTime = DateTime.Now.Date;
                    //使用状态
                    var State = Enums.UseState.Enable;
                    //获取所有上传的文件
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        var FCZFiles = HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.FCZ);
                        var TDZFiles = HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.TDZ);
                        var BDCZFiles = HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.BDCZ);
                        var HJFiles = HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.HJ);
                        SaveMPFilesByID(FCZFiles, guid, Enums.DocType.FCZ, Enums.MPTypeStr.ResidenceMP);
                        SaveMPFilesByID(TDZFiles, guid, Enums.DocType.TDZ, Enums.MPTypeStr.ResidenceMP);
                        SaveMPFilesByID(BDCZFiles, guid, Enums.DocType.BDCZ, Enums.MPTypeStr.ResidenceMP);
                        SaveMPFilesByID(HJFiles, guid, Enums.DocType.HJ, Enums.MPTypeStr.ResidenceMP);
                    }
                    //对这条数据进行默认赋值
                    newData.ID = guid;
                    newData.AddressCoding = AddressCoding;
                    newData.DYPosition = DYPosition;
                    newData.StandardAddress = StandardAddress;
                    newData.State = State;
                    newData.CreateTime = CreateTime;
                    newData.CreateUser = LoginUtils.CurrentUser.UserName;

                    dbContext.MPOFResidence.Add(newData);
                }
                #endregion
                #region 修改
                else
                {
                    //query = dbContext.MPOFResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == oldData.ID);
                    //count = query.Count();
                    //if (count == 0)
                    //    throw new Exception("该条数据已被注销，请重新查询并编辑！");
                    //dbContext.MPOFResidence.Remove(query.First());
                    //newData.ID = oldData.ID;
                    //newData.AddressCoding = AddressCoding;
                    //newData.DYPosition = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : oldData.DYPosition;//单元空间位置
                    //newData.StandardAddress = StandardAddress;
                    //newData.CreateTime = oldData.CreateTime;
                    //newData.CreateUser = oldData.CreateUser;
                    //newData.BZTime = oldData.BZTime;
                    //newData.LastModifyTime = DateTime.Now.Date;//修改时间
                    //newData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                    //newData.CancelTime = oldData.CancelTime;
                    //newData.CancelUser = oldData.CancelUser;
                    //newData.DelTime = newData.DelTime;
                    //newData.DelUser = newData.DelUser;
                    //newData.State = oldData.State;

                    var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<MPOfResidence>(oldDataJson);
                    var targetData = dbContext.MPOFResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                    if (targetData == null)
                        throw new Exception("该条数据已被注销，请重新查询并编辑！");
                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    if (!CheckResidenceMPIsAvailable(targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityID, targetData.ResidenceName, targetData.MPNumber, targetData.Dormitory, targetData.HSNumber, targetData.LZNumber, targetData.DYNumber))
                        throw new Exception("该户室牌已经存在，请检查后重新修改！");
                    //上传的附件进行修改
                    var AddedFCZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.FCZ);
                    var AddedTDZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.TDZ);
                    var AddedBDCZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.BDCZ);
                    var AddedHJFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.HJ);
                    UpdateMPFilesByID(FCZIDs, AddedFCZFiles, sourceData.ID, Enums.DocType.FCZ, Enums.MPTypeStr.ResidenceMP);
                    UpdateMPFilesByID(TDZIDs, AddedTDZFiles, sourceData.ID, Enums.DocType.TDZ, Enums.MPTypeStr.ResidenceMP);
                    UpdateMPFilesByID(BDCZIDs, AddedBDCZFiles, sourceData.ID, Enums.DocType.BDCZ, Enums.MPTypeStr.ResidenceMP);
                    UpdateMPFilesByID(HJIDs, AddedHJFiles, sourceData.ID, Enums.DocType.HJ, Enums.MPTypeStr.ResidenceMP);
                }
                #endregion
                dbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 上传住宅门牌数据，将错误和数据存到内存
        /// </summary>
        /// <param name="file"></param>
        public static void UploadResidenceMP(HttpPostedFileBase file)
        {
            Stream fs = file.InputStream;
            Workbook wb = new Workbook(fs);
            if (wb == null || wb.Worksheets.Count == 0)
                throw new Exception("上传文件不包含有效数据！");

            Worksheet ws = wb.Worksheets[0];
            int rowCount = ws.Cells.Rows.Count;
            int columnCount = ws.Cells.Columns.Count;
            if (columnCount < 14)
                throw new Exception("数据列不完整！");

            var mps = new List<ResidenceMPDetails>();
            var errors = new List<ResidenceMPErrors>();
            var warnings = new List<string>();

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                for (int i = 1; i < rowCount; i++)
                {
                    var error = new ResidenceMPErrors();
                    string warning = string.Empty;

                    Row row = ws.Cells.Rows[i];
                    var CountyName = row[0].Value != null ? row[0].Value.ToString().Trim() : null;
                    var NeighborhoodsName = row[1].Value != null ? row[1].Value.ToString().Trim() : null;
                    var CommunityName = row[2].Value != null ? row[2].Value.ToString().Trim() : null;
                    var Postcode = row[3].Value != null ? row[3].Value.ToString().Trim() : null;
                    var ResidenceName = row[4].Value != null ? row[4].Value.ToString().Trim() : null;
                    var MPNumber = row[5].Value != null ? row[5].Value.ToString().Replace(" ", "") : null;
                    var Dormitory = row[6].Value != null ? row[6].Value.ToString().Trim() : null;
                    var LZNumber = row[7].Value != null ? row[7].Value.ToString().Replace(" ", "") : null;
                    var DYNumber = row[8].Value != null ? row[8].Value.ToString().Replace(" ", "") : null;
                    var HSNumber = row[9].Value != null ? row[9].Value.ToString().Replace(" ", "") : null;
                    var PropertyOwner = row[10].Value != null ? row[10].Value.ToString().Trim() : null;
                    var Applicant = row[11].Value != null ? row[11].Value.ToString().Trim() : null;
                    var ApplicantPhone = row[12].Value != null ? row[12].Value.ToString().Trim() : null;
                    var SBDW = row[13].Value != null ? row[13].Value.ToString().Trim() : null;
                    var BZTime = row[14].Value != null ? row[14].Value.ToString().Trim() : null;

                    if (row.IsBlank)
                    {
                        warnings.Add($"第{i}行：空行");
                        continue;
                    }
                    string CountyID = null;
                    string NeighborhoodsID = null;
                    string CommunityID = null;
                    DateTime bzTime = DateTime.Now.Date;
                    #region 市辖区检查
                    if (string.IsNullOrEmpty(CountyName))
                    {
                        error.ErrorMessages.Add("市辖区名称为空");
                    }
                    else
                    {
                        CountyID = SystemUtils.Districts.Where(t => t.Name.Contains(CountyName)).Select(t => t.ID).FirstOrDefault();
                        if (CountyID == null)
                            error.ErrorMessages.Add("市辖区名称有误");
                    }
                    #endregion
                    #region 镇街道检查
                    if (string.IsNullOrEmpty(NeighborhoodsName))
                    {
                        error.ErrorMessages.Add("镇街道名称为空");
                    }
                    else
                    {
                        NeighborhoodsID = SystemUtils.Districts.Where(t => t.Name.Contains(NeighborhoodsName)).Select(t => t.ID).FirstOrDefault();
                        if (NeighborhoodsID == null)
                            error.ErrorMessages.Add("镇街道名称有误");
                    }
                    #endregion
                    #region 村社区检查
                    if (string.IsNullOrEmpty(CommunityName))
                    {
                        error.ErrorMessages.Add("村社区名称为空");
                    }
                    else
                    {
                        CommunityID = SystemUtils.Districts.Where(t => t.Name.Contains(CommunityName)).Select(t => t.ID).FirstOrDefault();
                        if (CommunityID == null)
                            error.ErrorMessages.Add("村社区名称有误");
                    }
                    #endregion
                    #region 邮政编码检查 6位数，且以3140开头  可以为空
                    if (!string.IsNullOrEmpty(Postcode))
                    {
                        if (!CheckPostcode(Postcode))
                            error.ErrorMessages.Add("邮政编码有误");
                    }
                    #endregion
                    #region 小区名称和道路名称检查，两个不能同时为空，无小区名有道路名时，必须有门牌号，有小区名时可以有道路名，但不检查有无门牌号 已注释
                    //if (string.IsNullOrEmpty(ResidenceName))  //小区名为空
                    //{
                    //    if (string.IsNullOrEmpty(RoadName))  //小区名为空时道路名不能再为空
                    //    {
                    //        error.ErrorMessages.Add("小区名称和道路名称不能全为空");
                    //    }
                    //    else  //小区名为空道路名不为空
                    //    {
                    //        if (string.IsNullOrEmpty(MPNumber))  //道路名不为空时门牌号不能为空，宿舍名可以为空
                    //        {
                    //            error.ErrorMessages.Add("门牌号为空");
                    //        }
                    //        else
                    //        {
                    //            if (!CheckIsNumber(MPNumber))
                    //                error.ErrorMessages.Add("门牌号不是数字");
                    //        }
                    //        var Roads = dbContext.Road.Where(t => t.RoadName.Contains(RoadName)).Select(t => t.RoadID);
                    //        if (Roads.Count() == 0)
                    //            error.ErrorMessages.Add("不存在该道路");
                    //        if (Roads.Count() > 1)
                    //            warning = warning + $"道路表中道路名为【{RoadName}】的道路有【{Roads.Count()}】条;";
                    //        RoadID = Roads.FirstOrDefault().ToString();
                    //    }
                    //}
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(RoadName))
                    //    {
                    //        var Roads = dbContext.Road.Where(t => t.RoadName.Contains(RoadName)).Select(t => t.RoadID);
                    //        if (Roads.Count() == 0)
                    //            error.ErrorMessages.Add("不存在该道路");
                    //        if (Roads.Count() > 1)
                    //            warning = warning + $"道路表中道路名为【{RoadName}】的道路有【{Roads.Count()}】条;";
                    //        RoadID = Roads.FirstOrDefault().ToString();
                    //    }
                    //}
                    #endregion
                    #region 门牌号、楼幢号、单元号和户室号检查是否只含数字
                    if (!string.IsNullOrEmpty(MPNumber))
                    {
                        if (!CheckIsNumber(MPNumber))
                            error.ErrorMessages.Add("门牌号不是纯数字");
                    }
                    if (!string.IsNullOrEmpty(LZNumber))
                    {
                        if (!CheckIsNumber(LZNumber))
                            error.ErrorMessages.Add("楼幢号不是纯数字");
                    }

                    if (!string.IsNullOrEmpty(DYNumber))
                    {
                        if (!CheckIsNumber(DYNumber))
                            error.ErrorMessages.Add("单元号不是纯数字");
                    }
                    if (!string.IsNullOrEmpty(HSNumber))
                    {
                        if (!CheckIsNumber(HSNumber))
                            error.ErrorMessages.Add("户室号不是纯数字");
                    }
                    #endregion
                    #region 产权人检查 暂不检查，可以为空
                    //if (string.IsNullOrEmpty(PropertyOwner))
                    //{
                    //    error.ErrorMessages.Add("产权人姓名为空");
                    //}
                    #endregion
                    #region 申办人检查 暂不检查，可以为空
                    //if (string.IsNullOrEmpty(Applicant))
                    //{
                    //    error.ErrorMessages.Add("申办人为空");
                    //}
                    #endregion
                    #region 申办人联系电话的格式检查 可以为空
                    if (!string.IsNullOrEmpty(ApplicantPhone))
                    {
                        if (!CheckIsPhone(ApplicantPhone))
                            error.ErrorMessages.Add("申办人联系电话不是正确的号码格式！");
                    }
                    #endregion
                    #region 户室牌查重
                    if (!CheckResidenceMPIsAvailable(CountyID, NeighborhoodsID, CommunityID, ResidenceName, MPNumber, Dormitory, HSNumber, LZNumber, DYNumber))
                    {
                        error.ErrorMessages.Add("该户室牌已经存在");
                    }
                    #endregion
                    #region 编制日期的格式检查
                    if (!string.IsNullOrEmpty(BZTime))
                    {
                        if (CheckIsDatetime(ref BZTime))
                            bzTime = Convert.ToDateTime(BZTime);
                        else
                            error.ErrorMessages.Add("编制日期不是标准日期格式");
                    }
                    #endregion
                    var mp = new Models.Extends.ResidenceMPDetails
                    {
                        CountyID = CountyID,
                        CountyName = CountyName,
                        NeighborhoodsID = NeighborhoodsID,
                        NeighborhoodsName = NeighborhoodsName,
                        CommunityID = CommunityID,
                        CommunityName = CommunityName,
                        ResidenceName = ResidenceName,
                        MPNumber = MPNumber,
                        Dormitory = Dormitory,
                        LZNumber = LZNumber,
                        DYNumber = DYNumber,
                        HSNumber = HSNumber,
                        Postcode = Postcode,
                        PropertyOwner = PropertyOwner,
                        Applicant = Applicant,
                        ApplicantPhone = ApplicantPhone,
                        SBDW = SBDW,
                        BZTime = bzTime
                    };

                    // 将这个门牌加到List中
                    mps.Add(mp);
                    // 判断是否有错误，如果有将错误的行数、错误的门牌给这个error实体
                    if (error.ErrorMessages.Count > 0)
                    {
                        error.Index = i;
                        error.Title = string.Format("第{0}行有误", i);
                        error.mp = mp;
                        errors.Add(error);
                    }
                    // 判断是否有警告，如果有就存到内存
                    if (!string.IsNullOrEmpty(warning))
                    {
                        warnings.Add($"第{i}行：" + warning);
                    }
                }
                #region 自身导入的数据的重复检查   p.RoadName,
                var selfCount = (from p in mps
                                 group p by new { p.CountyName, p.NeighborhoodsName, p.CommunityName, p.ResidenceName, p.MPNumber, p.Dormitory, p.LZNumber, p.DYNumber, p.HSNumber }
                                 into g
                                 where g.Count() > 1
                                 select g).ToList();
                if (selfCount.Count > 0)
                {
                    var error = new ResidenceMPErrors();
                    error.Title = "导入的数据中有重复";
                    foreach (var group in selfCount)
                    {
                        var r = group.Key.ResidenceName == null ? string.Empty : group.Key.ResidenceName;
                        //var ro = group.Key.RoadName == null ? string.Empty : group.Key.RoadName;
                        var m = group.Key.MPNumber == null ? string.Empty : group.Key.MPNumber + "号";
                        var d = group.Key.Dormitory == null ? string.Empty : group.Key.Dormitory;
                        var l = group.Key.LZNumber == null ? string.Empty : group.Key.LZNumber + "幢";
                        var dy = group.Key.DYNumber == null ? string.Empty : group.Key.DYNumber + "单元";
                        var h = group.Key.HSNumber == null ? string.Empty : group.Key.HSNumber + "室";
                        var erMsg = $"{group.Key.CountyName} {group.Key.NeighborhoodsName} {group.Key.CommunityName} {r} {m} {d} {l} {dy} {h}";
                        erMsg = string.IsNullOrEmpty(erMsg) ? $"存在多条空行" : erMsg + " 号";
                        error.ErrorMessages.Add(erMsg);
                    }
                    errors.Add(error);
                }
                #endregion

                Dictionary<string, object> D = new Dictionary<string, object>();
                D.Add(mpKey, mps);
                D.Add(errorKey, errors);
                D.Add(warningKey, warnings);
                temp[LoginUtils.CurrentUser.UserName] = D;
            }
        }
        /// <summary>
        /// 获取所有上传的数据和错误
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetUploadResidenceMP(int PageSize, int PageNum)
        {
            List<ResidenceMPDetails> rows = null;
            int totalCount = 0;
            var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<ResidenceMPDetails>;
            var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<ResidenceMPErrors>;
            var warnings = temp[LoginUtils.CurrentUser.UserName][warningKey] as List<string>;
            if (mps != null)
            {
                rows = mps.Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                totalCount = mps.Count;
            }
            return new Dictionary<string, object> {
                { "Data",rows},
                { "Count",totalCount},
                { "Errors",errors},
                { "Warnings",warnings}
            };
        }
        /// <summary>
        /// 将没有错误的数据更新到数据库 并进行批量门牌制作 返回制作汇总表
        /// </summary>
        public static SummarySheet UpdateResidenceMP()
        {
            var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<ResidenceMPDetails>;
            var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<ResidenceMPErrors>;
            if (errors.Count > 0)
                throw new Exception("数据包含错误信息，请先检查数据！");
            if (mps.Count() == 0 || mps == null)
                throw new Exception("无可导入数据！");
            #region****************没有错误后将导入的数据更新到住宅门牌数据库中
            foreach (var mp in mps)
            {
                MPOfResidence m = new MPOfResidence
                {
                    CountyID = mp.CountyID,
                    NeighborhoodsID = mp.NeighborhoodsID,
                    CommunityID = mp.CommunityID,
                    ResidenceName = mp.ResidenceName,
                    MPNumber = mp.MPNumber,
                    Dormitory = mp.Dormitory,
                    LZNumber = mp.LZNumber,
                    DYNumber = mp.DYNumber,
                    HSNumber = mp.HSNumber,
                    PropertyOwner = mp.PropertyOwner,
                    Applicant = mp.Applicant,
                    ApplicantPhone = mp.ApplicantPhone,
                    Postcode = mp.Postcode,
                    SBDW = mp.SBDW,
                    BZTime = mp.BZTime
                };
                ModifyResidenceMP(m, null, null, null, null, null);
            }
            temp.Remove(LoginUtils.CurrentUser.UserName);
            #endregion
            #region*************批量导入成功后立刻进行门牌制作，更新到门牌制作表中，再给出门牌制作汇总表********************
            var Residence = (from t in mps
                             group t by new
                             {
                                 t.CountyID,
                                 t.NeighborhoodsID,
                                 t.CommunityID,
                                 t.ResidenceName
                             } into g
                             select new Record
                             {
                                 CountyID = g.Key.CountyID,
                                 NeighborhoodsID = g.Key.NeighborhoodsID,
                                 CommunityID = g.Key.CommunityID,
                                 Name = g.Key.ResidenceName,
                                 Type = "小区",
                                 Count = 0
                             }).ToList();
            var LZMP = (from t in mps
                        group t by new
                        {
                            t.CountyID,
                            t.NeighborhoodsID,
                            t.CommunityID,
                            t.ResidenceName,
                            t.LZNumber
                        } into g
                        select new Record
                        {
                            CountyID = g.Key.CountyID,
                            NeighborhoodsID = g.Key.NeighborhoodsID,
                            CommunityID = g.Key.CommunityID,
                            Name = g.Key.ResidenceName,
                            Type = g.Key.LZNumber,
                            Count = 2
                        }).OrderBy(t => t.Type).ToList();

            LZMP.Add((from t in LZMP
                      group t by new
                      {
                          t.CountyID,
                          t.NeighborhoodsID,
                          t.CommunityID,
                          t.Name,
                      } into g
                      select new Record
                      {
                          CountyID = g.Key.CountyID,
                          NeighborhoodsID = g.Key.NeighborhoodsID,
                          CommunityID = g.Key.CommunityID,
                          Name = g.Key.Name,
                          Type = "楼幢牌合计",
                          Count = g.Sum(t => t.Count)
                      }).First());
            var dyDis = (from t in mps
                         select new ResidenceMPDetails
                         {
                             CountyID = t.CountyID,
                             NeighborhoodsID = t.NeighborhoodsID,
                             CommunityID = t.CommunityID,
                             ResidenceName = t.ResidenceName,
                             LZNumber = t.LZNumber,
                             DYNumber = t.DYNumber
                         }).Distinct();
            var DYMP = (from t in dyDis
                        group t by new
                        {
                            t.CountyID,
                            t.NeighborhoodsID,
                            t.CommunityID,
                            t.ResidenceName,
                            t.DYNumber
                        } into g
                        select new Record
                        {
                            CountyID = g.Key.CountyID,
                            NeighborhoodsID = g.Key.NeighborhoodsID,
                            CommunityID = g.Key.CommunityID,
                            Name = g.Key.ResidenceName,
                            Type = g.Key.DYNumber,
                            Count = g.Count()
                        }).OrderBy(t => t.Type).ToList();
            DYMP.Add((from t in DYMP
                      group t by new
                      {
                          t.CountyID,
                          t.NeighborhoodsID,
                          t.CommunityID,
                          t.Name,
                      } into g
                      select new Record
                      {
                          CountyID = g.Key.CountyID,
                          NeighborhoodsID = g.Key.NeighborhoodsID,
                          CommunityID = g.Key.CommunityID,
                          Name = g.Key.Name,
                          Type = "单元牌合计",
                          Count = g.Sum(t => t.Count)
                      }).First());

            var HSMP = (from t in mps
                        group t by new
                        {
                            t.CountyID,
                            t.NeighborhoodsID,
                            t.CommunityID,
                            t.ResidenceName,
                            t.HSNumber
                        } into g
                        select new Record
                        {
                            CountyID = g.Key.CountyID,
                            NeighborhoodsID = g.Key.NeighborhoodsID,
                            CommunityID = g.Key.CommunityID,
                            Name = g.Key.ResidenceName,
                            Type = g.Key.HSNumber,
                            Count = g.Count()
                        }).OrderBy(t => t.Type).ToList();

            HSMP.Add((from t in HSMP
                      group t by new
                      {
                          t.CountyID,
                          t.NeighborhoodsID,
                          t.CommunityID,
                          t.Name,
                      } into g
                      select new Record
                      {
                          CountyID = g.Key.CountyID,
                          NeighborhoodsID = g.Key.NeighborhoodsID,
                          CommunityID = g.Key.CommunityID,
                          Name = g.Key.Name,
                          Type = "户室牌合计",
                          Count = g.Sum(t => t.Count)
                      }).First());
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<Models.Entities.MPProduce> mpPros = new List<Models.Entities.MPProduce>();
                foreach (var residence in Residence)
                {
                    Models.Entities.MPProduce mpPro = new Models.Entities.MPProduce();
                    mpPro.ID = Guid.NewGuid().ToString();
                    mpPro.CommunityID = residence.CommunityID;
                    mpPro.NeighborhoodsID = residence.NeighborhoodsID;
                    mpPro.CountyID = residence.CountyID;
                    mpPro.MPType = Enums.MPType.Residence;
                    mpPro.CreateTime = DateTime.Now.Date;
                    mpPro.CreateUser = LoginUtils.CurrentUser.UserName;
                    mpPro.LZMPCount = LZMP.Where(t => t.CountyID == residence.CountyID).Where(t => t.NeighborhoodsID == residence.NeighborhoodsID).Where(t => t.CommunityID == residence.CommunityID).Where(t => t.Name == residence.Name).Where(t => t.Type == "楼幢牌合计").Select(t => t.Count).First();
                    mpPro.DYMPCount = DYMP.Where(t => t.CountyID == residence.CountyID).Where(t => t.NeighborhoodsID == residence.NeighborhoodsID).Where(t => t.CommunityID == residence.CommunityID).Where(t => t.Name == residence.Name).Where(t => t.Type == "单元牌合计").Select(t => t.Count).First();
                    mpPro.HSMPCount = HSMP.Where(t => t.CountyID == residence.CountyID).Where(t => t.NeighborhoodsID == residence.NeighborhoodsID).Where(t => t.CommunityID == residence.CommunityID).Where(t => t.Name == residence.Name).Where(t => t.Type == "户室牌合计").Select(t => t.Count).First();
                    mpPro.TotalCount = mpPro.LZMPCount + mpPro.DYMPCount + mpPro.HSMPCount;
                    mpPros.Add(mpPro);
                }
                dbContext.MPProduce.AddRange(mpPros);
                dbContext.SaveChanges();
            }
            #endregion
            SummarySheet data = new SummarySheet();
            data.StandardName = Residence;
            data.LZMP = LZMP;
            data.DYMP = DYMP;
            data.HSMP = HSMP;
            return data;
        }
        /// <summary>
        /// 住宅门牌注销，只修改数据的state、注销时间和注销人，其它不变
        /// </summary>
        /// <param name="Data"></param>
        public static void CancelOrDelResidenceMP(string ID, int UseState)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                IQueryable<MPOfResidence> query = dbContext.MPOFResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID);
                int count = query.Count();
                if (count == 0)
                    throw new Exception("该条数据已被注销，请重新查询！");

                var CommunityID = query.Select(t => t.CommunityID).FirstOrDefault();
                if (!DistrictUtils.CheckPermission(CommunityID))
                    throw new Exception("无权修改其他镇街数据！");

                var user = LoginUtils.CurrentUser;
                var sql = $@"update [TopSystemDB].[dbo].[MPOFRESIDENCE] set [MPOFRESIDENCE].State={UseState},[MPOFRESIDENCE].[CancelTime]=GETDATE(),[MPOFRESIDENCE].[CancelUser]='{user.UserName}' where [MPOFRESIDENCE].ID='{ID}'";
                var rt = dbContext.Database.ExecuteSqlCommand(sql);
                if (rt == 0)
                    throw new Exception("数据注销失败，请重试！");
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
        public static bool CheckResidenceMPIsAvailable(string CountyID, string NeighborhoodsID, string CommunityID, string ResidenceName, string MPNumber, string Dormitory, string HSNumber, string LZNumber, string DYNumber)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var q = dbContext.MPOFResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityID == CommunityID);
                var count = q.Where(t => t.ResidenceName == ResidenceName).Where(t => t.MPNumber == MPNumber).Where(t => t.Dormitory == Dormitory).Where(t => t.LZNumber == LZNumber).Where(t => t.DYNumber == DYNumber).Where(t => t.HSNumber == HSNumber).Count();
                return count == 0;
            }

        }
        #endregion

        #region 道路门牌
        /// <summary>
        /// 一条数据的新增或者修改
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        public static void ModifyRoadMP(MPOfRoad newData, string oldDataJson, List<string> FCZIDs, List<string> TDZIDs, List<string> YYZZIDs)
        {
            #region 权限检查
            if (!DistrictUtils.CheckPermission(newData.CommunityID))
                throw new Exception("无权操作其他镇街数据！");
            #endregion
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                #region 新增
                //新增，则需对门牌号查重
                if (string.IsNullOrEmpty(oldDataJson))
                {
                    #region 基本检查
                    if (string.IsNullOrEmpty(newData.RoadName))
                        throw new Exception("道路名称不能为空！");

                    if (string.IsNullOrEmpty(newData.MPNumber))
                    {
                        throw new Exception("门牌号为空！");
                    }
                    else
                    {
                        newData.MPNumber = newData.MPNumber.Replace(" ", "");
                        if (!CheckIsNumber(newData.MPNumber))
                            throw new Exception("门牌号不是数字！");
                    }

                    if (!CheckRoadMPIsAvailable(newData.MPNumber, newData.RoadName, newData.CountyID, newData.NeighborhoodsID, newData.CommunityID))
                    {
                        throw new Exception("该门牌号已经存在，请检查后重新输入！");
                    }

                    if (!string.IsNullOrEmpty(newData.ApplicantPhone))
                    {
                        if (!CheckIsPhone(newData.ApplicantPhone))
                            throw new Exception("申办人联系电话不是正确的号码格式！");
                    }

                    if (newData.MPProduce == null) //是否制作门牌不能为空
                    {
                        throw new Exception("是否制作门牌为空！");
                    }
                    else
                    {
                        if (newData.MPProduce == Enums.MPProduce.ToBeMade) //如果制作门牌
                        {
                            if (newData.MPMail == null) //门牌邮寄不能为空
                            {
                                throw new Exception("是否邮寄门牌为空！");
                            }
                            else if (newData.MPMail == Enums.MPMail.Yes)//如果门牌邮寄
                            {
                                if (!string.IsNullOrEmpty(newData.MailAddress))//必须填门牌邮寄的地址
                                    throw new Exception("门牌邮寄地址为空！");
                            }
                        }
                        else if (newData.MPProduce == Enums.MPProduce.NotMake) //如果不制作门牌
                        {
                            newData.MPMail = Enums.MPMail.No;   //不制作门牌时邮寄都设置为2
                        }
                    }
                    #endregion
                    #region 标准地址拼接
                    //拼接标准住宅门牌地址，先获取对应的道路门牌的标准地址，再拼接好宿舍名、幢号、单元号、户室号
                    var CountyName = SystemUtils.Districts.Where(t => t.ID == newData.CountyID).Select(t => t.Name).FirstOrDefault();
                    var NeighborhoodsName = SystemUtils.Districts.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Name).FirstOrDefault();
                    var CommunityName = SystemUtils.Districts.Where(t => t.ID == newData.CommunityID).Select(t => t.Name).FirstOrDefault();
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.RoadName + newData.MPNumber + "号";
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = SystemUtils.Districts.Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = SystemUtils.Districts.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Road.Value.ToString();
                    var year = DateTime.Now.Year.ToString();
                    var AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
                    #endregion
                    var guid = Guid.NewGuid().ToString();
                    var MPPosition = (newData.Lng != 0 && newData.Lat != 0) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    var CreateTime = DateTime.Now.Date;
                    //获取所有上传的文件
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        var FCZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.FCZ);
                        var TDZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.TDZ);
                        var YYZZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.YYZZ);
                        SaveMPFilesByID(FCZFiles, guid, Enums.DocType.FCZ, Enums.MPTypeStr.RoadMP);
                        SaveMPFilesByID(TDZFiles, guid, Enums.DocType.TDZ, Enums.MPTypeStr.RoadMP);
                        SaveMPFilesByID(YYZZFiles, guid, Enums.DocType.YYZZ, Enums.MPTypeStr.RoadMP);
                    }
                    //对这条数据进行默认赋值
                    newData.ID = guid;
                    newData.AddressCoding = AddressCoding;
                    newData.MPPosition = MPPosition;
                    newData.StandardAddress = StandardAddress;
                    newData.State = Enums.UseState.Enable;
                    newData.MPMail = newData.MPMail == null ? Enums.MPMail.No : newData.MPMail;
                    newData.CreateTime = CreateTime;
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
                    if (!CheckRoadMPIsAvailable(targetData.MPNumber, targetData.RoadName, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityID))
                        throw new Exception("该道路门牌已经存在，请检查后重新修改！");
                    //上传的附件进行修改 ？？？？？？？？？？待完成
                    var AddedFCZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.FCZ);
                    var AddedTDZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.TDZ);
                    var AddedYYZZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.YYZZ);
                    UpdateMPFilesByID(FCZIDs, AddedFCZFiles, targetData.ID, Enums.DocType.FCZ, Enums.MPTypeStr.RoadMP);
                    UpdateMPFilesByID(TDZIDs, AddedTDZFiles, targetData.ID, Enums.DocType.TDZ, Enums.MPTypeStr.RoadMP);
                    UpdateMPFilesByID(YYZZIDs, AddedYYZZFiles, targetData.ID, Enums.DocType.YYZZ, Enums.MPTypeStr.RoadMP);
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
            //HttpContext.Current.Session["_RoadMP"] = null;
            //HttpContext.Current.Session["_RoadMPErrors"] = null;
            //HttpContext.Current.Session["_RoadMPWarning"] = null;

            Stream fs = file.InputStream;
            Workbook wb = new Workbook(fs);
            if (wb == null || wb.Worksheets.Count == 0)
                throw new Exception("上传文件不包含有效数据！");

            Worksheet ws = wb.Worksheets[0];
            //ws.Cells.DeleteBlankRows();
            //ws.Cells.DeleteBlankColumns();
            int rowCount = ws.Cells.Rows.Count;
            int columnCount = ws.Cells.Columns.Count;

            if (columnCount < 19)
                throw new Exception("数据列不完整！");

            var mps = new List<RoadMPDetails>();
            var errors = new List<RoadMPErrors>();
            var warnings = new List<string>();

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                for (int i = 1; i < rowCount; i++)
                {
                    var error = new RoadMPErrors();
                    string warning = string.Empty;

                    Row row = ws.Cells.Rows[i];
                    var CountyName = row[0].Value != null ? row[0].Value.ToString().Trim() : null;
                    var NeighborhoodsName = row[1].Value != null ? row[1].Value.ToString().Trim() : null;
                    var CommunityName = row[2].Value != null ? row[2].Value.ToString().Trim() : null;
                    var RoadName = row[3].Value != null ? row[3].Value.ToString().Trim() : null;
                    var StartPoint = row[4].Value != null ? row[4].Value.ToString().Trim() : null;
                    var EndPoint = row[5].Value != null ? row[5].Value.ToString().Trim() : null;
                    var MPRules = row[6].Value != null ? row[6].Value.ToString().Trim() : null;
                    var MPNumberRange = row[7].Value != null ? row[7].Value.ToString().Trim() : null;
                    var MPNumber = row[8].Value != null ? row[8].Value.ToString().Trim() : null;
                    var ReservedNumber = row[9].Value != null ? row[9].Value.ToString().Trim() : null;
                    var OriginalNumber = row[10].Value != null ? row[10].Value.ToString().Trim() : null;
                    var ShopName = row[11].Value != null ? row[11].Value.ToString().Trim() : null;
                    var ResidenceName = row[12].Value != null ? row[12].Value.ToString().Trim() : null;
                    var PropertyOwner = row[13].Value != null ? row[13].Value.ToString().Trim() : null;
                    var BZTime = row[14].Value != null ? row[14].Value.ToString().Trim() : null;
                    var MPSize = row[15].Value != null ? row[15].Value.ToString().Trim() : null;
                    var Applicant = row[16].Value != null ? row[16].Value.ToString().Trim() : null;
                    var ApplicantPhone = row[17].Value != null ? row[17].Value.ToString().Trim() : null;
                    //var MPProduce = row[18].Value != null ? row[18].Value.ToString().Trim() : null;
                    var Postcode = row[18].Value != null ? row[18].Value.ToString().Trim() : null;
                    var SBDW = row[19].Value != null ? row[19].Value.ToString().Trim() : null;

                    if (row.IsBlank)
                    {
                        warnings.Add($"第{i}行：空行");
                        continue;
                    }
                    string CountyID = null;
                    string NeighborhoodsID = null;
                    string CommunityID = null;
                    string RoadID = null;
                    DateTime bzTime = DateTime.Now.Date;
                    int mpProduce = Enums.MPProduce.HasBeenMade;

                    #region 市辖区检查
                    if (string.IsNullOrEmpty(CountyName))
                    {
                        error.ErrorMessages.Add("市辖区名称为空");
                    }
                    else
                    {
                        CountyID = SystemUtils.Districts.Where(t => t.Name.Contains(CountyName)).Select(t => t.ID).FirstOrDefault();
                        if (CountyID == null)
                            error.ErrorMessages.Add("市辖区名称有误");
                    }
                    #endregion
                    #region 镇街道检查
                    if (string.IsNullOrEmpty(NeighborhoodsName))
                    {
                        error.ErrorMessages.Add("镇街道名称为空");
                    }
                    else
                    {
                        NeighborhoodsID = SystemUtils.Districts.Where(t => t.Name.Contains(NeighborhoodsName)).Select(t => t.ID).FirstOrDefault();
                        if (NeighborhoodsID == null)
                            error.ErrorMessages.Add("镇街道名称有误");
                    }
                    #endregion
                    #region 村社区检查
                    if (string.IsNullOrEmpty(CommunityName))
                    {
                        error.ErrorMessages.Add("村社区名称为空");
                    }
                    else
                    {
                        CommunityID = SystemUtils.Districts.Where(t => t.Name.Contains(CommunityName)).Select(t => t.ID).FirstOrDefault();
                        if (CommunityID == null)
                            error.ErrorMessages.Add("村社区名称有误");
                    }
                    #endregion
                    #region 道路检查 目前不检查 道路可以选择也可以输入
                    //if (string.IsNullOrEmpty(RoadName)) //说明道路名称为空
                    //{
                    //    error.ErrorMessages.Add("道路名称为空");
                    //}
                    //else  //说明道路名称不为空
                    //{
                    //    var Roads = dbContext.Road.Where(t => t.RoadName.Contains(RoadName)); //通过道路名称去道路表中查找
                    //    if (Roads.Count() == 0) //说明只通过道路名称未找到道路
                    //    {
                    //        error.ErrorMessages.Add("不存在该道路");
                    //    }
                    //    else //说明只通过道路名称找到了道路
                    //    {
                    //        if (Roads.Count() == 1) //说明只通过道路名称只能找到一条道路
                    //        {
                    //            RoadID = Roads.Select(t => t.RoadID).FirstOrDefault().ToString();
                    //            var StartPointInDB = Roads.Select(t => t.StartPoint).FirstOrDefault().ToString();
                    //            var EndPointInDB = Roads.Select(t => t.EndPoint).FirstOrDefault().ToString();
                    //            if (StartPointInDB != StartPoint)
                    //            {
                    //                StartPointInDB = StartPointInDB == null ? "空" : StartPointInDB;
                    //                warning = warning + $"道路表中道路名为【{RoadName}】的起点为:{StartPointInDB};";
                    //            }
                    //            if (EndPointInDB != EndPoint)
                    //            {
                    //                EndPointInDB = EndPointInDB == null ? "空" : EndPointInDB;
                    //                warning = warning + $"道路表中道路名为【{RoadName}】的讫点为:{EndPointInDB};";
                    //            }
                    //        }
                    //        else //说明只通过道路名称能找到一条以上道路
                    //        {
                    //            var roads = Roads.Where(t => t.StartPoint == StartPoint).Where(t => t.EndPoint == EndPoint);

                    //            if (roads.Count() == 0) //说明只通过道路名称能找到一条以上道路，但是结合起点和止点后找不到道路，那就找其中一条赋值
                    //            {
                    //                RoadID = Roads.Select(t => t.RoadID).FirstOrDefault().ToString();
                    //                warning = warning + $"道路表中道路名为【{RoadName}】的道路有【{Roads.Count()}】条，但起点和讫点不是：【{StartPoint}】和【{EndPoint}】；";
                    //            }
                    //            else if (roads.Count() == 1) //说明只通过道路名称能找到一条以上道路，结合起点和止点后能找到一条道路
                    //            {
                    //                RoadID = roads.Select(t => t.RoadID).FirstOrDefault().ToString();
                    //            }
                    //            else //说明只通过道路名称能找到一条以上道路，结合起点和止点后还能找到一条以上道路
                    //            {
                    //                RoadID = roads.Select(t => t.RoadID).FirstOrDefault().ToString();
                    //                warning = warning + $"道路表中起点为【{StartPoint}】、讫点为【{EndPoint}】、名称为【{RoadName}】的道路有【{roads.Count()}】条；";
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region 道路门牌号检查及查重
                    if (string.IsNullOrEmpty(MPNumber)) //门牌号为空
                    {
                        error.ErrorMessages.Add("门牌号为空");
                    }
                    else  //门牌号不为空
                    {
                        if (!CheckIsNumber(MPNumber))  //门牌号不是数字
                            error.ErrorMessages.Add("门牌号不是纯数字");
                        if (!CheckRoadMPIsAvailable(MPNumber, RoadID, CountyID, NeighborhoodsID, CommunityID))  //门牌号已经存在
                            error.ErrorMessages.Add("门牌号已经存在");
                    }

                    #endregion
                    #region 编制日期的格式检查  不填默认是当前时间
                    if (!string.IsNullOrEmpty(BZTime))
                    {
                        if (CheckIsDatetime(ref BZTime))
                            bzTime = Convert.ToDateTime(BZTime);
                        else
                            error.ErrorMessages.Add("编制日期不是标准日期格式");
                    }
                    #endregion
                    #region 门牌规格格式检查
                    if (string.IsNullOrEmpty(MPSize))
                    {
                        error.ErrorMessages.Add("门牌规格为空");
                    }
                    else
                    {
                        if (!CheckMPSize(ref MPSize))
                            error.ErrorMessages.Add("门牌规格不在规定范围内");
                    }
                    #endregion
                    #region 申办人联系电话格式检查
                    if (!string.IsNullOrEmpty(ApplicantPhone))
                    {
                        if (!CheckIsPhone(ApplicantPhone))
                            error.ErrorMessages.Add("申办人联系电话不是正确的号码格式！");

                    }
                    #endregion
                    #region 门牌制作检查  强制全部制作
                    //if (string.IsNullOrEmpty(MPProduce))
                    //{
                    //    error.ErrorMessages.Add("是否制作门牌为空！");
                    //}
                    //else
                    //{
                    //    if (!CheckMPProdece(MPProduce, ref mpProduce))
                    //        error.ErrorMessages.Add("是否制作门牌列格式不正确，只能填“是”或者“否”！");
                    //}
                    #endregion
                    #region 邮政编码检查 6位数，且以3140开头  可以为空
                    if (!string.IsNullOrEmpty(Postcode))
                    {
                        if (!CheckPostcode(Postcode))
                            error.ErrorMessages.Add("邮政编码有误");
                    }
                    #endregion

                    var mp = new Models.Extends.RoadMPDetails
                    {
                        CountyID = CountyID,
                        CountyName = CountyName,
                        NeighborhoodsID = NeighborhoodsID,
                        NeighborhoodsName = NeighborhoodsName,
                        CommunityID = CommunityID,
                        CommunityName = CommunityName,
                        //RoadID = RoadID,
                        RoadName = RoadName,
                        RoadStart = StartPoint,
                        RoadEnd = EndPoint,
                        MPRules = MPRules,
                        MPNumberRange = MPNumberRange,
                        MPNumber = MPNumber,
                        ReservedNumber = ReservedNumber,
                        OriginalNumber = OriginalNumber,
                        ShopName = ShopName,
                        ResidenceName = ResidenceName,
                        PropertyOwner = PropertyOwner,
                        BZTime = bzTime,
                        MPSize = MPSize,
                        Applicant = Applicant,
                        ApplicantPhone = ApplicantPhone,
                        MPProduce = mpProduce,
                        Postcode = Postcode,
                        SBDW = SBDW
                    };
                    mps.Add(mp);
                    if (error.ErrorMessages.Count > 0)
                    {
                        error.Index = i;
                        error.Title = string.Format("第{0}行有误", i);
                        error.mp = mp;
                        errors.Add(error);
                    }
                    if (!string.IsNullOrEmpty(warning))
                    {
                        warnings.Add($"第{i}行：" + warning);
                    }
                }
                #region 自身导入的数据的重复检查
                var selfCount = (from p in mps
                                 group p by new { p.CountyName, p.NeighborhoodsName, p.CommunityName, p.RoadName, p.MPNumber } into g
                                 where g.Count() > 1
                                 select g).ToList();
                if (selfCount.Count > 0)
                {
                    var error = new RoadMPErrors();
                    error.Title = "导入的数据中有重复";
                    foreach (var group in selfCount)
                    {
                        var erMsg = $"{group.Key.CountyName} {group.Key.NeighborhoodsName} {group.Key.CommunityName} {group.Key.RoadName} {group.Key.MPNumber}";
                        erMsg = string.IsNullOrEmpty(erMsg) ? $"存在多条空行" : erMsg + " 号";
                        error.ErrorMessages.Add(erMsg);
                    }
                    errors.Add(error);
                }
                #endregion

                Dictionary<string, object> D = new Dictionary<string, object>();
                D.Add(mpKey, mps);
                D.Add(errorKey, errors);
                D.Add(warningKey, warnings);
                temp[LoginUtils.CurrentUser.UserName] = D;
                //HttpContext.Current.Session["_RoadMP"] = mps;
                //HttpContext.Current.Session["_RoadMPErrors"] = errors;
                //HttpContext.Current.Session["_RoadMPWarning"] = warnings;
            }
        }
        public static Dictionary<string, object> GetUploadRoadMP(int PageSize, int PageNum)
        {
            List<RoadMPDetails> rows = null;
            int totalCount = 0;
            if (temp[LoginUtils.CurrentUser.UserName][mpKey] != null)
            {
                rows = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<RoadMPDetails>).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                totalCount = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<RoadMPDetails>).Count;
            }
            return new Dictionary<string, object> {
                { "Data",rows},
                { "Count",totalCount},
                { "Errors",(temp[LoginUtils.CurrentUser.UserName][errorKey] as List<RoadMPErrors>)},
                { "Warnings",(temp[LoginUtils.CurrentUser.UserName][warningKey] as List<string>)}
            };
        }
        public static SummarySheet UpdateRoadMP()
        {
            var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<RoadMPDetails>;
            var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<RoadMPErrors>;
            if (errors.Count > 0)
                throw new Exception("数据包含错误信息，请先检查数据！");
            if (mps.Count() == 0 || mps == null)
                throw new Exception("无可导入数据！");
            #region****************没有错误后将导入的数据更新到住宅门牌数据库中
            foreach (var mp in mps)
            {
                MPOfRoad m = new MPOfRoad
                {
                    CountyID = mp.CountyID,
                    NeighborhoodsID = mp.NeighborhoodsID,
                    CommunityID = mp.CommunityID,
                    //RoadID = mp.RoadID,
                    RoadName = mp.RoadName,
                    RoadStart = mp.RoadStart,
                    RoadEnd = mp.RoadEnd,
                    MPRules = mp.MPRules,
                    MPNumberRange = mp.MPNumberRange,
                    MPNumber = mp.MPNumber,
                    ReservedNumber = mp.ReservedNumber,
                    OriginalNumber = mp.OriginalNumber,
                    ShopName = mp.ShopName,
                    ResidenceName = mp.ResidenceName,
                    PropertyOwner = mp.PropertyOwner,
                    CreateTime = mp.CreateTime,
                    MPSize = mp.MPSize,
                    Applicant = mp.Applicant,
                    ApplicantPhone = mp.ApplicantPhone,
                    MPProduce = mp.MPProduce
                };
                ModifyRoadMP(m, null, null, null, null);
            }
            temp.Remove(LoginUtils.CurrentUser.UserName);
            #endregion
            #region*************批量导入成功后立刻进行门牌制作，更新到门牌制作表中，再给出门牌制作汇总表********************
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var bigMPsize = dbContext.DMBZDic.Where(t => t.Type == "大门牌").Select(t => t.Size).ToList();
                var Road = (from t in mps
                            group t by new
                            {
                                t.CountyID,
                                t.NeighborhoodsID,
                                t.CommunityID,
                                t.RoadName
                            } into g
                            select new Record
                            {
                                CountyID = g.Key.CountyID,
                                NeighborhoodsID = g.Key.NeighborhoodsID,
                                CommunityID = g.Key.CommunityID,
                                Name = g.Key.RoadName,
                                Type = "道路",
                                Count = 0
                            }).ToList();
                var DLMP = (from t in mps
                            group t by new
                            {
                                t.CountyID,
                                t.NeighborhoodsID,
                                t.CommunityID,
                                t.RoadName,
                                t.MPNumber,
                                t.MPSize
                            } into g
                            select new Record
                            {
                                CountyID = g.Key.CountyID,
                                NeighborhoodsID = g.Key.NeighborhoodsID,
                                CommunityID = g.Key.CommunityID,
                                Name = g.Key.RoadName,
                                Type = g.Key.MPNumber,
                                Size = g.Key.MPSize,
                                Count = g.Count()
                            }).OrderBy(t => t.Type).ToList();

                DLMP.Add((from t in DLMP
                          group t by new
                          {
                              t.CountyID,
                              t.NeighborhoodsID,
                              t.CommunityID,
                              t.Name,
                          } into g
                          select new Record
                          {
                              CountyID = g.Key.CountyID,
                              NeighborhoodsID = g.Key.NeighborhoodsID,
                              CommunityID = g.Key.CommunityID,
                              Name = g.Key.Name,
                              Type = "道路门牌合计",
                              Count = g.Sum(t => t.Count)
                          }).First());

                List<Models.Entities.MPProduce> mpPros = new List<Models.Entities.MPProduce>();
                foreach (var road in Road)
                {
                    Models.Entities.MPProduce mpPro = new Models.Entities.MPProduce();
                    mpPro.ID = Guid.NewGuid().ToString();
                    mpPro.CommunityID = road.CommunityID;
                    mpPro.NeighborhoodsID = road.NeighborhoodsID;
                    mpPro.CountyID = road.CountyID;
                    mpPro.MPType = Enums.MPType.Road;
                    mpPro.CreateTime = DateTime.Now.Date;
                    mpPro.CreateUser = LoginUtils.CurrentUser.UserName;
                    mpPro.BigMPCount = DLMP.Where(t => t.CountyID == road.CountyID).Where(t => t.NeighborhoodsID == road.NeighborhoodsID).Where(t => t.CommunityID == road.CommunityID).Where(t => t.Name == road.Name).Where(t => bigMPsize.Contains(t.Size)).Count();
                    mpPro.TotalCount = DLMP.Where(t => t.CountyID == road.CountyID).Where(t => t.NeighborhoodsID == road.NeighborhoodsID).Where(t => t.CommunityID == road.CommunityID).Where(t => t.Name == road.Name).Where(t => t.Type == "道路门牌合计").Select(t => t.Count).First();
                    mpPro.SmallMPCount = mpPro.TotalCount - mpPro.BigMPCount;
                    mpPros.Add(mpPro);
                }
                dbContext.MPProduce.AddRange(mpPros);
                dbContext.SaveChanges();
                SummarySheet data = new SummarySheet();
                data.StandardName = Road;
                return data;
            }
            #endregion
            //HttpContext.Current.Session["_RoadMP"] = null;
            //HttpContext.Current.Session["_RoadMPErrors"] = null;
            //HttpContext.Current.Session["_RoadMPWarning"] = null;
        }
        public static void CancelOrDelRoadMP(string ID, int UseState)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                IQueryable<MPOfRoad> query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID);
                int count = query.Count();
                if (count == 0)
                    throw new Exception("该条数据已被注销，请重新查询！");

                var CommunityID = query.Select(t => t.CommunityID).FirstOrDefault();
                if (!DistrictUtils.CheckPermission(CommunityID))
                    throw new Exception("无权修改其他镇街数据！");

                var user = LoginUtils.CurrentUser;
                var sql = $@"update [TopSystemDB].[dbo].[MPOFROAD] set [MPOFROAD].State={UseState},[MPOFROAD].[CancelTime]=GETDATE(),[MPOFROAD].[CancelUser]='{user.UserName}' where [MPOFROAD].ID='{ID}'";
                var rt = dbContext.Database.ExecuteSqlCommand(sql);
                if (rt == 0)
                    throw new Exception("数据注销失败，请重试！");
            }
        }
        public static bool CheckRoadMPIsAvailable(string MPNumber, string RoadName, string CountyID, string NeighborhoodsID, string CommunityID)
        {
            var check = true;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityID == CommunityID).Where(t => t.RoadName == RoadName).Where(t => t.MPNumber == MPNumber);
                if (query.Count() > 0)
                {
                    check = false;
                }
            }
            return check;
        }
        #endregion

        #region 农村门牌
        public static void ModifyCountryMP(MPOfCountry newData, string oldDataJson, List<string> TDZIDs, List<string> QQZIDs)
        {
            #region 权限检查
            if (!DistrictUtils.CheckPermission(newData.CommunityID))
                throw new Exception("无权操作其他镇街数据！");
            #endregion
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                #region 新增
                if (string.IsNullOrEmpty(oldDataJson))
                {
                    #region 基本检查
                    if (string.IsNullOrEmpty(newData.ViligeName))
                    {
                        throw new Exception("自然村名称为空！");
                    }
                    else
                    {
                        newData.ViligeName = newData.ViligeName.Trim();
                    }

                    if (string.IsNullOrEmpty(newData.MPNumber))
                    {
                        throw new Exception("门牌号为空！");
                    }
                    else
                    {
                        newData.MPNumber = newData.MPNumber.Replace(" ", "");
                        if (!CheckIsNumber(newData.MPNumber))
                            throw new Exception("门牌号不是数字！");
                    }

                    if (!string.IsNullOrEmpty(newData.HSNumber))
                    {
                        newData.HSNumber = newData.HSNumber.Replace(" ", "");
                        if (!CheckIsNumber(newData.HSNumber))
                            throw new Exception("户室号不是数字！");
                    }

                    if (!CheckCountryMPIsAvailable(newData.ViligeName, newData.MPNumber, newData.HSNumber, newData.CountyID, newData.NeighborhoodsID, newData.CommunityID))
                    {
                        throw new Exception("该门牌号已经存在，请检查后重新输入！");
                    }

                    if (!string.IsNullOrEmpty(newData.ApplicantPhone))
                    {
                        if (!CheckIsPhone(newData.ApplicantPhone))
                            throw new Exception("申办人联系电话不是正确的号码格式！");
                    }

                    if (newData.MPProduce == null) //是否制作门牌不能为空
                    {
                        throw new Exception("是否制作门牌为空！");
                    }
                    else
                    {
                        if (newData.MPProduce == Enums.MPProduce.ToBeMade) //如果制作门牌
                        {
                            if (newData.MPMail == null) //门牌邮寄不能为空
                            {
                                throw new Exception("是否邮寄门牌为空！");
                            }
                            else if (newData.MPMail == Enums.MPMail.Yes)//如果门牌邮寄
                            {
                                if (!string.IsNullOrEmpty(newData.MailAddress))//必须填门牌邮寄的地址
                                    throw new Exception("门牌邮寄地址为空！");
                            }
                        }
                        else if (newData.MPProduce == Enums.MPProduce.NotMake) //如果不制作门牌
                        {
                            newData.MPMail = Enums.MPMail.No;   //不制作门牌时邮寄都设置为2
                        }
                    }
                    #endregion
                    #region 标准地址拼接
                    var CountyName = SystemUtils.Districts.Where(t => t.ID == newData.CountyID).Select(t => t.Name).FirstOrDefault();
                    var NeighborhoodsName = SystemUtils.Districts.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Name).FirstOrDefault();
                    var CommunityName = SystemUtils.Districts.Where(t => t.ID == newData.CommunityID).Select(t => t.Name).FirstOrDefault();
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.ViligeName + newData.MPNumber + "号" + newData.HSNumber == null ? string.Empty : newData.HSNumber + "室";
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = SystemUtils.Districts.Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = SystemUtils.Districts.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Country.Value.ToString();
                    var year = DateTime.Now.Year.ToString();
                    var AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
                    #endregion
                    var guid = Guid.NewGuid().ToString();
                    var MPPosition = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    var CreateTime = DateTime.Now.Date;
                    //获取所有上传的文件
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        var TDZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.TDZ);
                        var QQZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.QQZ);
                        SaveMPFilesByID(TDZFiles, guid, Enums.DocType.TDZ, Enums.MPTypeStr.CountryMP);
                        SaveMPFilesByID(QQZFiles, guid, Enums.DocType.QQZ, Enums.MPTypeStr.CountryMP);
                    }
                    //对这条数据进行默认赋值
                    newData.ID = guid;
                    newData.AddressCoding = AddressCoding;
                    newData.MPPosition = MPPosition;
                    newData.StandardAddress = StandardAddress;
                    newData.State = Enums.UseState.Enable;
                    newData.MPMail = newData.MPMail == null ? Enums.MPMail.No : newData.MPMail;
                    newData.CreateTime = CreateTime;
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
                    if (!CheckCountryMPIsAvailable(targetData.ViligeName, targetData.MPNumber, targetData.HSNumber, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityID))
                        throw new Exception("该户室牌已经存在，请检查后重新修改！");
                    //上传的附件进行修改 ？？？？？？？？？？待完成
                    var AddedTDZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.TDZ);
                    var AddedQQZFiles = System.Web.HttpContext.Current.Request.Files.GetMultiple(Enums.DocType.QQZ);
                    UpdateMPFilesByID(TDZIDs, AddedTDZFiles, targetData.ID, Enums.DocType.TDZ, Enums.MPTypeStr.CountryMP);
                    UpdateMPFilesByID(QQZIDs, AddedQQZFiles, targetData.ID, Enums.DocType.QQZ, Enums.MPTypeStr.CountryMP);
                }
                #endregion
                dbContext.SaveChanges();
            }
        }
        public static void UploadCountryMP(HttpPostedFileBase file)
        {
            //HttpContext.Current.Session["_CountryMP"] = null;
            //HttpContext.Current.Session["_CountryMPErrors"] = null;
            //HttpContext.Current.Session["_CountryMPWarning"] = null;

            Stream fs = file.InputStream;
            Workbook wb = new Workbook(fs);
            if (wb == null || wb.Worksheets.Count == 0)
                throw new Exception("上传文件不包含有效数据！");

            Worksheet ws = wb.Worksheets[0];
            int rowCount = ws.Cells.Rows.Count;
            int columnCount = ws.Cells.Columns.Count;

            if (columnCount < 13)
                throw new Exception("数据列不完整！");

            var mps = new List<CountryMPDetails>();
            var errors = new List<CountryMPErrors>();
            var warnings = new List<string>();

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                for (int i = 1; i < rowCount; i++)
                {
                    var error = new CountryMPErrors();
                    string warning = string.Empty;

                    Row row = ws.Cells.Rows[i];
                    var CountyName = row[0].Value != null ? row[0].Value.ToString().Trim() : null;
                    var NeighborhoodsName = row[1].Value != null ? row[1].Value.ToString().Trim() : null;
                    var CommunityName = row[2].Value != null ? row[2].Value.ToString().Trim() : null;
                    var ViligeName = row[3].Value != null ? row[3].Value.ToString().Trim() : null;
                    var MPNumber = row[4].Value != null ? row[4].Value.ToString().Trim() : null;
                    var OriginalNumber = row[5].Value != null ? row[5].Value.ToString().Trim() : null;
                    var HSNumber = row[6].Value != null ? row[6].Value.ToString().Trim() : null;
                    var PropertyOwner = row[7].Value != null ? row[7].Value.ToString().Trim() : null;
                    var BZTime = row[8].Value != null ? row[8].Value.ToString().Trim() : null;
                    var MPSize = row[9].Value != null ? row[9].Value.ToString().Trim() : null;
                    var Applicant = row[10].Value != null ? row[10].Value.ToString().Trim() : null;
                    var ApplicantPhone = row[11].Value != null ? row[11].Value.ToString().Trim() : null;
                    //var MPProduce = row[12].Value != null ? row[12].Value.ToString().Trim() : null;
                    var Postcode = row[12].Value != null ? row[12].Value.ToString().Trim() : null;
                    var SBDW = row[13].Value != null ? row[13].Value.ToString().Trim() : null;

                    if (row.IsBlank)
                    {
                        warnings.Add($"第{i}行：空行");
                        continue;
                    }
                    string CountyID = null;
                    string NeighborhoodsID = null;
                    string CommunityID = null;
                    DateTime bzTime = DateTime.Now.Date;
                    int mpProduce = Enums.MPProduce.HasBeenMade;

                    #region 市辖区检查
                    if (string.IsNullOrEmpty(CountyName))
                    {
                        error.ErrorMessages.Add("市辖区名称为空");
                    }
                    else
                    {
                        CountyID = SystemUtils.Districts.Where(t => t.Name.Contains(CountyName)).Select(t => t.ID).FirstOrDefault();
                        if (CountyID == null)
                            error.ErrorMessages.Add("市辖区名称有误");
                    }
                    #endregion
                    #region 镇街道检查
                    if (string.IsNullOrEmpty(NeighborhoodsName))
                    {
                        error.ErrorMessages.Add("镇街道名称为空");
                    }
                    else
                    {
                        NeighborhoodsID = SystemUtils.Districts.Where(t => t.Name.Contains(NeighborhoodsName)).Select(t => t.ID).FirstOrDefault();
                        if (NeighborhoodsID == null)
                            error.ErrorMessages.Add("镇街道名称有误");
                    }
                    #endregion
                    #region 村社区检查
                    if (string.IsNullOrEmpty(CommunityName))
                    {
                        error.ErrorMessages.Add("村社区名称为空");
                    }
                    else
                    {
                        CommunityID = SystemUtils.Districts.Where(t => t.Name.Contains(CommunityName)).Select(t => t.ID).FirstOrDefault();
                        if (CommunityID == null)
                            error.ErrorMessages.Add("村社区名称有误");
                    }
                    #endregion
                    #region 自然村名称检查
                    if (string.IsNullOrEmpty(ViligeName))
                    {
                        error.ErrorMessages.Add("自然村名称为空");
                    }
                    #endregion
                    #region 农村门牌号检查及查重
                    if (string.IsNullOrEmpty(MPNumber)) //门牌号为空
                    {
                        error.ErrorMessages.Add("门牌号为空");
                    }
                    else  //门牌号不为空
                    {
                        if (!CheckIsNumber(MPNumber))  //门牌号不是数字
                            error.ErrorMessages.Add("门牌号不是纯数字");
                        if (!CheckCountryMPIsAvailable(ViligeName, MPNumber, HSNumber, CountyID, NeighborhoodsID, CommunityID))  //门牌号已经存在
                            error.ErrorMessages.Add("门牌号已经存在");
                    }

                    #endregion
                    #region 户室牌检查
                    if (!string.IsNullOrEmpty(HSNumber))
                    {
                        if (!CheckIsNumber(HSNumber))
                            error.ErrorMessages.Add("户室号不是数字");
                    }
                    #endregion
                    #region 编制日期的格式检查  不填默认是当前时间
                    if (!string.IsNullOrEmpty(BZTime))
                    {
                        if (CheckIsDatetime(ref BZTime))
                            bzTime = Convert.ToDateTime(BZTime);
                        else
                            error.ErrorMessages.Add("编制日期不是标准日期格式");
                    }
                    #endregion
                    #region 门牌规格格式检查
                    if (string.IsNullOrEmpty(MPSize))
                    {
                        error.ErrorMessages.Add("门牌规格为空");
                    }
                    else
                    {
                        if (!CheckMPSize(ref MPSize))
                            error.ErrorMessages.Add("门牌规格不在规定范围内");
                    }
                    #endregion
                    #region 申办人联系电话格式检查
                    if (!string.IsNullOrEmpty(ApplicantPhone))
                    {
                        if (!CheckIsPhone(ApplicantPhone))
                            error.ErrorMessages.Add("申办人联系电话不是正确的号码格式！");

                    }
                    #endregion
                    #region 门牌制作检查 强制全部制作
                    //if (string.IsNullOrEmpty(MPProduce))
                    //{
                    //    error.ErrorMessages.Add("是否制作门牌为空！");
                    //}
                    //else
                    //{
                    //    if (!CheckMPProdece(MPProduce, ref mpProduce))
                    //        error.ErrorMessages.Add("是否制作门牌列格式不正确，只能填“是”或者“否”！");
                    //}
                    #endregion
                    #region 邮政编码检查 6位数，且以3140开头  可以为空
                    if (!string.IsNullOrEmpty(Postcode))
                    {
                        if (!CheckPostcode(Postcode))
                            error.ErrorMessages.Add("邮政编码有误");
                    }
                    #endregion

                    var mp = new Models.Extends.CountryMPDetails
                    {
                        CountyID = CountyID,
                        CountyName = CountyName,
                        NeighborhoodsID = NeighborhoodsID,
                        NeighborhoodsName = NeighborhoodsName,
                        CommunityID = CommunityID,
                        CommunityName = CommunityName,
                        ViligeName = ViligeName,
                        MPNumber = MPNumber,
                        OriginalNumber = OriginalNumber,
                        HSNumber = HSNumber,
                        PropertyOwner = PropertyOwner,
                        BZTime = bzTime,
                        MPSize = MPSize,
                        Applicant = Applicant,
                        ApplicantPhone = ApplicantPhone,
                        MPProduce = mpProduce,
                        Postcode = Postcode,
                        SBDW = SBDW
                    };
                    mps.Add(mp);
                    if (error.ErrorMessages.Count > 0)
                    {
                        error.Index = i;
                        error.Title = string.Format("第{0}行有误", i);
                        error.mp = mp;
                        errors.Add(error);
                    }
                    if (!string.IsNullOrEmpty(warning))
                    {
                        warnings.Add($"第{i}行：" + warning);
                    }
                }
                #region 自身导入的数据的重复检查
                var selfCount = (from p in mps
                                 group p by new { p.CountyName, p.NeighborhoodsName, p.CommunityName, p.ViligeName, p.MPNumber, p.HSNumber } into g
                                 where g.Count() > 1
                                 select g).ToList();
                if (selfCount.Count > 0)
                {
                    var error = new CountryMPErrors();
                    error.Title = "导入的数据中有重复";
                    foreach (var group in selfCount)
                    {
                        var erMsg = $"{group.Key.CountyName} {group.Key.NeighborhoodsName} {group.Key.CommunityName} {group.Key.ViligeName} {group.Key.MPNumber}";
                        var hs = group.Key.HSNumber == null ? "" : group.Key.HSNumber + "室";
                        erMsg = string.IsNullOrEmpty(erMsg) ? $"存在多条空行" : erMsg + " 号" + hs;
                        error.ErrorMessages.Add(erMsg);
                    }
                    errors.Add(error);
                }
                #endregion

                Dictionary<string, object> D = new Dictionary<string, object>();
                D.Add(mpKey, mps);
                D.Add(errorKey, errors);
                D.Add(warningKey, warnings);
                temp[LoginUtils.CurrentUser.UserName] = D;
                //HttpContext.Current.Session["_CountryMP"] = mps;
                //HttpContext.Current.Session["_CountryMPErrors"] = errors;
                //HttpContext.Current.Session["_CountryMPWarning"] = warnings;
            }
        }
        public static Dictionary<string, object> GetUploadCountryMP(int PageSize, int PageNum)
        {
            List<CountryMPDetails> rows = null;
            int totalCount = 0;
            if (temp[LoginUtils.CurrentUser.UserName][mpKey] != null)
            {
                rows = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                totalCount = (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>).Count;
            }
            return new Dictionary<string, object> {
                { "Data",rows},
                { "Count",totalCount},
                { "Errors",(temp[LoginUtils.CurrentUser.UserName][errorKey] as List<CountryMPErrors>)},
                { "Warnings",(temp[LoginUtils.CurrentUser.UserName][warningKey] as List<string>)}
            };
        }
        public static SummarySheet UpdateCountryMP()
        {
            var mps = temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>;
            var errors = temp[LoginUtils.CurrentUser.UserName][errorKey] as List<CountryMPErrors>;
            if (errors.Count > 0)
                throw new Exception("数据包含错误信息，请先检查数据！");
            if (mps.Count() == 0 || mps == null)
                throw new Exception("无可导入数据！");

            #region****************没有错误后将导入的数据更新到住宅门牌数据库中
            foreach (var mp in (temp[LoginUtils.CurrentUser.UserName][mpKey] as List<CountryMPDetails>))
            {
                MPOfCountry m = new MPOfCountry
                {
                    CountyID = mp.CountyID,
                    NeighborhoodsID = mp.NeighborhoodsID,
                    CommunityID = mp.CommunityID,
                    ViligeName = mp.ViligeName,
                    MPNumber = mp.MPNumber,
                    OriginalNumber = mp.OriginalNumber,
                    HSNumber = mp.HSNumber,
                    PropertyOwner = mp.PropertyOwner,
                    CreateTime = mp.CreateTime,
                    MPSize = mp.MPSize,
                    Applicant = mp.Applicant,
                    ApplicantPhone = mp.ApplicantPhone,
                    MPProduce = mp.MPProduce
                };
                ModifyCountryMP(m, null, null, null);
            }
            temp.Remove(LoginUtils.CurrentUser.UserName);
            #endregion
            #region*************批量导入成功后立刻进行门牌制作，更新到门牌制作表中，再给出门牌制作汇总表********************
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var bigMPsize = dbContext.DMBZDic.Where(t => t.Type == "大门牌").Select(t => t.Size).ToList();
                var Vilige = (from t in mps
                              group t by new
                              {
                                  t.CountyID,
                                  t.NeighborhoodsID,
                                  t.CommunityID,
                                  t.ViligeName
                              } into g
                              select new Record
                              {
                                  CountyID = g.Key.CountyID,
                                  NeighborhoodsID = g.Key.NeighborhoodsID,
                                  CommunityID = g.Key.CommunityID,
                                  Name = g.Key.ViligeName,
                                  Type = "自然村",
                                  Count = 0
                              }).ToList();
                var NCMP = (from t in mps
                            group t by new
                            {
                                t.CountyID,
                                t.NeighborhoodsID,
                                t.CommunityID,
                                t.ViligeName,
                                t.MPNumber,
                            } into g
                            select new Record
                            {
                                CountyID = g.Key.CountyID,
                                NeighborhoodsID = g.Key.NeighborhoodsID,
                                CommunityID = g.Key.CommunityID,
                                Name = g.Key.ViligeName,
                                Type = g.Key.MPNumber,
                                Count = g.Count()
                            }).OrderBy(t => t.Type).ToList();

                NCMP.Add((from t in NCMP
                          group t by new
                          {
                              t.CountyID,
                              t.NeighborhoodsID,
                              t.CommunityID,
                              t.Name,
                          } into g
                          select new Record
                          {
                              CountyID = g.Key.CountyID,
                              NeighborhoodsID = g.Key.NeighborhoodsID,
                              CommunityID = g.Key.CommunityID,
                              Name = g.Key.Name,
                              Type = "农村门牌合计",
                              Count = g.Sum(t => t.Count)
                          }).First());

                List<Models.Entities.MPProduce> mpPros = new List<Models.Entities.MPProduce>();
                foreach (var vilige in Vilige)
                {
                    Models.Entities.MPProduce mpPro = new Models.Entities.MPProduce();
                    mpPro.ID = Guid.NewGuid().ToString();
                    mpPro.CommunityID = vilige.CommunityID;
                    mpPro.NeighborhoodsID = vilige.NeighborhoodsID;
                    mpPro.CountyID = vilige.CountyID;
                    mpPro.MPType = Enums.MPType.Country;
                    mpPro.CreateTime = DateTime.Now.Date;
                    mpPro.CreateUser = LoginUtils.CurrentUser.UserName;
                    mpPro.CountryMPCount = NCMP.Where(t => t.CountyID == vilige.CountyID).Where(t => t.NeighborhoodsID == vilige.NeighborhoodsID).Where(t => t.CommunityID == vilige.CommunityID).Where(t => t.Name == vilige.Name).Where(t => t.Type == "农村门牌合计").Select(t => t.Count).First();
                    mpPro.TotalCount = mpPro.CountryMPCount;
                    mpPros.Add(mpPro);
                }
                dbContext.MPProduce.AddRange(mpPros);
                dbContext.SaveChanges();
                SummarySheet data = new SummarySheet();
                data.StandardName = Vilige;
                return data;
            }
            #endregion
            //HttpContext.Current.Session["_CountryMP"] = null;
            //HttpContext.Current.Session["_CountryMPErrors"] = null;
            //HttpContext.Current.Session["_CountryMPWarning"] = null;
        }
        public static void CancelOrDelCountryMP(string ID, int UseState)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                IQueryable<MPOfCountry> query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID);
                int count = query.Count();
                if (count == 0)
                    throw new Exception("该条数据已被注销，请重新查询！");

                var CommunityID = query.Select(t => t.CommunityID).FirstOrDefault();
                if (!DistrictUtils.CheckPermission(CommunityID))
                    throw new Exception("无权修改其他镇街数据！");

                var user = LoginUtils.CurrentUser;
                var sql = $@"update [TopSystemDB].[dbo].[MPOFCOUNTRY] set [MPOFCOUNTRY].State={UseState},[MPOFCOUNTRY].[CancelTime]=GETDATE(),[MPOFCOUNTRY].[CancelUser]='{user.UserName}' where [MPOFCOUNTRY].ID='{ID}'";
                var rt = dbContext.Database.ExecuteSqlCommand(sql);
                if (rt == 0)
                    throw new Exception("数据注销失败，请重试！");
            }
        }
        public static bool CheckCountryMPIsAvailable(string ViligeName, string MPNumber, string HSNumber, string CountyID, string NeighborhoodsID, string CommunityID)
        {
            var check = true;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityID == CommunityID).Where(t => t.ViligeName == ViligeName.Trim());
                if (query.Count() > 0)
                {
                    query = query.Where(t => t.MPNumber == MPNumber);

                    if (string.IsNullOrEmpty(HSNumber))
                    {
                        if (query.Count() > 0)
                            check = false;
                    }
                    else
                    {
                        query = query.Where(t => t.HSNumber == HSNumber);
                        if (query.Count() > 0)
                            check = false;
                    }

                }
            }
            return check;
        }
        #endregion

        /// <summary>
        /// 保存文件，检查文件是否重名，并返回文件名列表
        /// </summary>
        /// <param name="files"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static List<string> SaveMPFiles(IList<HttpPostedFile> files, string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            List<string> UploadNames = new List<string>();

            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {
                    int counter = 1;
                    string filename = file.FileName;
                    string path = Path.Combine(directory, filename);
                    string extension = Path.GetExtension(path);
                    string newFullPath = path;
                    while (System.IO.File.Exists(newFullPath))
                    {
                        string newFilename = $"{Path.GetFileNameWithoutExtension(path)}({counter}){extension}";
                        newFullPath = Path.Combine(directory, newFilename);
                        counter++;
                    }
                    UploadNames.Add(Path.GetFileName(newFullPath));
                    MemoryStream m = new MemoryStream();
                    FileStream fs = new FileStream(newFullPath, FileMode.OpenOrCreate);
                    BinaryWriter w = new BinaryWriter(fs);
                    w.Write(m.ToArray());
                    fs.Close();
                    m.Close();
                }
            }
            return UploadNames;
        }
        /// <summary>
        /// 将所有的文件存到文件夹，并将记录存入MPOfUploadFiles表中，首先要获取到每个文件的文件名和证件类型，取当前这个门牌ID，再生成一个GUID，存入数据库
        /// </summary>
        /// <param name="files">文件集合</param>
        /// <param name="MPID">门牌ID</param>
        /// <param name="DocType">证件类型</param>
        /// <param name="MPTypeStr">门牌类型</param>
        /// <returns></returns>
        private static void SaveMPFilesByID(IList<HttpPostedFile> files, string MPID, string DocType, string MPTypeStr)
        {
            var directory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", MPTypeStr, MPID, DocType);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                foreach (var file in files)
                {
                    if (file.ContentLength > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        string filename = file.FileName;
                        string extension = Path.GetExtension(filename);
                        string newfilename = guid + extension;
                        string fullPath = Path.Combine(directory, newfilename);
                        MemoryStream m = new MemoryStream();
                        FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
                        BinaryWriter w = new BinaryWriter(fs);
                        w.Write(m.ToArray());
                        fs.Close();
                        m.Close();
                        MPOfUploadFiles data = new MPOfUploadFiles();
                        data.ID = guid;
                        data.MPID = MPID;
                        data.Name = filename;
                        data.FileType = extension;
                        data.DocType = DocType;
                        data.State = Enums.UseState.Enable;
                        dbContext.MPOfUploadFiles.Add(data);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 从表中所有文件中删选出不在当前文件中的一些数据并逻辑删除，新增的一些文件存进去
        /// </summary>
        /// <param name="CurrentIDs">当前所有文件的ID/param>
        /// <param name="AdddedFiles">新增加的文件</param>
        /// <param name="MPID"></param>
        /// <param name="DocType"></param>
        /// <param name="MPTypeStr"></param>
        private static void UpdateMPFilesByID(List<string> CurrentIDs, IList<HttpPostedFile> AdddedFiles, string MPID, string DocType, string MPTypeStr)
        {
            var directory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", MPTypeStr, MPID, DocType);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (CurrentIDs != null && CurrentIDs.Count() > 0)
                {
                    string sql = $@"update [TopSystemDB].[dbo].[MPOFUPLOADFILES]  
                                    set [State]={Enums.UseState.Delete} 
                                    where [ID] not in ({string.Join(",", CurrentIDs)}) 
                                    and [State]={Enums.UseState.Enable} 
                                    and [MPID]='{MPID}' 
                                    and [DocType]='{DocType}'";
                    var rt = dbContext.Database.ExecuteSqlCommand(sql);
                }
                SaveMPFilesByID(AdddedFiles, MPID, DocType, MPTypeStr);
            }
        }
        /// <summary>
        /// 正则表达式检查字符串是否是数字
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool CheckIsNumber(string number)
        {
            const string pattern = "^[1-9]*[1-9][0-9]*$";
            Regex rx = new Regex(pattern);
            return rx.IsMatch(number);
        }
        /// <summary>
        /// 正则表达式检查字符串是否是电话号码或手机号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool CheckIsPhone(string number)
        {
            const string patternPhone = @"^1(3|4|5|7|8)\d{9}$";
            const string patternTel = @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{7,14}$";
            Regex rxPhone = new Regex(patternPhone);
            Regex rxTel = new Regex(patternTel);
            return rxPhone.IsMatch(number) || rxTel.IsMatch(number);
        }
        /// <summary>
        /// 检查是否为日期格式  2018年7月25日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static bool CheckIsDatetime(ref string date)
        {
            try
            {
                date = Convert.ToDateTime(date).ToString("d");
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 检查门牌规格是否在几个规定的规格里面
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool CheckMPSize(ref string data)
        {
            data = data.ToLower();
            var MPSizes = new string[] { "40*60CM", "30*20CM", "21*15MM", "18*14MM", "15*10MM" };
            return MPSizes.Contains(data);
        }
        public static bool CheckPostcode(string data)
        {
            const string pattern = @"^\d{6}$";
            Regex rx = new Regex(pattern);
            return data.StartsWith("3140") && rx.IsMatch(data);
        }
        private static bool CheckMPProdece(string data, ref int MPProdece)
        {
            var t = true;
            if (data.Equals("是"))
                MPProdece = 1;
            else if (data.Equals("否"))
                MPProdece = 2;
            else
                t = false;
            return t;
        }

        //public void ProcessRequest(HttpContext context)
        //{
        //    throw new NotImplementedException();
        //}

    }
}