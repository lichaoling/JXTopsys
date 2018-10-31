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
        public static Dictionary<string, object> SearchCountryMP(int PageSize, int PageNum, string DistrictID, string CommunityName, string ViligeName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState)
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
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var query = q.Where(where.Compile());

                if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "1"))
                {
                    query = query.Where(t => t.CountyID == DistrictID || t.NeighborhoodsID == DistrictID);
                }
                if (!string.IsNullOrEmpty(CommunityName))
                {
                    query = query.Where(t => t.CommunityName == CommunityName);
                }
                if (!string.IsNullOrEmpty(ViligeName))
                {
                    query = query.Where(t => t.ViligeName.Contains(ViligeName));
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
                        select new CountryMPDetails
                        {
                            ID = t.ID,
                            AddressCoding = t.AddressCoding,
                            CountyID = t.CountyID,
                            CountyName = t.CountyID.Split('.').Last(),
                            NeighborhoodsID = t.NeighborhoodsID,
                            NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                            CommunityName = t.CommunityName,
                            ViligeName = t.ViligeName,
                            MPNumber = t.MPNumber,
                            OriginalMPAddress = t.OriginalMPAddress,
                            MPSize = t.MPSize,
                            HSNumber = t.HSNumber,
                            AddType = t.AddType,
                            MPProduce = t.MPProduce,
                            MPProduceComplete = t.MPProduceComplete,
                            MPProduceCompleteTime = t.MPProduceCompleteTime,
                            PLID = t.PLID,
                            MPMail = t.MPMail,
                            MailAddress = t.MailAddress,
                            Postcode = t.Postcode,
                            PropertyOwner = t.PropertyOwner,
                            IDType = t.IDType,
                            IDNumber = t.IDNumber,
                            StandardAddress = t.StandardAddress,
                            TDZAddress = t.TDZAddress,
                            TDZNumber = t.TDZNumber,
                            QQZAddress = t.QQZAddress,
                            QQZNumber = t.QQZNumber,
                            OtherAddress = t.OtherAddress,
                            Applicant = t.Applicant,
                            ApplicantPhone = t.ApplicantPhone,
                            SBDW = t.SBDW,
                            BZTime = t.BZTime,
                            Lat = t.MPPosition == null ? null : t.MPPosition.Latitude,
                            Lng = t.MPPosition == null ? null : t.MPPosition.Longitude
                        }).ToList();

                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        public static CountryMPDetails SearchCountryMPByID(string MPID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = (from t in dbContext.MPOfCountry
                             where t.State == Enums.UseState.Enable && t.ID == MPID
                             select new CountryMPDetails
                             {
                                 ID = t.ID,
                                 AddressCoding = t.AddressCoding,
                                 CountyID = t.CountyID,
                                 NeighborhoodsID = t.NeighborhoodsID,
                                 CommunityName = t.CommunityName,
                                 ViligeName = t.ViligeName,
                                 MPNumber = t.MPNumber,
                                 OriginalMPAddress = t.OriginalMPAddress,
                                 MPSize = t.MPSize,
                                 HSNumber = t.HSNumber,
                                 AddType = t.AddType,
                                 MPProduce = t.MPProduce,
                                 MPProduceComplete = t.MPProduceComplete,
                                 MPProduceCompleteTime = t.MPProduceCompleteTime,
                                 PLID=t.PLID,
                                 MPMail = t.MPMail,
                                 MailAddress = t.MailAddress,
                                 Postcode = t.Postcode,
                                 PropertyOwner = t.PropertyOwner,
                                 IDType = t.IDType,
                                 IDNumber = t.IDNumber,
                                 StandardAddress = t.StandardAddress,
                                 TDZAddress = t.TDZAddress,
                                 TDZNumber = t.TDZNumber,
                                 QQZAddress = t.QQZAddress,
                                 QQZNumber = t.QQZNumber,
                                 OtherAddress = t.OtherAddress,
                                 Applicant = t.Applicant,
                                 ApplicantPhone = t.ApplicantPhone,
                                 SBDW = t.SBDW,
                                 BZTime = t.BZTime,
                                 Lat = t.MPPosition == null ? null : t.MPPosition.Latitude,
                                 Lng = t.MPPosition == null ? null : t.MPPosition.Longitude
                             }).FirstOrDefault();
                if (query == null)
                    throw new Exception("该门牌已经被注销！");
                query.NeighborhoodsName = query.NeighborhoodsID.Split('.').Last();
                query.CountyName = query.CountyID.Split('.').Last();

                //将附件的名字都加上路径返回
                var files = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == MPID);
                if (files.Count() > 0)
                {
                    var TDZ = files.Where(t => t.DocType == Enums.DocType.TDZ);
                    var QQZ = files.Where(t => t.DocType == Enums.DocType.QQZ);
                    var baseUrl = Path.Combine("Files", Enums.TypeStr.MP, Enums.MPFileType.CountryMP, MPID);

                    if (TDZ.Count() > 0)
                    {
                        query.TDZ = (from t in TDZ
                                     select new Pictures
                                     {
                                         FileID = t.ID,
                                         Name = t.Name,
                                         RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                                         TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                                     }).ToList();
                    }
                    if (QQZ.Count() > 0)
                    {
                        query.QQZ = (from t in QQZ
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
        public static MemoryStream ExportCountryMP(string DistrictID, string CommunityName, string ViligeName, string AddressCoding, string PropertyOwner, string StandardAddress, int UseState)
        {
            Dictionary<string, object> dict = SearchCountryMP(-1, -1, DistrictID, CommunityName, ViligeName, AddressCoding, PropertyOwner, StandardAddress, UseState);
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
                new ExcelFields() { Field="地址编码",Alias="AddressCoding"},
                new ExcelFields() { Field="市辖区",Alias="CountyName"},
                new ExcelFields() { Field="镇街道",Alias="NeighborhoodsName"},
                new ExcelFields() { Field="村社区",Alias="CommunityName"},
                new ExcelFields() { Field="自然村名称",Alias="ViligeName"},
                new ExcelFields() { Field="门牌号",Alias="MPNumber"},
                new ExcelFields() { Field="原门牌地址",Alias="OriginalMPAddress"},
                new ExcelFields() { Field="门牌规格",Alias="MPSize"},
                new ExcelFields() { Field="户室号",Alias="HSNumber"},
                new ExcelFields() { Field="邮寄地址",Alias="MailAddress"},
                new ExcelFields() { Field="邮政编码",Alias="MailAddress"},
                new ExcelFields() { Field="产权人",Alias="PropertyOwner"},
                new ExcelFields() { Field="证件类型",Alias="IDType"},
                new ExcelFields() { Field="证件号",Alias="IDNumber"},
                new ExcelFields() { Field="土地证地址",Alias="TDZAddress"},
                new ExcelFields() { Field="土地证号",Alias="TDZNumber"},
                new ExcelFields() { Field="确权证地址",Alias="QQZAddress"},
                new ExcelFields() { Field="确权证号",Alias="QQZNumber"},
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
        #endregion
    }
}