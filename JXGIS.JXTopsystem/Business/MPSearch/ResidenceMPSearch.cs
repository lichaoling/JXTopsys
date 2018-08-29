using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.MPSearch
{
    public class ResidenceMPSearch
    {
        #region 住宅门牌
        /// <summary>
        /// 住宅门牌查询，根据行政区划ID和名称（小区名或道路名或宿舍名）
        /// 关联道路表和行政区划表，获取到行政区划的名称、道路名称和住宅门牌的信息
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SearchResidenceMP(int PageSize, int PageNum, string DistrictID, string ResidenceName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState)
        {
            #region sql语句写法 注释
            //string sql = $@"select a.[ID]
            //              ,a.[AddressCoding]
            //              ,a.[CountyID]
            //           ,d.Name CountyName
            //              ,a.[NeighborhoodsID]
            //           ,e.Name NeighborhoodsName
            //              ,a.[CommunityID]
            //           ,f.Name CommunityName
            //              ,a.[RoadID]
            //           ,c.biaozhunmingcheng RoadName
            //              ,a.[MPNumber]
            //              ,a.[ResidenceName]
            //              ,a.[Dormitory]
            //              ,a.[LZNumber]
            //              ,a.[DYNumber]
            //              ,a.[DYPosition]
            //              ,a.[HSNumber]
            //              ,a.[MPSize]
            //              ,a.[Postcode]
            //              ,a.[PropertyOwner]
            //              ,a.[IDType]
            //              ,a.[IDNumber]
            //              ,a.[StandardAddress]
            //              ,a.[FCZAddress]
            //              ,a.[FCZNumber]
            //              ,a.[FCZFile]
            //              ,a.[TDZAddress]
            //              ,a.[TDZNumber]
            //              ,a.[TDZFile]
            //              ,a.[BDCZAddress]
            //              ,a.[BDCZNumber]
            //              ,a.[BDCZFile]
            //              ,a.[HJAddress]
            //              ,a.[HJNumber]
            //              ,a.[HJFile]
            //              ,a.[OtherAddress]
            //              ,a.[Applicant]
            //              ,a.[ApplicantPhone]
            //              ,a.[CreateTime]
            //              ,a.[CreateUser]
            //              ,a.[LastModifyTime]
            //              ,a.[LastModifyUser]
            //              ,a.[State]
            //              ,a.[CancelTime]
            //              ,a.[CancelUser]
            //           ,ROW_NUMBER() OVER(Order by a.[CreateTime] desc) AS RowId
            //          FROM [TopSystemDB].[dbo].[MPOFRESIDENCE] a
            //          left join [TopSystemDB].[dbo].[TopCombineRoad] c on a.roadid=c.dmid
            //          left join [TopSystemDB].[dbo].[DISTRICT] d on a.CountyID=d.ID
            //          left join [TopSystemDB].[dbo].[DISTRICT] e on a.NeighborhoodsID=e.ID
            //          left join [TopSystemDB].[dbo].[DISTRICT] f on a.CommunityID=f.ID
            //          where a.[State]=1 ";

            //if (!string.IsNullOrEmpty(DistrictID))
            //{
            //    sql += $"and a.[CommunityID] like '{DistrictID}.%' ";
            //}

            //if (!string.IsNullOrEmpty(Name))
            //{
            //    sql += $"and c.biaozhunmingcheng like '%{Name}%' or a.ResidenceName like '%{Name}%' or a.Dormitory like '%{Name}%' ";
            //}

            ////计算总条数
            //var rowSql = $@"select count(1) from ({sql}) b";
            //var rowsCount = SystemUtils.EFDbContext.Database.SqlQuery<int>(rowSql).FirstOrDefault();


            ////如果是导出Excel，就不分页
            //if (PageSize == -1 && PageNum == -1)
            //{
            //    sql = $@"select ID
            //        ,CountyName
            //        ,NeighborhoodsName
            //        ,CommunityName
            //        ,(case when LTrim(RTrim(ResidenceName)) is null then LTrim(RTrim(RoadName))+LTrim(RTrim(MPNumber))+'号'+LTrim(RTrim(Dormitory)) else  LTrim(RTrim(ResidenceName)) end) PlaceName
            //        ,StandardAddress
            //        ,PropertyOwner
            //        ,CreateTime
            //    from ({sql}) b order by CreateTime desc";
            //}
            ////如果是查询数据，就分页返回
            //else
            //{
            //    sql = $@"select ID
            //        ,CountyName
            //        ,NeighborhoodsName
            //        ,CommunityName
            //        ,(case when LTrim(RTrim(ResidenceName)) is null then LTrim(RTrim(RoadName))+LTrim(RTrim(MPNumber))+'号'+LTrim(RTrim(Dormitory)) else  LTrim(RTrim(ResidenceName)) end) PlaceName
            //        ,StandardAddress
            //        ,PropertyOwner
            //        ,CreateTime
            //    from ({sql}) b 
            //    where RowId between {(PageNum - 1) * PageSize + 1} and {PageNum * PageSize} order by CreateTime desc";
            //}
            //var query = SystemUtils.EFDbContext.Database.SqlQuery<ResidenceMP>(sql).ToList();

            //return new Dictionary<string, object> {
            //       { "Data",query},
            //       { "Count",rowsCount}
            //};
            #endregion
            int count = 0;
            List<ResidenceMPDetails> data = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var q = dbContext.MPOFResidence.Where(t => t.State == UseState);

                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPOfResidence>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var query = q.Where(where.Compile());

                if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "1"))
                {
                    query = query.Where(t => t.CountyID == DistrictID || t.NeighborhoodsID == DistrictID || t.CommunityID == DistrictID);
                }
                if (!string.IsNullOrEmpty(ResidenceName))
                {
                    query = query.Where(t => t.ResidenceName.Contains(ResidenceName));
                }
                if (!string.IsNullOrEmpty(AddressCoding))
                {
                    query = query.Where(t => t.AddressCoding.Contains(AddressCoding));
                }
                if (!string.IsNullOrEmpty(PropertyOwner))
                {
                    query = query.Where(t => t.PropertyOwner.Contains(PropertyOwner));
                }
                if (!string.IsNullOrEmpty(StandardAddress))
                {
                    query = query.Where(t => t.StandardAddress.Contains(StandardAddress));
                }
                count = query.Count();
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    query = query.OrderByDescending(t => t.BZTime).ToList();
                }
                //如果是分页查询，就分页返回
                else
                {
                    query = query.OrderByDescending(t => t.BZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                }

                data = (from t in query
                        join a in SystemUtils.Districts
                        on t.CountyID equals a.ID into aa
                        from at in aa.DefaultIfEmpty()

                        join b in SystemUtils.Districts
                        on t.NeighborhoodsID equals b.ID into bb
                        from bt in bb.DefaultIfEmpty()

                        join c in SystemUtils.Districts
                        on t.CommunityID equals c.ID into cc
                        from ct in cc.DefaultIfEmpty()
                        select new ResidenceMPDetails
                        {
                            ID = t.ID,
                            CountyName = at == null || at.Name == null ? null : at.Name,
                            NeighborhoodsName = bt == null || bt.Name == null ? null : bt.Name,
                            CommunityName = ct == null || ct.Name == null ? null : ct.Name,
                            ResidenceName = t.ResidenceName,
                            StandardAddress = t.StandardAddress,
                            PropertyOwner = t.PropertyOwner,
                            Lat = t.DYPosition == null ? null : t.DYPosition.Latitude,
                            Lng = t.DYPosition == null ? null : t.DYPosition.Longitude,
                            BZTime = t.BZTime,
                            CreateTime = t.CreateTime
                        }).ToList();

                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        /// <summary>
        /// 根据一条住宅门牌数据的ID来查详情
        /// </summary>
        /// <param name="ID"></param>
        public static ResidenceMPDetails SearchResidenceMPByID(string ID)
        {//left join [TopSystemDB].[dbo].[TopCombineRoad] c on a.roadid=c.dmid  ,a.[RoadID]  ,c.biaozhunmingcheng RoadName
            string sql = $@"select a.[ID]
                          ,a.[AddressCoding]
                          ,a.[CountyID]
	                      ,d.Name CountyName
                          ,a.[NeighborhoodsID]
	                      ,e.Name NeighborhoodsName
                          ,a.[CommunityID]
	                      ,f.Name CommunityName
                          ,a.[ResidenceName]
                          ,a.[MPNumber]
                          ,a.[Dormitory]
                          ,a.[LZNumber]
                          ,a.[DYNumber]
                          ,a.[HSNumber]
                          ,a.[DYPosition]
                          ,a.[DYPosition].Lat Lat
                          ,a.[DYPosition].Long Lng
                          ,a.[MPSize]
                          ,a.[Postcode]
                          ,a.[PropertyOwner]
                          ,a.[IDType]
                          ,a.[IDNumber]
                          ,a.[StandardAddress]
                          ,a.[FCZAddress]
                          ,a.[FCZNumber]
                          ,a.[TDZAddress]
                          ,a.[TDZNumber]
                          ,a.[BDCZAddress]
                          ,a.[BDCZNumber]
                          ,a.[HJAddress]
                          ,a.[HJNumber]
                          ,a.[OtherAddress]
                          ,a.[Applicant]
                          ,a.[ApplicantPhone]
                          ,a.[SBDW]
                          ,a.[BZTime]
                          ,a.[CreateTime]
                          ,a.[CreateUser]
                          ,a.[LastModifyTime]
                          ,a.[LastModifyUser]
                          ,a.[State]
                          ,a.[CancelTime]
                          ,a.[CancelUser]
	                      ,ROW_NUMBER() OVER(Order by a.[CreateTime] desc) AS RowId
                      FROM [TopSystemDB].[dbo].[MPOFRESIDENCE] a
                      left join [TopSystemDB].[dbo].[DISTRICT] d on a.CountyID=d.ID
                      left join [TopSystemDB].[dbo].[DISTRICT] e on a.NeighborhoodsID=e.ID
                      left join [TopSystemDB].[dbo].[DISTRICT] f on a.CommunityID=f.ID
                      where a.[State]=1 and a.ID='{ID}'";
            var query = SystemUtils.NewEFDbContext.Database.SqlQuery<ResidenceMPDetails>(sql).FirstOrDefault();

            if (query == null)
                throw new Exception("该门牌已经被注销！");
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                //将附件的名字都加上路径返回
                var files = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == ID);
                if (files.Count() > 0)
                {
                    var FCZ = files.Where(t => t.DocType == Enums.DocType.FCZ);
                    var TDZ = files.Where(t => t.DocType == Enums.DocType.TDZ);
                    var BDCZ = files.Where(t => t.DocType == Enums.DocType.BDCZ);
                    var HJ = files.Where(t => t.DocType == Enums.DocType.HJ);
                    if (FCZ.Count() > 0)
                    {
                        query.FCZ = (from t in FCZ
                                     select new Pictures
                                     {
                                         pid = t.ID,
                                         name = t.Name,
                                         url = "Files/ResidenceMP/" + ID + "/FCZ/" + t.ID + t.FileType
                                     }).ToList();
                    }
                    if (TDZ.Count() > 0)
                    {
                        query.TDZ = (from t in TDZ
                                     select new Pictures
                                     {
                                         pid = t.ID,
                                         name = t.Name,
                                         url = "Files/ResidenceMP/" + ID + "/TDZ/" + t.ID + t.FileType
                                     }).ToList();
                    }
                    if (BDCZ.Count() > 0)
                    {
                        query.BDCZ = (from t in BDCZ
                                      select new Pictures
                                      {
                                          pid = t.ID,
                                          name = t.Name,
                                          url = "Files/ResidenceMP/" + ID + "/BDCZ/" + t.ID + t.FileType
                                      }).ToList();
                    }
                    if (HJ.Count() > 0)
                    {
                        query.HJ = (from t in HJ
                                    select new Pictures
                                    {
                                        pid = t.ID,
                                        name = t.Name,
                                        url = "Files/ResidenceMP/" + ID + "/HJ/" + t.ID + t.FileType
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
        public static MemoryStream ExportResidenceMP(string DistrictID, string ResidenceName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState)
        {
            Dictionary<string, object> dict = SearchResidenceMP(-1, -1, DistrictID, ResidenceName, AddressCoding, PropertyOwner, StandardAddress, UseState);

            int RowCount = int.Parse(dict["Count"].ToString());
            if (RowCount >= 65000)
                throw new Exception("数据量过大，请缩小查询范围后再导出！");
            var Data = dict["Data"] as List<ResidenceMPDetails>;

            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            ws.Name = "住宅门牌";
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
                new ExcelFields() { Field="小区名称",Alias="ResidenceName"},
                new ExcelFields() { Field="标准地址",Alias="StandardAddress"},
                new ExcelFields() { Field="产权人",Alias="PropertyOwner"},
                new ExcelFields() { Field="编制日期",Alias="BZTime"},
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
                    if (field.Field == "编制日期")
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
        #endregion
    }
}