using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.PlaceName
{
    public class DMOfZYSS
    {

        public static List<string> GetDMTypesFromData(string DistrictID, string ZYSSType)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                List<string> DMLBTypes = new List<string>();
                IQueryable<Models.Entities.DMOFZYSS> query = db.ZYSS.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ZYSSType == ZYSSType);
                query = BaseUtils.DataFilterWithTown<Models.Entities.DMOFZYSS>(query);
                if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "嘉兴市"))
                {
                    query = query.Where(t => t.CountyID == DistrictID || t.NeighborhoodsID == DistrictID);
                }
                DMLBTypes = query.Select(t => t.DMType).Distinct().ToList();
                return DMLBTypes;
            }
        }

        /// <summary>
        /// 住宅门牌查询，根据行政区划ID和名称（小区名或道路名或宿舍名）
        /// 关联道路表和行政区划表，获取到行政区划的名称、道路名称和住宅门牌的信息
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SearchDMOfZYSS(int PageSize, int PageNum, string ZYSSType, string DistrictID, string CommunityName, string DMType, string Name, string ZGDW, DateTime? start, DateTime? end)
        {
            int count = 0;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.ZYSS.Where(t => t.State == 1);
                query = BaseUtils.DataFilterWithTown<Models.Entities.DMOFZYSS>(query);
                if (!string.IsNullOrEmpty(ZYSSType))
                {
                    query = query.Where(t => t.ZYSSType == ZYSSType);
                }
                if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "嘉兴市"))
                {
                    query = query.Where(t => t.CountyID == DistrictID || t.NeighborhoodsID == DistrictID);
                }
                if (!string.IsNullOrEmpty(CommunityName))
                {
                    query = query.Where(t => t.CommunityName == CommunityName);
                }
                if (!string.IsNullOrEmpty(DMType))
                {
                    query = query.Where(t => t.DMType == DMType);
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    query = query.Where(t => t.Name.Contains(Name));
                }
                if (!string.IsNullOrEmpty(ZGDW))
                {
                    query = query.Where(t => t.Name.Contains(ZGDW));
                }
                if (start != null || end != null)
                {
                    if (start != null)
                        query = query.Where(t => t.ApplicantDate >= start);
                    if (end != null)
                        query = query.Where(t => t.ApplicantDate <= end);
                }
                count = query.Count();
                List<Models.Entities.DMOFZYSS> result;
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    result = query.OrderByDescending(t => t.ApplicantDate).ToList();
                }
                //如果是分页查询，就分页返回
                else
                {
                    result = query.OrderByDescending(t => t.ApplicantDate).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                }

                return new Dictionary<string, object> {
                   { "Data",result },
                   { "Count",count}
                };
            }
        }

        /// <summary>
        /// 根据一条住宅门牌数据的ID来查详情
        /// </summary>
        /// <param name="ID"></param>
        public static Models.Entities.DMOFZYSS SearchDMOfZYSSByID(string ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = (from t in dbContext.ZYSS
                             where t.State == Enums.UseState.Enable && t.ID == ID
                             select t).FirstOrDefault();
                if (query == null)
                    throw new Error("该地名已经被注销！");

                //将附件的名字都加上路径返回
                var files = dbContext.DMOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.DMID == ID);
                if (files.Count() > 0)
                {
                    var SQB = files.Where(t => t.DocType == Enums.DocType.SQB);
                    var LXPFS = files.Where(t => t.DocType == Enums.DocType.LXPFS);
                    var SJT = files.Where(t => t.DocType == Enums.DocType.SJT);
                    var baseUrl = Path.Combine(StaticVariable.ProfessionalDMRelativePath, ID);
                    if (SQB.Count() > 0)
                    {
                        query.SBBG = (from t in SQB
                                      select new Pictures
                                      {
                                          FileID = t.ID,
                                          Name = t.Name,
                                          RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                                          TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                                      }).ToList();
                    }
                    if (LXPFS.Count() > 0)
                    {
                        query.LXPFWJ = (from t in LXPFS
                                        select new Pictures
                                        {
                                            FileID = t.ID,
                                            Name = t.Name,
                                            RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                                            TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                                        }).ToList();
                    }
                    if (SJT.Count() > 0)
                    {
                        query.SJT = (from t in SJT
                                     select new Pictures
                                     {
                                         FileID = t.ID,
                                         Name = t.Name,
                                         RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                                         TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                                     }).ToList();
                    }
                }
                return query;
            }

        }

        /// <summary>
        /// 导出住宅门牌Excel
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static MemoryStream ExportDMOfZYSS(int PageSize, int PageNum, string ZTSSTypoe, string DistrictID, string CommunityName, string DMType, string Name, string ZGDW, DateTime? start, DateTime? end)
        {
            Dictionary<string, object> dict = SearchDMOfZYSS(-1, -1, ZTSSTypoe, DistrictID, CommunityName, DMType, Name, ZGDW, start, end);

            int RowCount = int.Parse(dict["Count"].ToString());
            if (RowCount >= 65000)
                throw new Error("数据量过大，请缩小查询范围后再导出！");
            var Data = dict["Data"] as List<Models.Entities.DMOFZYSS>;

            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            ws.Name = Enums.PlaceNameTypeCH.ZYSS;
            Aspose.Cells.Style styleHeader = wb.Styles[wb.Styles.Add()];
            styleHeader.Pattern = Aspose.Cells.BackgroundType.Solid;
            styleHeader.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            styleHeader.ForegroundColor = System.Drawing.Color.FromArgb(240, 240, 240);
            styleHeader.Font.IsBold = true;
            styleHeader.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            Aspose.Cells.Style styleData = wb.Styles[wb.Styles.Add()];
            styleData.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            styleData.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            styleData.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            styleData.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            List<ExcelFields> Fields = new List<ExcelFields> {
                new ExcelFields() { Field="市辖区",Alias="CountyName"},
                new ExcelFields() { Field="镇街道",Alias="NeighborhoodsName"},
                new ExcelFields() { Field="村社区",Alias="CommunityName"},
                new ExcelFields() { Field="地名类别",Alias="DMType"},
                new ExcelFields() { Field="小类类别",Alias="SmallType"},
                new ExcelFields() { Field="邮政编码",Alias="Postcode"},
                new ExcelFields() { Field="标准名称",Alias="Name"},
                new ExcelFields() { Field="罗马字母拼写",Alias="Pinyin"},
                new ExcelFields() { Field="申报单位",Alias="SBDW"},
                new ExcelFields() { Field="统一社会信用代码",Alias="XYDM"},
                new ExcelFields() { Field="项目地址",Alias="XMAddress"},
                new ExcelFields() { Field="东至",Alias="East"},
                new ExcelFields() { Field="南至",Alias="South"},
                new ExcelFields() { Field="西至",Alias="West"},
                new ExcelFields() { Field="北至",Alias="North"},
                new ExcelFields() { Field="地名含义",Alias="DMHY"},
                new ExcelFields() { Field="地理实体概况",Alias="DLST"},
                new ExcelFields() { Field="申办人",Alias="Applicant"},
                new ExcelFields() { Field="联系电话",Alias="Telephone"},
                new ExcelFields() { Field="联系地址",Alias="ContractAddress"},
                new ExcelFields() { Field="主管单位",Alias="ZGDW"},
                new ExcelFields() { Field="申请日期",Alias="ApplicantDate"},
                new ExcelFields() { Field="备案日期",Alias="RecordDate"},
                new ExcelFields() { Field="纬度",Alias="X"},
                new ExcelFields() { Field="经度",Alias="Y"},
            };
            //写入表头
            for (int i = 0, l = Fields.Count; i < l; i++)
            {
                var field = Fields[i];
                ws.Cells[0, i].PutValue(field.Field);
                ws.Cells[0, i].SetStyle(styleHeader);
            }
            //写入数据
            for (int i = 0; i < RowCount; i++)
            {
                var row = Data[i];
                for (int j = 0, l = Fields.Count; j < l; j++)
                {
                    var field = Fields[j];
                    var value = row[field.Alias];
                    if (field.Field == "申请日期")
                    {
                        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                        timeConverter.DateTimeFormat = "yyyy-MM-dd";
                        string rt = Newtonsoft.Json.JsonConvert.SerializeObject(value, timeConverter);
                        value = rt.Replace("\"", "");
                    }
                    if (field.Field == "备案日期")
                    {
                        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                        timeConverter.DateTimeFormat = "yyyy-MM-dd";
                        string rt = Newtonsoft.Json.JsonConvert.SerializeObject(value, timeConverter);
                        value = rt.Replace("\"", "");
                    }

                    ws.Cells[i + 1, j].PutValue(value);
                    ws.Cells[i + 1, j].SetStyle(styleData);
                }
            }
            ws.AutoFitColumns();
            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Excel97To2003);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// 一条数据的新增或者修改
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        ///  <param name="CurrentFileIDs">存储四个证件的所有ids</param>
        public static void ModifyDMOfZYSS(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Entities.DMOFZYSS>(oldDataJson);
                var targetData = dbContext.ZYSS.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                if (targetData == null) //新增
                {
                    targetData = new Models.Entities.DMOFZYSS();
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    #region 权限检查
                    var districtID = targetData.NeighborhoodsID == null ? targetData.CountyID : targetData.NeighborhoodsID;
                    if (!DistrictUtils.CheckPermission(districtID))
                        throw new Error("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckZYSSIsAvailable(targetData.ID, targetData.ZYSSType, targetData.DMType, targetData.SmallType, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.Name))
                        throw new Exception("该专业设施已经存在，请检查后重新输入！");
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new Models.Entities.CommunityDic();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion

                    targetData.Geom = targetData.Lng != null && targetData.Lat != null ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : targetData.Geom;
                    targetData.State = Enums.UseState.Enable;
                    targetData.CreateTime = DateTime.Now;
                    targetData.CreateUser = LoginUtils.CurrentUser.UserID;
                    targetData.SBLY = Enums.SBLY.zj;
                    dbContext.ZYSS.Add(targetData);
                }
                else //修改
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);

                    #region 权限检查
                    if (!DistrictUtils.CheckPermission(targetData.NeighborhoodsID))
                        throw new Error("无权操作其他镇街数据！");
                    #endregion
                    #region 重复性检查
                    if (!CheckZYSSIsAvailable(targetData.ID, targetData.ZYSSType, targetData.DMType, targetData.SmallType, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.Name))
                        throw new Exception("该专业设施已经存在，请检查后重新输入！");
                    #endregion
                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new Models.Entities.CommunityDic();
                    CommunityDic.CountyID = targetData.CountyID;
                    CommunityDic.NeighborhoodsID = targetData.NeighborhoodsID;
                    CommunityDic.CommunityName = targetData.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion

                    targetData.Geom = targetData.Lng != null && targetData.Lat != null ? (DbGeography.FromText($"POINT({targetData.Lng} {targetData.Lat})")) : targetData.Geom;
                    targetData.LastModifyTime = DateTime.Now;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserID;

                    if (targetData.DataPushStatus == 1)
                    {
                        targetData.DataPushStatus = 0;
                    }
                }
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 注销地名
        /// </summary>
        /// <param name="ID"></param>
        public static void CancelDMOfZYSS(List<string> ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.ZYSS.Where(t => t.State == Enums.UseState.Enable).Where(t => ID.Contains(t.ID)).ToList();
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
        public static bool CheckZYSSIsAvailable(string ID, string ZYSSType, string DMType, string SmallType, string CountyID, string NeighborhoodsID, string CommunityName, string Name)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.ZYSS.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID != ID).Where(t => t.ZYSSType == ZYSSType).Where(t => t.DMType == DMType).Where(t => t.SmallType == SmallType).Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityName == CommunityName).Where(t => t.Name == Name).Count();
                return count == 0;
            }
        }
    }
}