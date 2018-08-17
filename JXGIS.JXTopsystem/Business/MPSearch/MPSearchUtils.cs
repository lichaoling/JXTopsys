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
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Business.MPSearch
{
    public class MPSearchUtils
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
        public static Dictionary<string, object> SearchResidenceMP(int PageSize, int PageNum, string DistrictID, string Name, int UseState)
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
                    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                }
                var query = q.Where(where.Compile());

                if (!string.IsNullOrEmpty(DistrictID))
                {
                    query = query.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    //query = from t in query
                    //        join d in dbContext.Road
                    //        on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                    //        from dt in dd.DefaultIfEmpty()
                    //        where dt.RoadName.Contains(Name) || t.ResidenceName.Contains(Name) || t.Dormitory.Contains(Name)
                    //        select t;
                    query = from t in query
                            where t.ResidenceName.Contains(Name) || t.Dormitory.Contains(Name)
                            select t;
                }
                count = query.Count();
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    query = query.OrderByDescending(t => t.CreateTime).ToList();
                }
                //如果是分页查询，就分页返回
                else
                {
                    query = query.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                }

                //data = (from t in query
                //        join d in dbContext.Road
                //        on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                //        from dt in dd.DefaultIfEmpty()
                //        select new ResidenceMPDetails
                //        {
                //            ID = t.ID,
                //            CountyID = t.CountyID,
                //            NeighborhoodsID = t.NeighborhoodsID,
                //            CommunityID = t.CommunityID,
                //            //PlaceName = dt == null ? (t.ResidenceName) : (dt.RoadName + t.MPNumber + "号" + t.Dormitory),
                //            ResidenceName = t.ResidenceName,
                //            StandardAddress = t.StandardAddress,
                //            PropertyOwner = t.PropertyOwner,
                //            CreateTime = t.CreateTime,
                //            RoadID = t.RoadID,
                //        }).ToList();

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
                            //PlaceName = t.PlaceName,
                            ResidenceName = t.ResidenceName,
                            StandardAddress = t.StandardAddress,
                            PropertyOwner = t.PropertyOwner,
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
        public static MemoryStream ExportResidenceMP(string DistrictID, string Name, int UseState)
        {
            Dictionary<string, object> dict = SearchResidenceMP(-1, -1, DistrictID, Name, UseState);

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
                new ExcelFields() { Field="编制日期",Alias="CreateTime"},
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

        #region 道路门牌
        /// <summary>
        /// 道路门牌查询，根据行政区划ID、名称(道路名称)和门牌号类型
        /// 关联道路表和行政区划表，获取到行政区划名称、道路名称和道路门牌的信息  guid生成出来都是小写的，如果GUID以string形式存储到数据库中全是小写的，但是在数据库中GUID展示出来是大写的
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="Name"></param>
        /// <param name="MPNumberType"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SearchRoadMP(int PageSize, int PageNum, string DistrictID, string Name, int MPNumberType, int UseState)
        {
            int count = 0;
            List<RoadMPDetails> data = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var q = dbContext.MPOfRoad.Where(t => t.State == UseState);

                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPOfRoad>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                }
                var query = q.Where(where.Compile());

                if (!string.IsNullOrEmpty(DistrictID))
                {
                    query = query.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                }
                if (MPNumberType != 0)
                {
                    query = query.Where(t => t.MPNumberType == MPNumberType);
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    query = from t in query
                            join d in dbContext.Road
                            on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                            from dt in dd.DefaultIfEmpty()
                            where dt.RoadName.Contains(Name) || t.ShopName.Contains(Name) || t.ResidenceName.Contains(Name)
                            select t;
                }

                count = query.Count();
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    query = query.OrderByDescending(t => t.CreateTime);
                }
                //如果是分页查询，就分页返回
                else
                {
                    query = query.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize);
                }
                data = (from t in query
                        join d in dbContext.Road
                        on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                        from dt in dd.DefaultIfEmpty()
                        select new RoadMPDetails
                        {
                            ID = t.ID,
                            CountyID = t.CountyID,
                            NeighborhoodsID = t.NeighborhoodsID,
                            CommunityID = t.CommunityID,
                            RoadName = dt.RoadName,
                            MPNumber = t.MPNumber,
                            OriginalNumber = t.OriginalNumber,
                            PropertyOwner = t.PropertyOwner,
                            ShopName = t.ShopName,
                            ReservedNumber = t.ReservedNumber,
                            CreateTime = t.CreateTime
                        }).ToList();
                data = (from t in data
                        join a in SystemUtils.Districts
                        on t.CountyID equals a.ID into aa
                        from at in aa.DefaultIfEmpty()

                        join b in SystemUtils.Districts
                        on t.NeighborhoodsID equals b.ID into bb
                        from bt in bb.DefaultIfEmpty()

                        join c in SystemUtils.Districts
                        on t.CommunityID equals c.ID into cc
                        from ct in cc.DefaultIfEmpty()
                        select new RoadMPDetails
                        {
                            ID = t.ID,
                            CountyName = at == null || at.Name == null ? null : at.Name,
                            NeighborhoodsName = bt == null || bt.Name == null ? null : bt.Name,
                            CommunityName = ct == null || ct.Name == null ? null : ct.Name,
                            RoadName = t.RoadName,
                            MPNumber = t.MPNumber,
                            OriginalNumber = t.OriginalNumber,
                            PropertyOwner = t.PropertyOwner,
                            ShopName = t.ShopName,
                            ReservedNumber = t.ReservedNumber,
                            CreateTime = t.CreateTime
                        }).ToList();

                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        /// <summary>
        /// 根据一条道路门牌数据的ID来查详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static RoadMPDetails SearchRoadMPByID(string ID)
        {
            using (var dbContenxt = SystemUtils.NewEFDbContext)
            {
                string sql = $@"SELECT a.[ID]
                          ,a.[AddressCoding]
                          ,a.[CountyID]
                          ,d.Name CountyName
                          ,a.[NeighborhoodsID]
                          ,e.Name NeighborhoodsName
                          ,a.[CommunityID]
                          ,f.Name CommunityName
                          ,a.[RoadID]
                          ,c.biaozhunmingcheng RoadName
                          ,c.QiDian StartPoint
                          ,c.ZhiDian EndPoint
                          ,a.[ShopName]
                          ,a.[ResidenceName]
                          ,a.[MPRules]
                          ,a.[MPNumberRange]
                          ,a.[MPNumber]
                          ,a.[MPNumberType]
                          ,a.[MPPosition].Lat Lat
                          ,a.[MPPosition].Long Lng
                          ,a.[ReservedNumber]
                          ,a.[OriginalNumber]
                          ,a.[MPSize]
                          ,a.[MPProduce]
                          ,a.[MPMail]
                          ,a.[MailAddress]
                          ,a.[Postcode]
                          ,a.[PropertyOwner]
                          ,a.[IDType]
                          ,a.[IDNumber]
                          ,a.[StandardAddress]
                          ,a.[FCZAddress]
                          ,a.[FCZNumber]
                          ,a.[FCZFile]
                          ,a.[TDZAddress]
                          ,a.[TDZnumber]
                          ,a.[TDZFile]
                          ,a.[YYZZAddress]
                          ,a.[YYZZNumber]
                          ,a.[YYZZFile]
                          ,a.[OtherAddress]
                          ,a.[Applicant]
                          ,a.[ApplicantPhone]
                          ,a.[CreateTime]
                          ,a.[CreateUser]
                          ,a.[LastModifyTime]
                          ,a.[LastModifyUser]
                          ,a.[State]
                          ,a.[CancelTime]
                          ,a.[CancelUser]
                      FROM [TopSystemDB].[dbo].[MPOFROAD] a
                      left join [TopSystemDB].[dbo].[TopCombineRoad] c on a.roadid=c.dmid
                      left join [TopSystemDB].[dbo].[DISTRICT] d on a.CountyID=d.ID
                      left join [TopSystemDB].[dbo].[DISTRICT] e on a.NeighborhoodsID=e.ID
                      left join [TopSystemDB].[dbo].[DISTRICT] f on a.CommunityID=f.ID
                      where a.State=1 and a.ID='{ID}'";
                var query = SystemUtils.NewEFDbContext.Database.SqlQuery<RoadMPDetails>(sql).FirstOrDefault();
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
                        var YYZZ = files.Where(t => t.DocType == Enums.DocType.YYZZ);

                        if (FCZ.Count() > 0)
                        {
                            query.FCZ = (from t in FCZ
                                         select new Pictures
                                         {
                                             pid = t.ID,
                                             name = t.Name,
                                             url = "Files/RoadMP/" + ID + "/FCZ/" + t.ID + t.FileType
                                         }).ToList();
                        }
                        if (TDZ.Count() > 0)
                        {
                            query.TDZ = (from t in TDZ
                                         select new Pictures
                                         {
                                             pid = t.ID,
                                             name = t.Name,
                                             url = "Files/RoadMP/" + ID + "/TDZ/" + t.ID + t.FileType
                                         }).ToList();
                        }
                        if (YYZZ.Count() > 0)
                        {
                            query.YYZZ = (from t in YYZZ
                                          select new Pictures
                                          {
                                              pid = t.ID,
                                              name = t.Name,
                                              url = "Files/RoadMP/" + ID + "/YYZZ/" + t.ID + t.FileType
                                          }).ToList();
                        }
                    }
                    return query;
                }

            }
        }
        /// <summary>
        /// 导出道路门牌Excel
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <param name="Name"></param>
        /// <param name="MPNumberType"></param>
        /// <returns></returns>
        public static MemoryStream ExportRoadMP(string DistrictID, string Name, int MPNumberType, int UseState)
        {
            Dictionary<string, object> dict = SearchRoadMP(-1, -1, DistrictID, Name, MPNumberType, UseState);
            int RowCount = int.Parse(dict["Count"].ToString());
            if (RowCount >= 65000)
                throw new Exception("数据量过大，请缩小查询范围后再导出！");
            var Data = dict["Data"] as List<RoadMPDetails>;
            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            ws.Name = "道路门牌";
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
                new ExcelFields() { Field="道路名称",Alias="RoadName"},
                new ExcelFields() { Field="门牌号码",Alias="MPNumber"},
                new ExcelFields() { Field="原门牌号码",Alias="OriginalNumber"},
                new ExcelFields() { Field="产权人",Alias="PropertyOwner"},
                new ExcelFields() { Field="商铺名称",Alias="ShopName"},
                new ExcelFields() { Field="预留号段",Alias="ReservedNumber"},
                new ExcelFields() { Field="编制日期",Alias="CreateTime"},
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

        #region 农村门牌
        public static Dictionary<string, object> SearchCountryMP(int PageSize, int PageNum, string DistrictID, string Name, int UseState)
        {
            int count = 0;
            List<CountryMPDetails> data = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var q = dbContext.MPOfCountry.Where(t => t.State == UseState);

                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPOfCountry>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                }
                var query = q.Where(where.Compile());

                if (!string.IsNullOrEmpty(DistrictID))
                {
                    query = query.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    query = from t in query
                            where t.ViligeName.Contains(Name)
                            select t;
                }

                count = query.Count();
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    query = query.OrderByDescending(t => t.CreateTime);
                }
                //如果是分页查询，就分页返回
                else
                {
                    query = query.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize);
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
                        select new CountryMPDetails
                        {
                            ID = t.ID,
                            CountyID = t.CountyID,
                            NeighborhoodsID = t.NeighborhoodsID,
                            CommunityID = t.CommunityID,
                            CountyName = at == null || at.Name == null ? null : at.Name,
                            NeighborhoodsName = bt == null || bt.Name == null ? null : bt.Name,
                            CommunityName = ct == null || ct.Name == null ? null : ct.Name,
                            ViligeName = t.ViligeName,
                            MPNumber = t.MPNumber,
                            HSNumber = t.HSNumber,
                            OriginalNumber = t.OriginalNumber,
                            PropertyOwner = t.PropertyOwner,
                            CreateTime = t.CreateTime
                        }).ToList();

                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        public static CountryMPDetails SearchCountryMPByID(string ID)
        {
            using (var dbContenxt = SystemUtils.NewEFDbContext)
            {
                var sql = @"SELECT a.[ID]
                      ,a.[AddressCoding]
                      ,a.[CountyID]
	                  ,d.Name CountyName
                      ,a.[NeighborhoodsID]
                      ,e.Name NeighborhoodsName
                      ,a.[CommunityID]
                      ,f.Name CommunityName
                      ,a.[ViligeName]
                      ,a.[MPNumber]
                      ,a.[MPPosition].Lat Lat
                      ,a.[MPPosition].Long Lng
                      ,a.[OriginalNumber]
                      ,a.[MPSize]
                      ,a.[HSNumber]
                      ,a.[MPProduce]
                      ,a.[MPMail]
                      ,a.[MailAddress]
                      ,a.[Postcode]
                      ,a.[PropertyOwner]
                      ,a.[IDType]
                      ,a.[IDNumber]
                      ,a.[StandardAddress]
                      ,a.[TDZAddress]
                      ,a.[TDZNumber]
                      ,a.[TDZFile]
                      ,a.[QQZAddress]
                      ,a.[QQZNumber]
                      ,a.[QQZFile]
                      ,a.[OtherAddress]
                      ,a.[Applicant]
                      ,a.[ApplicantPhone]
                      ,a.[CreateTime]
                      ,a.[CreateUser]
                      ,a.[LastModifyTime]
                      ,a.[LastModifyUser]
                      ,a.[State]
                      ,a.[CancelTime]
                      ,a.[CancelUser]
                  FROM [TopSystemDB].[dbo].[MPOFCOUNTRY] a
                  left join [TopSystemDB].[dbo].[DISTRICT] d on a.CountyID=d.ID
                  left join [TopSystemDB].[dbo].[DISTRICT] e on a.NeighborhoodsID=e.ID
                  left join [TopSystemDB].[dbo].[DISTRICT] f on a.CommunityID=f.ID
                  where a.State=1";
                var query = SystemUtils.NewEFDbContext.Database.SqlQuery<CountryMPDetails>(sql).FirstOrDefault();
                if (query == null)
                    throw new Exception("该门牌已经被注销！");
                using (var dbContext = SystemUtils.NewEFDbContext)
                {
                    //将附件的名字都加上路径返回
                    var files = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == ID);
                    if (files.Count() > 0)
                    {
                        var TDZ = files.Where(t => t.DocType == Enums.DocType.TDZ);
                        var QQZ = files.Where(t => t.DocType == Enums.DocType.QQZ);

                        if (TDZ.Count() > 0)
                        {
                            query.TDZ = (from t in TDZ
                                         select new Pictures
                                         {
                                             pid = t.ID,
                                             name = t.Name,
                                             url = "Files/CountryMP/" + ID + "/TDZ/" + t.ID + t.FileType
                                         }).ToList();
                        }
                        if (QQZ.Count() > 0)
                        {
                            query.QQZ = (from t in QQZ
                                         select new Pictures
                                         {
                                             pid = t.ID,
                                             name = t.Name,
                                             url = "Files/CountryMP/" + ID + "/QQZ/" + t.ID + t.FileType
                                         }).ToList();
                        }
                    }
                    return query;
                }
            }
        }
        public static MemoryStream ExportCountryMP(string DistrictID, string Name, int UseState)
        {
            Dictionary<string, object> dict = SearchCountryMP(-1, -1, DistrictID, Name, UseState);
            int RowCount = int.Parse(dict["Count"].ToString());
            if (RowCount >= 65000)
                throw new Exception("数据量过大，请缩小查询范围后再导出！");
            var Data = dict["Data"] as List<CountryMPDetails>;
            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            ws.Name = "农村门牌";
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
                new ExcelFields() { Field="自然村名称",Alias="ViligeName"},
                new ExcelFields() { Field="门牌号码",Alias="MPNumber"},
                new ExcelFields() { Field="户室号",Alias="HSNumber"},
                new ExcelFields() { Field="原门牌号码",Alias="OriginalNumber"},
                new ExcelFields() { Field="产权人",Alias="PropertyOwner"},
                new ExcelFields() { Field="编制日期",Alias="CreateTime"},
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