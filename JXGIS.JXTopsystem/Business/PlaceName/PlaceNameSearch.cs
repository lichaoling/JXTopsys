using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.PlaceName
{
    public class PlaceNameSearch
    {
        /// <summary>
        /// 住宅门牌查询，根据行政区划ID和名称（小区名或道路名或宿舍名）
        /// 关联道路表和行政区划表，获取到行政区划的名称、道路名称和住宅门牌的信息
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SearchResidenceMP(int PageSize, int PageNum, string DistrictID, string CommunityName, string Postcode)
        {
            int count = 0;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.PlaceName.Where(t => t.State == 1);
                query = BaseUtils.DataFilterWithTown<Models.Entities.PlaceName>(query);
                if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "嘉兴市"))
                {
                    query = query.Where(t => t.CountyID == DistrictID || t.NeighborhoodsID == DistrictID);
                }
                if (!string.IsNullOrEmpty(CommunityName))
                {
                    query = query.Where(t => t.CommunityName == CommunityName);
                }
                if (!string.IsNullOrEmpty(Postcode))
                {
                    query = query.Where(t => t.Postcode == Postcode);
                }
                count = query.Count();
                List<Models.Entities.PlaceName> result;
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
        public static Models.Entities.PlaceName SearchPlaceNameByID(string ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = (from t in dbContext.PlaceName
                             where t.State == Enums.UseState.Enable && t.ID == ID
                             select t).FirstOrDefault();
                if (query == null)
                    throw new Exception("该地名已经被注销！");
             
                //将附件的名字都加上路径返回
                var files = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == MPID);
                if (files.Count() > 0)
                {
                    //var FCZ = files.Where(t => t.DocType == Enums.DocType.FCZ);
                    //var TDZ = files.Where(t => t.DocType == Enums.DocType.TDZ);
                    //var BDCZ = files.Where(t => t.DocType == Enums.DocType.BDCZ);
                    //var HJ = files.Where(t => t.DocType == Enums.DocType.HJ);
                    //var SQB = files.Where(t => t.DocType == Enums.DocType.SQB);

                    //var baseUrl = Path.Combine(StaticVariable.residenceMPRelativePath, MPID);
                    //if (FCZ.Count() > 0)
                    //{
                    //    query.FCZ = (from t in FCZ
                    //                 select new Pictures
                    //                 {
                    //                     FileID = t.ID,
                    //                     Name = t.Name,
                    //                     RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                    //                     TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                    //                 }).ToList();
                    //}
                    //if (TDZ.Count() > 0)
                    //{
                    //    query.TDZ = (from t in TDZ
                    //                 select new Pictures
                    //                 {
                    //                     FileID = t.ID,
                    //                     Name = t.Name,
                    //                     RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                    //                     TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                    //                 }).ToList();
                    //}
                    //if (BDCZ.Count() > 0)
                    //{
                    //    query.BDCZ = (from t in BDCZ
                    //                  select new Pictures
                    //                  {
                    //                      FileID = t.ID,
                    //                      Name = t.Name,
                    //                      RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                    //                      TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                    //                  }).ToList();
                    //}
                    //if (HJ.Count() > 0)
                    //{
                    //    query.HJ = (from t in HJ
                    //                select new Pictures
                    //                {
                    //                    FileID = t.ID,
                    //                    Name = t.Name,
                    //                    RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                    //                    TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                    //                }).ToList();
                    //}
                    //if (SQB.Count() > 0)
                    //{
                    //    query.SQB = (from t in SQB
                    //                 select new Pictures
                    //                 {
                    //                     FileID = t.ID,
                    //                     Name = t.Name,
                    //                     RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                    //                 }).ToList();
                    //}
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
        public static MemoryStream ExportResidenceMP(string DistrictID, string CommunityName, string ResidenceName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState)
        {
            Dictionary<string, object> dict = SearchResidenceMP(-1, -1, DistrictID, CommunityName, ResidenceName, AddressCoding, PropertyOwner, StandardAddress, UseState);

            int RowCount = int.Parse(dict["Count"].ToString());
            if (RowCount >= 65000)
                throw new Exception("数据量过大，请缩小查询范围后再导出！");
            var Data = dict["Data"] as List<ResidenceMPDetails>;

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
                new ExcelFields() { Field="地址编码",Alias="AddressCoding"},
                new ExcelFields() { Field="市辖区",Alias="CountyName"},
                new ExcelFields() { Field="镇街道",Alias="NeighborhoodsName"},
                new ExcelFields() { Field="村社区",Alias="CommunityName"},
                new ExcelFields() { Field="小区名称",Alias="ResidenceName"},
                new ExcelFields() { Field="门牌号码",Alias="MPNumber"},
                new ExcelFields() { Field="宿舍名",Alias="Dormitory"},
                new ExcelFields() { Field="幢号",Alias="LZNumber"},
                new ExcelFields() { Field="单元号",Alias="DYNumber"},
                new ExcelFields() { Field="户室号",Alias="HSNumber"},
                new ExcelFields() { Field="门牌规格",Alias="MPSize"},
                new ExcelFields() { Field="邮政编码",Alias="Postcode"},
                new ExcelFields() { Field="产权人",Alias="PropertyOwner"},
                new ExcelFields() { Field="证件类型",Alias="IDType"},
                new ExcelFields() { Field="证件号",Alias="IDNumber"},
                new ExcelFields() { Field="房产证地址",Alias="FCZAddress"},
                new ExcelFields() { Field="房产证号",Alias="FCZNumber"},
                new ExcelFields() { Field="土地证地址",Alias="TDZAddress"},
                new ExcelFields() { Field="土地证号",Alias="TDZNumber"},
                new ExcelFields() { Field="不动产证地址",Alias="BDCZAddress"},
                new ExcelFields() { Field="不动产证号",Alias="BDCZNumber"},
                new ExcelFields() { Field="户籍地址",Alias="HJAddress"},
                new ExcelFields() { Field="户籍号",Alias="HJNumber"},
                new ExcelFields() { Field="其他地址",Alias="OtherAddress"},
                new ExcelFields() { Field="申请人",Alias="Applicant"},
                new ExcelFields() { Field="联系电话",Alias="ApplicantPhone"},
                new ExcelFields() { Field="申办单位",Alias="SBDW"},
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