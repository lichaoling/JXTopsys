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
    public class ResidenceMPModify
    {
        /// <summary>
        /// 一条数据的新增或者修改
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        ///  <param name="CurrentFileIDs">存储四个证件的所有ids</param>
        public static void ModifyResidenceMP(MPOfResidence newData, string oldDataJson)
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
                    if (!CheckResidenceMPIsAvailable(newData.CountyID, newData.NeighborhoodsID, newData.CommunityName, newData.ResidenceName, newData.MPNumber, newData.Dormitory, newData.HSNumber, newData.LZNumber, newData.DYNumber))
                        throw new Exception("该住宅门牌已经存在，请检查后重新输入！");
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var CountyName = newData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = newData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = newData.CommunityName;
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + newData.ResidenceName + newData.MPNumber == null ? "" : newData.MPNumber + "号" + newData.Dormitory + newData.LZNumber == null ? "" : newData.LZNumber + "幢" + newData.DYNumber == null ? "" : newData.DYNumber + "单元" + newData.HSNumber == null ? "" : newData.HSNumber + "室";
                    #endregion
                    #region 地址编码前10位拼接
                    var CountyCode = dbContext.District.Where(t => t.ID == newData.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = dbContext.District.Where(t => t.ID == newData.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                    var year = DateTime.Now.Year.ToString();
                    //地址编码  不带流水号，流水号由数据库触发器生成
                    var AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = newData.CountyID;
                    CommunityDic.NeighborhoodsID = newData.NeighborhoodsID;
                    CommunityDic.CommunityName = newData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区下小区名称是否在字典表中存在，若不存在就新增
                    var ResidenceDic = new ResidenceDic();
                    ResidenceDic.CountyID = newData.CountyID;
                    ResidenceDic.NeighborhoodsID = newData.NeighborhoodsID;
                    ResidenceDic.CommunityName = newData.CommunityName;
                    ResidenceDic.ResidenceName = newData.ResidenceName;
                    newData.ResidenceID = DicUtils.AddResidenceDic(ResidenceDic);
                    #endregion

                    //对这条数据进行默认赋值
                    newData.ID = Guid.NewGuid().ToString();
                    newData.AddressCoding = AddressCoding;
                    newData.DYPosition = (newData.Lng != null && newData.Lat != null) ? (DbGeography.FromText($"POINT({newData.Lng},{newData.Lat})")) : null;
                    newData.StandardAddress = StandardAddress;
                    newData.AddType = Enums.MPAddType.LX;
                    newData.MPProduce = Enums.MPProduce.NO;
                    newData.MPProduceComplete = Enums.Complete.NO;
                    newData.MPZPrintComplete = Enums.Complete.NO;
                    newData.DZZMPrintComplete = Enums.Complete.NO;
                    newData.State = Enums.UseState.Enable;
                    newData.CreateTime = DateTime.Now.Date;
                    newData.CreateUser = LoginUtils.CurrentUser.UserName;
                    dbContext.MPOfResidence.Add(newData);
                }
                #endregion
                #region 修改
                else
                {
                    var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<MPOfResidence>(oldDataJson);
                    var targetData = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                    if (targetData == null)
                        throw new Exception("该条数据已被注销，请重新查询并编辑！");
                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);

                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Exception("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckResidenceMPIsAvailable(targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.ResidenceName, targetData.MPNumber, targetData.Dormitory, targetData.HSNumber, targetData.LZNumber, targetData.DYNumber))
                        throw new Exception("该住宅牌已经存在，请检查后重新修改！");
                    #endregion
                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var CountyName = targetData.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = targetData.NeighborhoodsID.Split('.')[2];
                    var CommunityName = targetData.CommunityName;
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + targetData.ResidenceName + targetData.MPNumber == null ? "" : targetData.MPNumber + "号" + targetData.Dormitory + targetData.LZNumber == null ? "" : targetData.LZNumber + "幢" + targetData.DYNumber == null ? "" : targetData.DYNumber + "单元" + targetData.HSNumber == null ? "" : targetData.HSNumber + "室";
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
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

                    //对这条数据进行默认赋值
                    targetData.DYPosition = targetData.Lng != null && targetData.Lat != null ? (DbGeography.FromText($"POINT({targetData.Lng},{targetData.Lat})")) : targetData.DYPosition;
                    targetData.StandardAddress = StandardAddress;
                    targetData.LastModifyTime = DateTime.Now.Date;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                }
                #endregion
                dbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 逐条上传，成功就保存，错误就显示错误信息，继续下一行上传
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
            if (columnCount < 17)
                throw new Exception("数据列不完整！");

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var PLID = Guid.NewGuid().ToString();
                for (int i = 1; i < rowCount; i++)
                {
                    var error = new ResidenceMPErrors();
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
                    var ResidenceName = row[4].Value == null ? null : row[4].Value.ToString().Trim();
                    var MPNumber = row[5].Value == null ? null : row[5].Value.ToString().Replace(" ", "");
                    var Dormitory = row[6].Value == null ? null : row[6].Value.ToString().Trim();
                    var LZNumber = row[7].Value == null ? null : row[7].Value.ToString().Replace(" ", "");
                    var DYNumber = row[8].Value == null ? null : row[8].Value.ToString().Replace(" ", "");
                    var HSNumber = row[9].Value == null ? null : row[9].Value.ToString().Replace(" ", "");
                    var PropertyOwner = row[10].Value == null ? null : row[10].Value.ToString().Trim();
                    var BZTime = row[11].Value == null ? null : row[11].Value.ToString().Trim();
                    var MPSize = row[12].Value == null ? null : row[12].Value.ToString().Trim();
                    var Applicant = row[13].Value == null ? null : row[13].Value.ToString().Trim();
                    var ApplicantPhone = row[14].Value == null ? null : row[14].Value.ToString().Trim();
                    var SBDW = row[15].Value == null ? null : row[15].Value.ToString().Trim();
                    var Postcode = row[16].Value == null ? null : row[16].Value.ToString().Trim();

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
                    if (string.IsNullOrEmpty(ResidenceName))
                    {
                        error.ErrorMessages.Add("小区名称为空");
                    }
                    if (!string.IsNullOrEmpty(MPNumber))
                    {
                        if (string.IsNullOrEmpty(MPSize))
                            error.ErrorMessages.Add("有门牌号时门牌规格不能为空");
                    }
                    #endregion
                    #region 户室牌查重
                    if (!CheckResidenceMPIsAvailable(CountyID, NeighborhoodsID, CommunityName, ResidenceName, MPNumber, Dormitory, HSNumber, LZNumber, DYNumber))
                    {
                        error.ErrorMessages.Add("该户室牌已经存在");
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
                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var StandardAddress = CountyName + NeighborhoodsName + CommunityName + ResidenceName + MPNumber == null ? "" : MPNumber + "号" + Dormitory + LZNumber == null ? "" : LZNumber + "幢" + DYNumber == null ? "" : DYNumber + "单元" + HSNumber == null ? "" : HSNumber + "室";
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = CountyID;
                    CommunityDic.NeighborhoodsID = NeighborhoodsID;
                    CommunityDic.CommunityName = CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion
                    #region 检查这个行政区下小区名称是否在字典表中存在，若不存在就新增
                    var ResidenceDic = new ResidenceDic();
                    ResidenceDic.CountyID = CountyID;
                    ResidenceDic.NeighborhoodsID = NeighborhoodsID;
                    ResidenceDic.CommunityName = CommunityName;
                    ResidenceDic.ResidenceName = ResidenceName;
                    var ResidenceID = DicUtils.AddResidenceDic(ResidenceDic);
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
                        var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                        var year = DateTime.Now.Year.ToString();
                        AddressCoding = CountyCode + NeighborhoodsCode + mpCategory + year;
                        #endregion
                        var data = new MPOfResidence();
                        data.ID = Guid.NewGuid().ToString();
                        data.AddressCoding = AddressCoding;
                        data.CountyID = CountyID;
                        data.NeighborhoodsID = NeighborhoodsID;
                        data.CommunityName = CommunityName;
                        data.ResidenceID = ResidenceID;
                        data.ResidenceName = ResidenceName;
                        data.MPNumber = MPNumber;
                        data.Dormitory = Dormitory;
                        data.LZNumber = LZNumber;
                        data.DYNumber = DYNumber;
                        data.HSNumber = HSNumber;
                        data.PropertyOwner = PropertyOwner;
                        data.StandardAddress = StandardAddress;
                        data.BZTime = BZTimeBZ;
                        data.Applicant = Applicant;
                        data.ApplicantPhone = ApplicantPhone;
                        data.SBDW = SBDW;
                        data.Postcode = Postcode;
                        data.State = Enums.UseState.Enable;
                        data.CreateTime = DateTime.Now.Date;
                        data.CreateUser = LoginUtils.CurrentUser.UserName;
                        data.AddType = Enums.MPAddType.PL;
                        data.PLID = PLID;
                        data.MPProduce = Enums.MPProduce.Yes;
                        data.MPProduceComplete = Enums.Complete.NO;
                        data.MPZPrintComplete = Enums.Complete.NO;
                        data.DZZMPrintComplete = Enums.Complete.NO;

                        dbContext.MPOfResidence.Add(data);
                    }
                    #endregion 新增
                    #region 批量更新
                    else
                    {
                        var data = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddressCoding == AddressCoding).First();
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
                        data.ResidenceID = ResidenceID;
                        data.ResidenceName = ResidenceName;
                        data.MPNumber = MPNumber;
                        data.Dormitory = Dormitory;
                        data.LZNumber = LZNumber;
                        data.DYNumber = DYNumber;
                        data.HSNumber = HSNumber;
                        data.PropertyOwner = PropertyOwner;
                        data.StandardAddress = StandardAddress;
                        data.BZTime = BZTimeBZ;
                        data.Applicant = Applicant;
                        data.ApplicantPhone = ApplicantPhone;
                        data.SBDW = SBDW;
                        data.Postcode = Postcode;
                        data.LastModifyTime = DateTime.Now.Date;
                        data.LastModifyUser = LoginUtils.CurrentUser.UserName;

                        dbContext.MPOfResidence.Add(data);

                    }
                    #endregion 更新
                    success = $"第{i}行导入成功";
                }
                dbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 批量注销
        /// </summary>
        /// <param name="Data"></param>
        public static void CancelResidenceMP(List<string> ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => ID.Contains(t.ID)).ToList();
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
        public static bool CheckResidenceMPIsAvailable(string CountyID, string NeighborhoodsID, string CommunityName, string ResidenceName, string MPNumber, string Dormitory, string HSNumber, string LZNumber, string DYNumber)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityName == CommunityName).Where(t => t.ResidenceName == ResidenceName).Where(t => t.MPNumber == MPNumber).Where(t => t.Dormitory == Dormitory).Where(t => t.LZNumber == LZNumber).Where(t => t.DYNumber == DYNumber).Where(t => t.HSNumber == HSNumber).Count();
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
        //        throw new Exception("数据包含错误信息，请先检查数据！");
        //    if (mps.Count() == 0 || mps == null)
        //        throw new Exception("无可导入数据！");
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