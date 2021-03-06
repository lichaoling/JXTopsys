﻿using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Controllers;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPSearch
{
    public class RPSearchUtils
    {
        public static Dictionary<string, object> SearchRP(int PageSize, int PageNum, string DistrictID, string CommunityName, string RoadName, string Intersection, string Direction, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, DateTime? start, DateTime? end, int? startCode, int? endCode, int? RepairState, int UseState)
        {
            int count = 0;
            List<RPDetails> data = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {

                var query = dbContext.RP.Where(t => t.State == UseState);

                query = BaseUtils.DataFilterWithTown<RP>(query);

                //行政区划筛选
                if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "嘉兴市"))
                {
                    query = query.Where(t => t.CountyID == DistrictID || t.NeighborhoodsID == DistrictID);
                }

                //社区
                if (!string.IsNullOrEmpty(CommunityName))
                {
                    query = query.Where(t => t.CommunityName == CommunityName);
                }

                //交叉路口筛选
                if (!string.IsNullOrEmpty(Intersection))
                {
                    query = query.Where(t => t.Intersection.Contains(Intersection));
                }

                //方位
                if (!string.IsNullOrEmpty(Direction))
                {
                    query = query.Where(t => t.Direction.Contains(Direction));
                }

                //样式筛选
                if (!string.IsNullOrEmpty(Model))
                {
                    query = query.Where(t => t.Model == Model);
                }
                //规格筛选
                if (!string.IsNullOrEmpty(Size))
                {
                    query = query.Where(t => t.Size == Size);
                }
                //材质筛选
                if (!string.IsNullOrEmpty(Material))
                {
                    query = query.Where(t => t.Material == Material);
                }
                //生产厂家筛选
                if (!string.IsNullOrEmpty(Manufacturers))
                {
                    query = query.Where(t => t.Manufacturers == Manufacturers);
                }
                //正面宣传语筛选
                if (!string.IsNullOrEmpty(FrontTagline))
                {
                    query = query.Where(t => t.FrontTagline.Contains(FrontTagline));
                }
                //反面宣传语筛选
                if (!string.IsNullOrEmpty(BackTagline))
                {
                    query = query.Where(t => t.BackTagline.Contains(BackTagline));
                }
                //设置时间筛选
                //if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                //{
                //    if (!string.IsNullOrEmpty(start))
                //    {
                //        query = query.Where(t => String.Compare(t.BZTime.ToString(), start, StringComparison.Ordinal) >= 0);
                //    }
                //    if (!string.IsNullOrEmpty(end))
                //    {
                //        query = query.Where(t => String.Compare(t.BZTime.ToString(), end, StringComparison.Ordinal) <= 0);
                //    }
                //}
                if (start != null || end != null)
                {
                    if (start != null)
                        query = query.Where(t => t.BZTime >= start);
                    if (end != null)
                        query = query.Where(t => t.BZTime <= end);
                }
                //道路名称筛选
                if (!string.IsNullOrEmpty(RoadName))
                {
                    query = query.Where(t => t.RoadName.Contains(RoadName));
                }

                //起止二维码编码删选
                if (startCode != null || endCode != null)
                {
                    if (startCode != null)
                        query = query.Where(t => t.Code >= startCode);
                    if (endCode != null)
                        query = query.Where(t => t.Code <= endCode);
                }

                if (RepairState == 0)//未修复
                {
                    query = query.Where(t => t.FinishRepaire == 0);
                }
                else if (RepairState == 1)//已修复
                {
                    query = query.Where(t => t.RepairedCount > 0).Where(t => t.FinishRepaire == 1);
                }
                else if (RepairState == 2)//完好
                {
                    query = query.Where(t => t.RepairedCount == 0);
                }


                count = query.Count();

                List<RP> data1;
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    data1 = query.OrderBy(t => t.NeighborhoodsID).ThenBy(t => t.RoadName).ThenBy(t => t.Code).ToList();
                }
                //如果是分页查询，就分页返回
                else
                {
                    data1 = query.OrderBy(t => t.NeighborhoodsID).ThenBy(t => t.RoadName).ThenBy(t => t.Code).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                }

                data = (from t in data1
                        select new RPDetails
                        {
                            ID = t.ID,
                            Code = t.Code,
                            AddressCoding = t.AddressCoding,
                            CountyID = t.CountyID,
                            NeighborhoodsID = t.NeighborhoodsID,
                            CountyName = string.IsNullOrEmpty(t.CountyID) ? null : t.CountyID.Split('.').Last(),
                            NeighborhoodsName = string.IsNullOrEmpty(t.NeighborhoodsID) ? null : t.NeighborhoodsID.Split('.').Last(),
                            CommunityName = t.CommunityName,
                            RoadName = t.RoadName,
                            Intersection = t.Intersection,
                            Direction = t.Direction,
                            RoadStart=t.RoadStart,
                            RoadEnd=t.RoadEnd,
                            BZRules = t.BZRules,
                            StartEndNum = t.StartEndNum,
                            Model = t.Model,
                            Size = t.Size,
                            Material = t.Material,
                            Manufacturers = t.Manufacturers,
                            FrontTagline = t.FrontTagline,
                            BackTagline = t.BackTagline,
                            Management = t.Management,
                            BZTime = t.BZTime,
                            CreateTime = t.CreateTime,
                            RepairedCount = t.RepairedCount,
                            PositionX = t.PositionX,
                            PositionY = t.PositionY
                        }).ToList();


                var baseUrl_QRCode = StaticVariable.RPQRCodeRelativePath;
                //关联路牌照片 重组url
                List<RPDetails> rt = new List<RPDetails>();
                foreach (var d in data)
                {
                    var baseUrl = Path.Combine(StaticVariable.RPBZPhotoRelativePath, d.ID);
                    var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == d.ID);
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
                        d.RPBZPhoto = filelst;
                    }
                    d.CodeFile = new Pictures()
                    {
                        RelativePath = baseUrl_QRCode + "/" + d.Code + ".jpg",
                        TRelativePath = baseUrl_QRCode + "/t-" + d.Code + ".jpg",
                    };

                    rt.Add(d);
                }

                return new Dictionary<string, object> {
                   { "Data",rt},
                   { "Count",count}
                };
            }
        }

        public static RPDetails SearchRPByID(string RPID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var rp = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == RPID).FirstOrDefault();
                if (rp == null)
                    throw new Error("该路牌已经被注销！");
                var data = new RPDetails(rp);

                var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == RPID);
                var baseUrl_QRCode = StaticVariable.RPQRCodeRelativePath;
                if (files.Count() > 0)
                {
                    var baseUrl_BZ = Path.Combine(StaticVariable.RPBZPhotoRelativePath, RPID);

                    var filelst = (from t in files
                                   select new Pictures
                                   {
                                       FileID = t.ID,
                                       Name = t.Name,
                                       RelativePath = baseUrl_BZ + "/" + t.ID + t.FileEx,
                                       TRelativePath = baseUrl_BZ + "/t-" + t.ID + t.FileEx
                                   }).ToList();
                    data.RPBZPhoto = filelst;
                }
                data.CodeFile = new Pictures()
                {
                    RelativePath = baseUrl_QRCode + "/" + data.Code + ".jpg",
                    TRelativePath = baseUrl_QRCode + "/t-" + data.Code + ".jpg",
                };
                data.CountyName = data.CountyID.Split('.').Last();
                data.NeighborhoodsName = data.NeighborhoodsID.Split('.').Last();
                data.PositionX = data.PositionX;
                data.PositionY = data.PositionY;
                return data;
            }
        }

        public static MemoryStream ExportRP(string DistrictID, string CommunityName, string RoadName, string Intersection, string Direction, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, DateTime? start, DateTime? end, int? startCode, int? endCode, int? RepairState, int UseState)
        {
            Dictionary<string, object> dict = SearchRP(-1, -1, DistrictID, CommunityName, RoadName, Intersection, Direction, Model, Size, Material, Manufacturers, FrontTagline, BackTagline, start, end, startCode, endCode, RepairState, UseState);

            int RowCount = int.Parse(dict["Count"].ToString());
            if (RowCount >= 65000)
                throw new Error("数据量过大，请缩小查询范围后再导出！");
            var Data = dict["Data"] as List<RPDetails>;

            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            ws.Name = Enums.MPTypeCh.Residence;
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
                new ExcelFields() { Field="二维码编号",Alias="Code"},
                new ExcelFields() { Field="地址编码",Alias="AddressCoding"},
                new ExcelFields() { Field="市辖区",Alias="CountyName"},
                new ExcelFields() { Field="镇街道",Alias="NeighborhoodsName"},
                new ExcelFields() { Field="村社区",Alias="CommunityName"},
                new ExcelFields() { Field="道路名称",Alias="RoadName"},
                new ExcelFields() { Field="交叉口",Alias="Intersection"},
                new ExcelFields() { Field="方位",Alias="Direction"},
                new ExcelFields() { Field="道路起点",Alias="RoadStart"},
                new ExcelFields() { Field="道路讫点",Alias="RoadEnd"},
                new ExcelFields() { Field="编制规则",Alias="BZRules"},
                new ExcelFields() { Field="起止号码",Alias="StartEndNum"},
                new ExcelFields() { Field="样式",Alias="Model"},
                new ExcelFields() { Field="规格",Alias="Size"},
                new ExcelFields() { Field="材质",Alias="Material"},
                new ExcelFields() { Field="生产厂家",Alias="Manufacturers"},
                new ExcelFields() { Field="正面宣传语",Alias="FrontTagline"},
                new ExcelFields() { Field="反面宣传语",Alias="BackTagline"},
                new ExcelFields() { Field="管理单位",Alias="Management"},
                new ExcelFields() { Field="维修次数",Alias="RepairedCount"},
                new ExcelFields() { Field="编制时间",Alias="BZTime"},
                new ExcelFields() { Field="纬度",Alias="Lat"},
                new ExcelFields() { Field="经度",Alias="Lng"},
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
                    if (field.Field == "编制时间")
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