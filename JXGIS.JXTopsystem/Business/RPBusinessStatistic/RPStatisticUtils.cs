using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPBusinessStatistic
{
    public class RPStatisticUtils
    {
        /// <summary>
        /// 路牌数量统计
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="DistrictID"></param>
        /// <param name="CommunityName"></param>
        /// <param name="RoadName"></param>
        /// <param name="Model"></param>
        /// <param name="Material"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetRPNumTJ(int PageSize, int PageNum, DateTime? start, DateTime? end, string DistrictID, string CommunityName, string RoadName, string Model, string Manufacturers, string Material, string Size)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable);

                if (start != null || end != null)
                {
                    if (start != null)
                        query = query.Where(t => t.BZTime >= start);
                    if (end != null)
                        query = query.Where(t => t.BZTime <= end);
                }

                if (!string.IsNullOrEmpty(DistrictID))
                    query = query.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID);

                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);

                if (!string.IsNullOrEmpty(RoadName))
                    query = query.Where(t => t.RoadName == RoadName);

                if (!string.IsNullOrEmpty(Model))
                    query = query.Where(t => t.Model == Model);

                if (!string.IsNullOrEmpty(Manufacturers))
                    query = query.Where(t => t.Manufacturers == Manufacturers);

                if (!string.IsNullOrEmpty(Material))
                    query = query.Where(t => t.Material == Material);

                if (!string.IsNullOrEmpty(Size))
                    query = query.Where(t => t.Size == Size);

                var re = from t in query
                         group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName, t.RoadName, t.Model, t.Material, t.Size } into g
                         select new
                         {
                             CountyID = g.Key.CountyID,
                             NeighborhoodsID = g.Key.NeighborhoodsID,
                             CommunityName = g.Key.CommunityName,
                             RoadName = g.Key.RoadName,
                             Model = g.Key.Model,
                             Material = g.Key.Material,
                             Size = g.Key.Size,
                             Count = g.Count(),
                         };
                var count = re.Count();
                var totalCount = re.Sum(t => t.Count);
                var result = re.OrderBy(t => t.NeighborhoodsID).ThenBy(t => t.CommunityName).ThenBy(t => t.RoadName).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = from t in result
                           select new
                           {
                               CountyID = t.CountyID,
                               CountyName = !string.IsNullOrEmpty(t.CountyID) ? t.CountyID.Split('.').Last() : null,
                               NeighborhoodsID = t.NeighborhoodsID,
                               NeighborhoodsName = !string.IsNullOrEmpty(t.NeighborhoodsID) ? t.NeighborhoodsID.Split('.').Last() : null,
                               CommunityName = t.CommunityName,
                               RoadName = t.RoadName,
                               Model = t.Model,
                               Material = t.Material,
                               Size = t.Size,
                               Count = t.Count,
                           };
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count},
                   { "TotalCount",totalCount}
                };

            }
        }

        /// <summary>
        /// 路牌维护统计
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="CommunityName"></param>
        /// <param name="RepairMode"></param>
        /// <param name="RepairedCount"></param>
        /// <param name="RepairParts"></param>
        /// <param name="RepairContent"></param>
        /// <param name="RepairFactory"></param>
        /// <param name="isFinishRepair"></param>
        /// <param name="FinishTimeStart"></param>
        /// <param name="FinishTimeEnd"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetRPRepairTJ(int PageSize, int PageNum, string DistrictID, string CommunityName, string RepairMode, string BXFS, int RepairedCount, string RepairParts, string RepairContent, string RepairFactory, int isFinishRepair, DateTime? FinishTimeStart, DateTime? FinishTimeEnd)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                IQueryable<Models.Entities.RPRepair> query = dbContext.RPRepair.Where(t => true);
                if (!string.IsNullOrEmpty(RepairMode))
                {
                    query = query.Where(t => t.RepairMode == RepairMode);
                }
                if (!string.IsNullOrEmpty(BXFS))
                {
                    query = query.Where(t => t.BXFS == BXFS);
                }
                if (!string.IsNullOrEmpty(RepairParts))
                {
                    query = query.Where(t => t.RepairParts == RepairParts);
                }
                if (!string.IsNullOrEmpty(RepairContent))
                {
                    query = query.Where(t => t.RepairContent == RepairContent);
                }
                if (!string.IsNullOrEmpty(RepairFactory))
                {
                    query = query.Where(t => t.RepairFactory == RepairFactory);
                }
                if (isFinishRepair == Enums.Complete.Yes)//已修复,有修复的起止时间
                {
                    query = query.Where(t => t.FinishRepaireTime != null);
                    if (FinishTimeStart != null)
                    {
                        var date = FinishTimeStart;
                        query = query.Where(t => t.FinishRepaireTime >= date);
                    }
                    if (FinishTimeEnd != null)
                    {
                        var date = FinishTimeEnd.Value.AddDays(1);
                        query = query.Where(t => t.FinishRepaireTime <= date);
                    }
                }
                else if (isFinishRepair == Enums.Complete.NO)//未修复
                {
                    query = query.Where(t => t.FinishRepaireTime == null);
                }
                var rpID = query.Select(t => t.RPID).Distinct().ToList();
                var rps = dbContext.RP.Where(t => rpID.Contains(t.ID));

                if (RepairedCount != -1)
                    rps = rps.Where(t => t.RepairedCount == RepairedCount);

                if (!string.IsNullOrEmpty(DistrictID))
                    rps = rps.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID);
                if (!string.IsNullOrEmpty(CommunityName))
                    rps = rps.Where(t => t.CommunityName == CommunityName);


                var count = rps.Count();
                var data = new List<RP>();

                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    data = rps.OrderByDescending(t => t.FinishRepaireTime).ThenBy(t => t.RoadName).ToList();
                }
                //如果是分页查询，就分页返回
                else
                {
                    data = rps.OrderByDescending(t => t.FinishRepaireTime).ThenBy(t => t.RoadName).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                }

                var result = (from t in data
                              select new RPDetails
                              {
                                  ID = t.ID,
                                  CountyID = t.CountyID,
                                  NeighborhoodsID = t.NeighborhoodsID,
                                  CountyName = t.CountyID.Split('.').Last(),
                                  NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                                  CommunityName = t.CommunityName,
                                  RoadName = t.RoadName,
                                  Intersection = t.Intersection,
                                  Direction = t.Direction,
                                  RoadStart = t.RoadStart,
                                  RoadEnd = t.RoadEnd,
                                  BZTime = t.BZTime,
                                  CreateTime = t.CreateTime,
                                  RepairedCount = t.RepairedCount,
                                  FinishRepaireTime = t.FinishRepaireTime,
                                  PositionX = t.PositionX,
                                  PositionY = t.PositionY,

                                  RepairParts = dbContext.RPRepair.Where(s => s.RPID == t.ID).OrderByDescending(s => s.RepairTime).Select(s => s.RepairParts).FirstOrDefault(),
                                  RepairContent = dbContext.RPRepair.Where(s => s.RPID == t.ID).OrderByDescending(s => s.RepairTime).Select(s => s.RepairContent).FirstOrDefault(),
                                  RepairTime = dbContext.RPRepair.Where(s => s.RPID == t.ID).OrderByDescending(s => s.RepairTime).Select(s => s.RepairTime).FirstOrDefault(),

                              }).ToList();
                //关联路牌照片 重组url
                List<RPDetails> rt = new List<RPDetails>();
                foreach (var r in result)
                {
                    var baseUrl = Path.Combine(StaticVariable.RPBZPhotoRelativePath, r.ID);
                    var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == r.ID);
                    if (files.Count() > 0)
                    {
                        var filelst = (from t in files
                                       select new Pictures
                                       {
                                           FileID = t.ID,
                                           Name = t.Name,
                                           RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                                           TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                                       }).ToList();
                        r.RPBZPhoto = filelst;
                    }
                    rt.Add(r);
                }

                return new Dictionary<string, object> {
                   { "Data",rt},
                   { "Count",count}
                };

            }
        }

        public static MemoryStream ExportRPRepairTJ(string DistrictID, string CommunityName, string RepairMode, string BXFS, int RepairedCount, string RepairParts, string RepairContent, string RepairFactory, int isFinishRepair, DateTime? FinishTimeStart, DateTime? FinishTimeEnd)
        {
            Dictionary<string, object> dict = GetRPRepairTJ(-1, -1, DistrictID, CommunityName, RepairMode, BXFS, RepairedCount, RepairParts, RepairContent, RepairFactory, isFinishRepair, FinishTimeStart, FinishTimeEnd);
            int RowCount = int.Parse(dict["Count"].ToString());
            if (RowCount >= 65000)
                throw new Error("数据量过大，请缩小查询范围后再导出！");
            var Data = dict["Data"] as List<RPDetails>;
            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            ws.Name = "路牌";
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
                new ExcelFields() { Field="行政区",Alias="CountyName"},
                new ExcelFields() { Field="镇街道",Alias="NeighborhoodsName"},
                new ExcelFields() { Field="村社区",Alias="CommunityName"},
                new ExcelFields() { Field="道路名称",Alias="RoadName"},
                new ExcelFields() { Field="设置路口",Alias="Intersection"},
                new ExcelFields() { Field="设置方位",Alias="Direction"},
                new ExcelFields() { Field="设置时间",Alias="BZTime"},
                new ExcelFields() { Field="维修次数",Alias="RepairedCount"},
                new ExcelFields() { Field="纬度",Alias="Lat"},
                new ExcelFields() { Field="经度",Alias="Lng"},
                new ExcelFields() { Field="维修部位",Alias="RepairParts"},
                new ExcelFields() { Field="维修内容",Alias="RepairContent"},
                new ExcelFields() { Field="报修时间",Alias="RepairTime"},
                new ExcelFields() { Field="修复时间",Alias="FinishRepaireTime"},
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
                    if (field.Field == "设置时间" || field.Field == "报修时间" || field.Field == "修复时间")
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
    }
}