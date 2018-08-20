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
    public class CountryMPSearch
    {
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
                    query = query.OrderByDescending(t => t.BZTime);
                }
                //如果是分页查询，就分页返回
                else
                {
                    query = query.OrderByDescending(t => t.BZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize);
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
            using (var dbContext = SystemUtils.NewEFDbContext)
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
                      ,a.[SBDW]
                      ,a.[BZTime]
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