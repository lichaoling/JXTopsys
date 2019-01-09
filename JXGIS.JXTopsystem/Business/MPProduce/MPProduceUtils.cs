﻿using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace JXGIS.JXTopsystem.Business.MPProduce
{
    public class MPProduceUtils
    {
        /// <summary>
        /// 获取已经制作的零星门牌
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="MPProduceComplete"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetProducedLXMP(int PageSize, int PageNum, string MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                IQueryable<ProducedLXMPList> result;
                if (MPType == Enums.MPTypeCh.Road)
                {
                    var mpOfRoad = BaseUtils.DataFilterWithTown<MPOfRoad>(dbContext.MPOfRoad);
                    result = from t in mpOfRoad
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.LXProduceID)
                             group t by new { t.LXProduceID, t.MPProduceUser, t.MPProduceTime } into g
                             select new ProducedLXMPList
                             {
                                 MPType = Enums.MPTypeCh.Road,
                                 LXProduceID = g.Key.LXProduceID,
                                 MPProduceUser = g.Key.MPProduceUser,
                                 MPProduceTime = g.Key.MPProduceTime
                             };
                }
                else if (MPType == Enums.MPTypeCh.Country)
                {
                    var mpOfCountry = BaseUtils.DataFilterWithTown<MPOfCountry>(dbContext.MPOfCountry);
                    result = from t in mpOfCountry
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.LXProduceID)
                             group t by new { t.LXProduceID, t.MPProduceUser, t.MPProduceTime } into g
                             select new ProducedLXMPList
                             {
                                 MPType = Enums.MPTypeCh.Country,
                                 LXProduceID = g.Key.LXProduceID,
                                 MPProduceUser = g.Key.MPProduceUser,
                                 MPProduceTime = g.Key.MPProduceTime
                             };
                }
                else
                    throw new Exception("未知的错误类型！");
                count = result.Count();
                var data = result.OrderBy(t => t.MPProduceTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        /// <summary>
        /// 获取未制作的零星门牌
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetNotProducedLXMP(int PageSize, int PageNum, string MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                IQueryable<NotProducedLXMPList> result;
                if (MPType == Enums.MPTypeCh.Road)
                {
                    var roadMPProduce = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => string.IsNullOrEmpty(t.LXProduceID));
                    result = from a in roadMPProduce
                             select new NotProducedLXMPList
                             {
                                 MPID = a.ID,
                                 CountyID = a.CountyID,
                                 NeighborhoodsID = a.NeighborhoodsID,
                                 CommunityName = a.CommunityName,
                                 MPType = Enums.TypeInt.Road,
                                 MPTypeName = Enums.MPTypeCh.Road,
                                 PlaceName = a.RoadName,
                                 MPNumber = a.MPNumber,
                                 MPSize = a.MPSize,
                                 Postcode = a.Postcode,
                                 MPBZTime = a.BZTime
                             };
                    result = BaseUtils.DataFilterWithTown<NotProducedLXMPList>(result);
                }
                else if (MPType == Enums.MPTypeCh.Country)
                {
                    var countryMPProduce = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => string.IsNullOrEmpty(t.LXProduceID));
                    result = from b in countryMPProduce
                             select new NotProducedLXMPList
                             {
                                 MPID = b.ID,
                                 CountyID = b.CountyID,
                                 NeighborhoodsID = b.NeighborhoodsID,
                                 CommunityName = b.CommunityName,
                                 MPType = Enums.TypeInt.Country,
                                 MPTypeName = Enums.MPTypeCh.Country,
                                 PlaceName = b.ViligeName,
                                 MPNumber = b.MPNumber,
                                 MPSize = b.MPSize,
                                 Postcode = b.Postcode,
                                 MPBZTime = b.BZTime
                             };
                    result = BaseUtils.DataFilterWithTown<NotProducedLXMPList>(result);
                }
                else
                    throw new Exception("未知的错误类型！");

                count = result.Count();
                var data = result.OrderByDescending(t => t.MPBZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                data = (from t in data
                        select new NotProducedLXMPList
                        {
                            MPID = t.MPID,
                            CountyID = t.CountyID,
                            CountyName = !string.IsNullOrEmpty(t.CountyID) ? t.CountyID.Split('.').Last() : null,
                            NeighborhoodsID = t.NeighborhoodsID,
                            NeighborhoodsName = !string.IsNullOrEmpty(t.NeighborhoodsID) ? t.NeighborhoodsID.Split('.').Last() : null,
                            CommunityName = t.CommunityName,
                            MPType = Enums.TypeInt.Country,
                            MPTypeName = Enums.MPTypeCh.Country,
                            PlaceName = t.PlaceName,
                            MPNumber = t.MPNumber,
                            MPSize = t.MPSize,
                            Postcode = t.Postcode,
                            MPBZTime = t.MPBZTime
                        }).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        /// <summary>
        /// 批量选择零星增加的需制作但未制作的门牌，进行批量制作
        /// </summary>
        /// <param name="mpLists"></param>
        public static MemoryStream ProduceLXMP(List<string> MPIDs, string MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<LXMPHZ> lxmphzs = new List<LXMPHZ>();
                var LXProduceID = DateTime.Now.ToString("yyyyMMddhhmmss");

                if (MPType == Enums.MPTypeCh.Road)
                {
                    foreach (var mpid in MPIDs)
                    {
                        LXMPHZ lxmphz = new LXMPHZ();
                        var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.ID == mpid).FirstOrDefault();
                        if (query == null)
                            throw new Exception($"ID为{mpid}门牌已被注销");
                        query.LXProduceID = LXProduceID;
                        query.MPProduceUser = LoginUtils.CurrentUser.UserName;
                        query.MPProduceTime = DateTime.Now;

                        lxmphz.PlaceName = query.RoadName;
                        lxmphz.Type = Enums.MPTypeCh.Road;
                        lxmphz.MPNumber = query.MPNumber;
                        lxmphz.MPSize = query.MPSize;
                        lxmphz.Count = 1;

                        lxmphzs.Add(lxmphz);
                    }
                    dbContext.SaveChanges();
                }
                else if (MPType == Enums.MPTypeCh.Country)
                {
                    foreach (var mpid in MPIDs)
                    {
                        LXMPHZ lxmphz = new LXMPHZ();
                        var query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.ID == mpid).FirstOrDefault();
                        if (query == null)
                            throw new Exception($"ID为{mpid}门牌已被注销");
                        query.LXProduceID = LXProduceID;
                        query.MPProduceUser = LoginUtils.CurrentUser.UserName;
                        query.MPProduceTime = DateTime.Now;

                        lxmphz.PlaceName = query.ViligeName;
                        lxmphz.Type = Enums.MPTypeCh.Country;
                        lxmphz.MPNumber = query.MPNumber;
                        lxmphz.MPSize = query.MPSize;
                        lxmphz.Count = 1;

                        lxmphzs.Add(lxmphz);
                    }
                    dbContext.SaveChanges();
                }
                else
                    throw new Exception("未知的错误类型！");


                return CreateTabToWord_LX(lxmphzs, LXProduceID);
            }
        }
        public static MemoryStream GetProducedLXMPDetails(string LXProduceID/*ProducedLXMPList producedLXMPList*/)
        {
            WebClient client = new WebClient();
            string strUrlFilePath = Path.Combine(StaticVariable.LXMPProducePath_Full, LXProduceID + ".doc");
            var bytes = client.DownloadData(strUrlFilePath);
            MemoryStream ms = new MemoryStream(bytes);
            return ms;
            //using (var dbContext = SystemUtils.NewEFDbContext)
            //{
            //    List<LXMPHZ> data = new List<LXMPHZ>();
            //    if (producedLXMPList.MPType == Enums.MPTypeCh.Road)
            //    {
            //        data = (from t in dbContext.MPOfRoad
            //                where t.LXProduceID == producedLXMPList.LXProduceID
            //                group t by new { t.RoadName, t.MPNumber, t.MPSize } into g
            //                select new LXMPHZ
            //                {
            //                    CountryName = string.Join(",", g.Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).ToList()),
            //                    PlaceName = g.Key.RoadName,
            //                    MPNumber = g.Key.MPNumber,
            //                    MPSize = g.Key.MPSize,
            //                    Type = producedLXMPList.MPType,
            //                    Count = g.Count(),
            //                }).ToList();
            //    }
            //    else if (producedLXMPList.MPType == Enums.MPTypeCh.Country)
            //    {
            //        data = (from t in dbContext.MPOfCountry
            //                where t.LXProduceID == producedLXMPList.LXProduceID
            //                group t by new { t.ViligeName, t.MPNumber, t.MPSize } into g
            //                select new LXMPHZ
            //                {
            //                    CountryName = string.Join(",", g.Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).ToList()),
            //                    PlaceName = g.Key.ViligeName,
            //                    MPNumber = g.Key.MPNumber,
            //                    MPSize = g.Key.MPSize,
            //                    Type = producedLXMPList.MPType,
            //                    Count = g.Count(),
            //                }).ToList();
            //    }
            //    CreateTabToWord_LX(data);
            //    return data;
            //}
        }
        public static MemoryStream CreateTabToWord_LX(List<LXMPHZ> lxMPHZ, string LXProduceID)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            try
            {
                var dataCount = lxMPHZ.Count();

                int rows = dataCount + 1;
                int cols = 6;//表格列数
                object oMissing = System.Reflection.Missing.Value;
                app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                doc = app.Documents.Add();//添加一个word文档

                //输出大标题加粗加大字号水平居中
                app.Selection.Font.Bold = 700;
                app.Selection.Font.Size = 16;
                app.Selection.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                app.Selection.Text = $"嘉兴市{string.Join("、", lxMPHZ.Select(t => t.CountryName).ToList())}零星门牌汇总表";
                object line = Microsoft.Office.Interop.Word.WdUnits.wdLine;

                //换行添加表格
                app.Selection.MoveDown(ref line, oMissing, oMissing);
                app.Selection.TypeParagraph();//换行
                Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
                Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, rows, cols, ref oMissing, ref oMissing);

                //设置表格的字体大小粗细
                table.Range.Font.Size = 10;
                table.Range.Font.Bold = 0;
                table.Borders.Enable = 1;

                //设置表格标题
                int rowIndex = 1;
                //table.Cell(rowIndex, 1).Merge(table.Cell(rowIndex, 1));
                table.Cell(rowIndex, 1).Range.Text = "标准名称";
                table.Cell(rowIndex, 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //table.Cell(rowIndex, 2).Merge(table.Cell(rowIndex, 2));
                table.Cell(rowIndex, 2).Range.Text = "门牌类别";
                table.Cell(rowIndex, 2).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //table.Cell(rowIndex, 3).Merge(table.Cell(rowIndex, 3));
                table.Cell(rowIndex, 3).Range.Text = "门牌号";
                table.Cell(rowIndex, 3).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //table.Cell(rowIndex, 4).Merge(table.Cell(rowIndex, 4));
                table.Cell(rowIndex, 4).Range.Text = "规格（CM）";
                table.Cell(rowIndex, 4).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //table.Cell(rowIndex, 5).Merge(table.Cell(rowIndex, 5));
                table.Cell(rowIndex, 5).Range.Text = "邮政编码";
                table.Cell(rowIndex, 5).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //table.Cell(rowIndex, 6).Merge(table.Cell(rowIndex, 6));
                table.Cell(rowIndex, 6).Range.Text = "数量";
                table.Cell(rowIndex, 6).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                rowIndex++;
                foreach (var lxmp in lxMPHZ)
                {
                    table.Cell(rowIndex, 1).Range.Text = lxmp.PlaceName;
                    table.Cell(rowIndex, 2).Range.Text = lxmp.Type;
                    table.Cell(rowIndex, 3).Range.Text = lxmp.MPNumber;
                    table.Cell(rowIndex, 4).Range.Text = lxmp.MPSize;
                    table.Cell(rowIndex, 5).Range.Text = lxmp.Postcode;
                    table.Cell(rowIndex, 6).Range.Text = lxmp.Count.ToString();
                    rowIndex++;
                }
                //导出文件
                string newFile = LXProduceID + ".doc";
                var path = StaticVariable.LXMPProducePath_Full;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string physicNewFile = Path.Combine(path, newFile);
                doc.SaveAs(physicNewFile,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                doc.Close(ref oMissing, ref oMissing, ref oMissing);
                doc = null;
                //关闭word
                app.Quit(ref oMissing, ref oMissing, ref oMissing);
                app = null;
                byte[] data = File.ReadAllBytes(physicNewFile);
                MemoryStream ms = new MemoryStream(data);
                return ms;
            }
            catch (Exception ex)
            {
                if (doc != null)
                {
                    doc.Close();//关闭文档
                }
                if (app != null)
                {
                    app.Quit();//退出应用程序
                }
                throw ex;
            }
            finally
            {

            }
        }


        /// <summary>
        /// 获取批量导入的已制作或未制作的门牌，根据申报单位、标准名、申办人、联系电话、编制日期和批量导入的ID进行分组，统计数量
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="PLMPProduceComplete"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetProducedPLMP(int PageSize, int PageNum, string MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                IQueryable<ProducedPLMPList> result;
                if (MPType == Enums.MPTypeCh.Residence)
                {
                    var mpOfResidence = BaseUtils.DataFilterWithTown<MPOfResidence>(dbContext.MPOfResidence);
                    result = from t in mpOfResidence
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                             group t by new { t.PLProduceID, t.MPProduceUser, t.MPProduceTime } into g
                             select new ProducedPLMPList
                             {
                                 MPType = Enums.MPTypeCh.Residence,
                                 PLProduceID = g.Key.PLProduceID,
                                 MPProduceUser = g.Key.MPProduceUser,
                                 MPProduceTime = g.Key.MPProduceTime
                             };
                }
                else if (MPType == Enums.MPTypeCh.Road)
                {
                    var mpOfRoad = BaseUtils.DataFilterWithTown<MPOfRoad>(dbContext.MPOfRoad);
                    result = from t in mpOfRoad
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                             group t by new { t.PLProduceID, t.MPProduceUser, t.MPProduceTime } into g
                             select new ProducedPLMPList
                             {
                                 MPType = Enums.MPTypeCh.Road,
                                 PLProduceID = g.Key.PLProduceID,
                                 MPProduceUser = g.Key.MPProduceUser,
                                 MPProduceTime = g.Key.MPProduceTime
                             };
                }
                else if (MPType == Enums.MPTypeCh.Country)
                {
                    var mpOfCountry = BaseUtils.DataFilterWithTown<MPOfCountry>(dbContext.MPOfCountry);
                    result = from t in mpOfCountry
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                             group t by new { t.PLProduceID, t.MPProduceUser, t.MPProduceTime } into g
                             select new ProducedPLMPList
                             {
                                 MPType = Enums.MPTypeCh.Country,
                                 PLProduceID = g.Key.PLProduceID,
                                 MPProduceUser = g.Key.MPProduceUser,
                                 MPProduceTime = g.Key.MPProduceTime
                             };
                }
                else
                    throw new Exception("未知的错误类型！");

                #region 注释
                //var lz = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLProduceID = t.PLProduceID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              MPBZTime = t.BZTime,
                //              MPProduceTime = t.MPProduceTime,
                //          }).Distinct();
                //var lzC = (from t in lz
                //           group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = g.Key.PLProduceID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count() * 2,
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               MPBZTime = g.Key.MPBZTime
                //           }).ToList();
                //var dy = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLProduceID = t.PLProduceID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              DYNumber = t.DYNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              MPBZTime = t.BZTime
                //          }).Distinct();
                //var dyC = (from t in dy
                //           group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = g.Key.PLProduceID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               MPBZTime = g.Key.MPBZTime
                //           }).ToList();
                //var hsC = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //           group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = g.Key.PLProduceID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               MPBZTime = g.Key.BZTime
                //           }).ToList();

                //var xqs = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = t.PLProduceID,
                //               SBDW = t.SBDW,
                //               ResidenceName = t.ResidenceName,
                //               Applicant = t.Applicant,
                //               ApplicantPhone = t.ApplicantPhone,
                //               MPBZTime = t.BZTime
                //           }).Distinct().ToList();
                //List<ProducedPLMPList> data = new List<ProducedPLMPList>();
                //foreach (var xq in xqs)
                //{
                //    var a = lzC.Where(t => t.PLProduceID == xq.PLProduceID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var b = dyC.Where(t => t.PLProduceID == xq.PLProduceID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var c = hsC.Where(t => t.PLProduceID == xq.PLProduceID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    xq.MPCount = a + b + c;
                //    data.Add(xq);
                //}

                //var dl = (from t in mpOfRoad
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          group t by new { t.PLProduceID, t.SBDW, t.RoadName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                //          select new ProducedPLMPList
                //          {
                //              PLProduceID = g.Key.PLProduceID,
                //              SBDW = g.Key.SBDW,
                //              RoadName = g.Key.RoadName,
                //              MPCount = g.Count(),
                //              Applicant = g.Key.Applicant,
                //              ApplicantPhone = g.Key.ApplicantPhone,
                //              MPBZTime = g.Key.BZTime
                //          }).ToList();

                //data.AddRange(dl);
                //var nc = (from t in mpOfCountry
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          group t by new { t.PLProduceID, t.SBDW, t.ViligeName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                //          select new ProducedPLMPList
                //          {
                //              PLProduceID = g.Key.PLProduceID,
                //              SBDW = g.Key.SBDW,
                //              ViligeName = g.Key.ViligeName,
                //              MPCount = g.Count(),
                //              Applicant = g.Key.Applicant,
                //              ApplicantPhone = g.Key.ApplicantPhone,
                //              MPBZTime = g.Key.BZTime
                //          }).ToList();
                //data.AddRange(nc);
                #endregion
                count = result.Count();
                var data = result.OrderByDescending(t => t.MPProduceTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        public static Dictionary<string, object> GetNotProducedPLMP(int PageSize, int PageNum, string MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                IQueryable<NotProducedPLMPList> result;
                if (MPType == Enums.MPTypeCh.Residence)
                {
                    var mpOfResidence = BaseUtils.DataFilterWithTown<MPOfResidence>(dbContext.MPOfResidence);
                    result = from t in mpOfResidence
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                             group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                             select new NotProducedPLMPList
                             {
                                 PLID = g.Key.PLID,
                                 MPType = Enums.MPTypeCh.Residence,
                                 SBDW = g.Key.SBDW,
                                 ResidenceName = g.Key.ResidenceName,
                                 Applicant = g.Key.Applicant,
                                 ApplicantPhone = g.Key.ApplicantPhone,
                                 CreateTime = g.Key.CreateTime,
                                 MPCount = g.Count() + (from s in g group s by new { s.LZNumber, s.DYNumber } into h select g).Count() + (from s in g group s by new { s.LZNumber } into h select g).Count() * 2,
                             };
                }
                else if (MPType == Enums.MPTypeCh.Road)
                {
                    var mpOfRoad = BaseUtils.DataFilterWithTown<MPOfRoad>(dbContext.MPOfRoad);
                    result = from t in mpOfRoad
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                             group t by new { t.PLID, t.SBDW, t.RoadName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                             select new NotProducedPLMPList
                             {
                                 PLID = g.Key.PLID,
                                 MPType = Enums.MPTypeCh.Road,
                                 SBDW = g.Key.SBDW,
                                 RoadName = g.Key.RoadName,
                                 MPCount = g.Count(),
                                 Applicant = g.Key.Applicant,
                                 ApplicantPhone = g.Key.ApplicantPhone,
                                 CreateTime = g.Key.CreateTime
                             };
                }
                else if (MPType == Enums.MPTypeCh.Country)
                {
                    var mpOfCountry = BaseUtils.DataFilterWithTown<MPOfCountry>(dbContext.MPOfCountry);
                    result = from t in mpOfCountry
                             where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                             group t by new { t.PLID, t.SBDW, t.ViligeName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                             select new NotProducedPLMPList
                             {
                                 PLID = g.Key.PLID,
                                 MPType = Enums.MPTypeCh.Country,
                                 SBDW = g.Key.SBDW,
                                 ViligeName = g.Key.ViligeName,
                                 MPCount = g.Count(),
                                 Applicant = g.Key.Applicant,
                                 ApplicantPhone = g.Key.ApplicantPhone,
                                 CreateTime = g.Key.CreateTime
                             };
                }
                else
                    throw new Exception("未知的错误类型！");
                #region 注释
                //var lz = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLID = t.PLID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              CreateTime = t.CreateTime
                //          }).Distinct();
                //var lzC = (from t in lz
                //           group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                //           select new NotProducedPLMPList
                //           {
                //               PLID = g.Key.PLID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count() * 2,
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               CreateTime = g.Key.CreateTime
                //           }).ToList();
                //var dy = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLID = t.PLID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              DYNumber = t.DYNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              CreateTime = t.CreateTime
                //          }).Distinct();
                //var dyC = (from t in dy
                //           group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                //           select new NotProducedPLMPList
                //           {
                //               PLID = g.Key.PLID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               CreateTime = g.Key.CreateTime
                //           }).ToList();
                //var hsC = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //           group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                //           select new NotProducedPLMPList
                //           {
                //               PLID = g.Key.PLID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               CreateTime = g.Key.CreateTime
                //           }).ToList();
                //var xqs = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //           select new NotProducedPLMPList
                //           {
                //               PLID = t.PLID,
                //               SBDW = t.SBDW,
                //               ResidenceName = t.ResidenceName,
                //               Applicant = t.Applicant,
                //               ApplicantPhone = t.ApplicantPhone,
                //               CreateTime = t.CreateTime
                //           }).Distinct().ToList();

                //List<NotProducedPLMPList> data = new List<NotProducedPLMPList>();
                //foreach (var xq in xqs)
                //{
                //    var a = lzC.Where(t => t.PLID == xq.PLID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var b = dyC.Where(t => t.PLID == xq.PLID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var c = hsC.Where(t => t.PLID == xq.PLID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    xq.MPCount = a + b + c;
                //    data.Add(xq);
                //}
                #endregion

                count = result.Count();
                var data = result.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        public static MemoryStream ProducePLMP(List<string> PLIDs, string MPType)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                List<PLMPHZ> plmphzs = new List<PLMPHZ>();
                var PLProduceID = DateTime.Now.ToString("yyyyMMddhhmmss");
                var MPProduceTime = DateTime.Now;
                if (MPType == Enums.MPTypeCh.Residence)
                {
                    foreach (var plid in PLIDs)
                    {
                        var query = db.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => string.IsNullOrEmpty(t.PLProduceID)).Where(t => t.PLID == plid).ToList();

                        //foreach (var q in query)
                        //{
                        //    q.PLProduceID = PLProduceID;
                        //    q.MPProduceUser = LoginUtils.CurrentUser.UserName;
                        //    q.MPProduceTime = MPProduceTime;
                        //    db.SaveChanges();
                        //}
                        var sql = @"update MPOfResidence set PLProduceID=@PLProduceID,MPProduceUser=@MPProduceUser,MPProduceTime=@MPProduceTime where State=1 and AddType='批量' and PLProduceID is null and plid=@plid";
                        var c = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@PLProduceID", PLProduceID), new SqlParameter("@MPProduceUser", LoginUtils.CurrentUser.UserName), new SqlParameter("@MPProduceTime", DateTime.Now), new SqlParameter("@plid", plid));

                        var PlaceNames = query.Select(t => t.ResidenceName).Distinct().ToList();
                        foreach (var PlaceName in PlaceNames)
                        {
                            var plmphz = new PLMPHZ();
                            plmphz.Type = Enums.MPTypeCh.Residence;
                            plmphz.PlaceName = PlaceName;
                            plmphz.SBDW = string.Join(",", query.Where(t => t.ResidenceName == PlaceName).Select(t => t.SBDW).Distinct().ToList());
                            plmphz.Postcode = string.Join(",", query.Where(t => t.ResidenceName == PlaceName).Select(t => t.Postcode).Distinct().ToList());
                            plmphz.CountryName = string.Join(",", query.Where(t => t.ResidenceName == PlaceName).Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).Distinct().ToList());

                            plmphz.LZP = (from t in query
                                          where t.ResidenceName == PlaceName
                                          orderby t.LZNumber
                                          group t by t.LZNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = 2
                                          }).ToList();
                            var dy = (from t in query
                                      where t.ResidenceName == PlaceName
                                      select new
                                      {
                                          LZNumber = t.LZNumber,
                                          DYNumner = t.DYNumber
                                      }).Distinct();
                            plmphz.DYP = (from t in dy
                                          orderby t.DYNumner
                                          group t by t.DYNumner into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = g.Count()
                                          }).ToList();
                            plmphz.HSP = (from t in query
                                          where t.ResidenceName == PlaceName
                                          orderby t.HSNumber
                                          group t by t.HSNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = g.Count()
                                          }).ToList();
                            plmphzs.Add(plmphz);
                        }

                    }
                }
                else if (MPType == Enums.MPTypeCh.Road)
                {
                    foreach (var plid in PLIDs)
                    {
                        var query = db.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => string.IsNullOrEmpty(t.PLProduceID)).Where(t => t.PLID == plid).ToList();
                        foreach (var q in query)
                        {
                            q.PLProduceID = PLProduceID;
                            q.MPProduceUser = LoginUtils.CurrentUser.UserName;
                            q.MPProduceTime = MPProduceTime;
                        }
                        var PlaceNames = query.Select(t => t.RoadName).Distinct().ToList();
                        foreach (var PlaceName in PlaceNames)
                        {
                            var plmphz = new PLMPHZ();
                            plmphz.Type = Enums.MPTypeCh.Road;
                            plmphz.PlaceName = PlaceName;
                            plmphz.SBDW = string.Join(",", query.Where(t => t.RoadName == PlaceName).Select(t => t.SBDW).Distinct().ToList());
                            plmphz.Postcode = string.Join(",", query.Where(t => t.RoadName == PlaceName).Select(t => t.Postcode).Distinct().ToList());
                            plmphz.CountryName = string.Join(",", query.Where(t => t.RoadName == PlaceName).Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).Distinct().ToList());

                            plmphz.DLP = (from t in query
                                          where t.RoadName == PlaceName
                                          orderby t.MPNumber
                                          group t by t.MPNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              MPSize = string.Join(",", g.Select(t => t.MPSize).ToList()),
                                              Count = g.Count()
                                          }).ToList();
                            plmphzs.Add(plmphz);
                        }
                        db.SaveChanges();
                    }
                }
                else if (MPType == Enums.MPTypeCh.Country)
                {
                    foreach (var plid in PLIDs)
                    {
                        var query = db.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => string.IsNullOrEmpty(t.PLProduceID)).Where(t => t.PLID == plid).ToList();
                        foreach (var q in query)
                        {
                            q.PLProduceID = PLProduceID;
                            q.MPProduceUser = LoginUtils.CurrentUser.UserName;
                            q.MPProduceTime = MPProduceTime;
                        }
                        var PlaceNames = query.Select(t => t.ViligeName).Distinct().ToList();

                        foreach (var PlaceName in PlaceNames)
                        {
                            var plmphz = new PLMPHZ();
                            plmphz.Type = Enums.MPTypeCh.Country;
                            plmphz.PlaceName = PlaceName;
                            plmphz.SBDW = string.Join(",", query.Where(t => t.ViligeName == PlaceName).Select(t => t.SBDW).Distinct().ToList());
                            plmphz.Postcode = string.Join(",", query.Where(t => t.ViligeName == PlaceName).Select(t => t.Postcode).Distinct().ToList());
                            plmphz.CountryName = string.Join(",", query.Where(t => t.ViligeName == PlaceName).Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).Distinct().ToList());

                            plmphz.NCP = (from t in query
                                          where t.ViligeName == PlaceName
                                          orderby t.MPNumber
                                          group t by t.MPNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = g.Count()
                                          }).ToList();
                            plmphzs.Add(plmphz);
                        }
                        db.SaveChanges();
                    }
                }
                else
                    throw new Exception("未知的错误类型！");


                return CreateTabToWord_PL(plmphzs, PLProduceID);
            }
        }
        public static MemoryStream GetProducedPLMPDetails(string PLProduceID/*ProducedPLMPList producedPLMPList*/)
        {
            WebClient client = new WebClient();
            string strUrlFilePath = Path.Combine(StaticVariable.PLMPProducePath_Full, PLProduceID + ".doc");
            var bytes = client.DownloadData(strUrlFilePath);
            MemoryStream ms = new MemoryStream(bytes);
            return ms;

            //using (var dbContext = SystemUtils.NewEFDbContext)
            //{
            //    List<PLMPHZ> plmphzs = new List<PLMPHZ>();
            //    if (producedPLMPList.MPType == Enums.MPTypeCh.Residence)
            //    {

            //        var query = dbContext.MPOfResidence.Where(t => t.PLProduceID == producedPLMPList.PLProduceID);
            //        var PlaceNames = query.Select(t => t.ResidenceName).Distinct().ToList();

            //        foreach (var PlaceName in PlaceNames)
            //        {
            //            var plmphz = new PLMPHZ();
            //            plmphz.PlaceName = PlaceName;
            //            plmphz.SBDW = query.Where(t => t.ResidenceName == PlaceName).Select(t => t.SBDW).FirstOrDefault();
            //            plmphz.Postcode = query.Where(t => t.ResidenceName == PlaceName).Select(t => t.Postcode).FirstOrDefault();
            //            plmphz.CountryName = string.Join(",", query.Where(t => t.ResidenceName == PlaceName).Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).ToList());

            //            plmphz.LZP = (from t in query
            //                          where t.ResidenceName == PlaceName
            //                          group t by t.LZNumber into g
            //                          select new PLMPSL
            //                          {
            //                              Number = g.Key,
            //                              Count = 2
            //                          }).ToList();
            //            var dy = (from t in query
            //                      where t.ResidenceName == PlaceName
            //                      select new
            //                      {
            //                          LZNumber = t.LZNumber,
            //                          DYNumner = t.DYNumber
            //                      }).Distinct();
            //            plmphz.DYP = (from t in dy
            //                          group t by t.DYNumner into g
            //                          select new PLMPSL
            //                          {
            //                              Number = g.Key,
            //                              Count = g.Count()
            //                          }).ToList();
            //            plmphz.HSP = (from t in query
            //                          where t.ResidenceName == PlaceName
            //                          group t by t.HSNumber into g
            //                          select new PLMPSL
            //                          {
            //                              Number = g.Key,
            //                              Count = g.Count()
            //                          }).ToList();
            //            plmphzs.Add(plmphz);
            //        }
            //    }
            //    else if (producedPLMPList.MPType == Enums.MPTypeCh.Road)
            //    {
            //        var query = dbContext.MPOfRoad.Where(t => t.PLProduceID == producedPLMPList.PLProduceID);
            //        var PlaceNames = query.Select(t => t.RoadName).Distinct().ToList();
            //        foreach (var PlaceName in PlaceNames)
            //        {
            //            var plmphz = new PLMPHZ();
            //            plmphz.PlaceName = PlaceName;
            //            plmphz.SBDW = query.Where(t => t.RoadName == PlaceName).Select(t => t.SBDW).FirstOrDefault();
            //            plmphz.Postcode = query.Where(t => t.RoadName == PlaceName).Select(t => t.Postcode).FirstOrDefault();
            //            plmphz.CountryName = string.Join(",", query.Where(t => t.RoadName == PlaceName).Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).ToList()); //query.Where(t => t.RoadName == PlaceName).Select(t => t.CountyID).FirstOrDefault().Split('.').Last();

            //            plmphz.DLP = (from t in query
            //                          where t.RoadName == PlaceName
            //                          group t by new { t.MPNumber, t.MPSize } into g
            //                          select new PLMPSL
            //                          {
            //                              Number = g.Key.MPNumber,
            //                              MPSize = g.Key.MPSize,
            //                              Count = g.Count()
            //                          }).ToList();

            //            plmphzs.Add(plmphz);
            //        }
            //    }
            //    else if (producedPLMPList.MPType == Enums.MPTypeCh.Country)
            //    {
            //        var query = dbContext.MPOfCountry.Where(t => t.PLProduceID == producedPLMPList.PLProduceID);
            //        var PlaceNames = query.Select(t => t.ViligeName).Distinct().ToList();
            //        foreach (var PlaceName in PlaceNames)
            //        {
            //            var plmphz = new PLMPHZ();
            //            plmphz.PlaceName = PlaceName;
            //            plmphz.SBDW = query.Where(t => t.ViligeName == PlaceName).Select(t => t.SBDW).FirstOrDefault();
            //            plmphz.Postcode = query.Where(t => t.ViligeName == PlaceName).Select(t => t.Postcode).FirstOrDefault();
            //            plmphz.CountryName = string.Join(",", query.Where(t => t.ViligeName == PlaceName).Select(t => t.CountyID).ToList().Select(t => t.Split('.').Last()).ToList());// query.Where(t => t.ViligeName == PlaceName).Select(t => t.CountyID).FirstOrDefault().Split('.').Last();
            //            plmphz.NCP = (from t in query
            //                          where t.ViligeName == PlaceName
            //                          group t by t.MPNumber into g
            //                          select new PLMPSL
            //                          {
            //                              Number = g.Key,
            //                              Count = g.Count()
            //                          }).ToList();
            //            plmphzs.Add(plmphz);
            //        }
            //    }
            //    CreateTabToWord_PL(plmphzs);
            //    return plmphzs;
            //}
        }
        public static MemoryStream CreateTabToWord_PL(List<PLMPHZ> plMPHZ, string PLProduceID)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            try
            {
                var dataCount = 0;
                foreach (var plmp in plMPHZ)
                {
                    if (plmp.Type == Enums.MPTypeCh.Residence)
                    {
                        var max = Math.Max(plmp.LZP.Count(), plmp.DYP.Count());
                        max = Math.Max(max, plmp.HSP.Count());
                        dataCount += max;
                    }
                    else if (plmp.Type == Enums.MPTypeCh.Road)
                    {
                        dataCount += plmp.DLP.Count();
                    }
                    else if (plmp.Type == Enums.MPTypeCh.Country)
                    {
                        dataCount += plmp.NCP.Count();
                    }
                }

                int rows = dataCount + 4;
                int cols = 12;//表格列数
                object oMissing = System.Reflection.Missing.Value;
                app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                doc = app.Documents.Add();//添加一个word文档

                //输出大标题加粗加大字号水平居中
                app.Selection.Font.Bold = 700;
                app.Selection.Font.Size = 16;
                app.Selection.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                app.Selection.Text = $"嘉兴市{string.Join("、", plMPHZ.Select(t => t.CountryName).ToList())}批量门牌汇总表";
                object line = Microsoft.Office.Interop.Word.WdUnits.wdLine;
                app.Selection.MoveDown(ref line, oMissing, oMissing);
                app.Selection.TypeParagraph();//换行
                app.Selection.Font.Size = 12;
                app.Selection.Text = $"申报单位：{string.Join("、", plMPHZ.Select(t => t.SBDW).ToList())}    邮政编码：{string.Join("、", plMPHZ.Select(t => t.Postcode).ToList())}";


                //换行添加表格
                app.Selection.MoveDown(ref line, oMissing, oMissing);
                app.Selection.TypeParagraph();//换行
                Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
                Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, rows, cols, ref oMissing, ref oMissing);

                //设置表格的字体大小粗细
                table.Range.Font.Size = 10;
                table.Range.Font.Bold = 0;
                table.Borders.Enable = 1;

                //设置表格标题
                int rowIndex = 1;
                table.Cell(rowIndex, 1).Merge(table.Cell(rowIndex + 1, 1));
                table.Cell(rowIndex, 1).Range.Text = "标准名称";
                table.Cell(rowIndex, 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 2).Merge(table.Cell(rowIndex, 2 + 1));
                table.Cell(rowIndex, 2).Range.Text = "幢牌";
                table.Cell(rowIndex, 2).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 3).Merge(table.Cell(rowIndex, 3 + 1));
                table.Cell(rowIndex, 3).Range.Text = "单元牌";
                table.Cell(rowIndex, 3).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 4).Merge(table.Cell(rowIndex, 4 + 1));
                table.Cell(rowIndex, 4).Range.Text = "户室牌";
                table.Cell(rowIndex, 4).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 5).Merge(table.Cell(rowIndex, 5 + 2));
                table.Cell(rowIndex, 5).Range.Text = "道路门牌";
                table.Cell(rowIndex, 5).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 6).Merge(table.Cell(rowIndex, 6 + 1));
                table.Cell(rowIndex, 6).Range.Text = "农村门牌";
                table.Cell(rowIndex, 6).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;


                rowIndex++;
                table.Cell(rowIndex, 2).Range.Text = "幢号";
                table.Cell(rowIndex, 3).Range.Text = "数量";
                table.Cell(rowIndex, 4).Range.Text = "单元号";
                table.Cell(rowIndex, 5).Range.Text = "数量";
                table.Cell(rowIndex, 6).Range.Text = "户室号";
                table.Cell(rowIndex, 7).Range.Text = "数量";
                table.Cell(rowIndex, 8).Range.Text = "门牌号";
                table.Cell(rowIndex, 9).Range.Text = "规格（CM）";
                table.Cell(rowIndex, 10).Range.Text = "数量";
                table.Cell(rowIndex, 11).Range.Text = "门牌号";
                table.Cell(rowIndex, 12).Range.Text = "数量";


                rowIndex++;
                foreach (var plmp in plMPHZ)
                {
                    if (plmp.Type == Enums.MPTypeCh.Residence)
                    {
                        var max = Math.Max(plmp.LZP.Count(), plmp.DYP.Count());
                        max = Math.Max(max, plmp.HSP.Count());
                        if (max > 1)
                            table.Cell(rowIndex, 1).Merge(table.Cell(rowIndex + max - 1, 1));
                        table.Cell(rowIndex, 1).Range.Text = plmp.PlaceName;
                        table.Cell(rowIndex, 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        var lzRow = 0;
                        foreach (var lz in plmp.LZP)
                        {
                            table.Cell(rowIndex + lzRow, 2).Range.Text = lz.Number;
                            table.Cell(rowIndex + lzRow, 3).Range.Text = lz.Count.ToString();
                            lzRow++;
                        }

                        var dyRow = 0;
                        foreach (var dy in plmp.DYP)
                        {
                            table.Cell(rowIndex + dyRow, 4).Range.Text = dy.Number;
                            table.Cell(rowIndex + dyRow, 5).Range.Text = dy.Count.ToString();
                            dyRow++;
                        }

                        var hsRow = 0;
                        foreach (var hs in plmp.HSP)
                        {
                            table.Cell(rowIndex + hsRow, 6).Range.Text = hs.Number;
                            table.Cell(rowIndex + hsRow, 7).Range.Text = hs.Count.ToString();
                            hsRow++;
                        }
                        rowIndex = rowIndex + max;
                    }

                    else if (plmp.Type == Enums.MPTypeCh.Road)
                    {
                        var max = plmp.DLP.Count();
                        table.Cell(rowIndex, 1).Merge(table.Cell(rowIndex + max - 1, 1));
                        table.Cell(rowIndex, 1).Range.Text = plmp.PlaceName;
                        table.Cell(rowIndex, 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        var dlRow = 0;
                        foreach (var dl in plmp.DLP)
                        {
                            table.Cell(rowIndex + dlRow, 8).Range.Text = dl.Number;
                            table.Cell(rowIndex + dlRow, 9).Range.Text = dl.MPSize;
                            table.Cell(rowIndex + dlRow, 10).Range.Text = dl.Count.ToString();
                            dlRow++;
                        }
                        rowIndex = rowIndex + max + 1;
                    }
                    else if (plmp.Type == Enums.MPTypeCh.Country)
                    {
                        var max = plmp.NCP.Count();
                        table.Cell(rowIndex, 1).Merge(table.Cell(rowIndex + max - 1, 1));
                        table.Cell(rowIndex, 1).Range.Text = plmp.PlaceName;
                        table.Cell(rowIndex, 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        var dlRow = 0;
                        foreach (var nc in plmp.NCP)
                        {
                            table.Cell(rowIndex + dlRow, 11).Range.Text = nc.Number;
                            table.Cell(rowIndex + dlRow, 12).Range.Text = nc.Count.ToString();
                            dlRow++;
                        }
                        rowIndex = rowIndex + max;
                    }
                }
                //导出文件
                string newFile = PLProduceID + ".doc";
                var path = StaticVariable.PLMPProducePath_Full;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string physicNewFile = Path.Combine(path, newFile);
                doc.SaveAs(physicNewFile,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                doc.Close(ref oMissing, ref oMissing, ref oMissing);
                doc = null;
                //关闭word
                app.Quit(ref oMissing, ref oMissing, ref oMissing);
                app = null;
                byte[] data = File.ReadAllBytes(physicNewFile);
                MemoryStream ms = new MemoryStream(data);
                return ms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }




        public static void CreateTabToWord2()
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            try
            {
                int rows = 10 + 2;
                int cols = 12;//表格列数
                object oMissing = System.Reflection.Missing.Value;
                app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                doc = app.Documents.Add();//添加一个word文档

                //输出大标题加粗加大字号水平居中
                app.Selection.Font.Bold = 700;
                app.Selection.Font.Size = 16;
                app.Selection.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                app.Selection.Text = $"嘉兴市批量门牌汇总表";
                object line = Microsoft.Office.Interop.Word.WdUnits.wdLine;
                app.Selection.MoveDown(ref line, oMissing, oMissing);
                app.Selection.TypeParagraph();//换行
                app.Selection.Font.Size = 12;
                app.Selection.Text = $"申报单位：                         邮政编码：";

                //换行添加表格
                app.Selection.MoveDown(ref line, oMissing, oMissing);
                app.Selection.TypeParagraph();//换行
                app.Selection.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
                Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, rows, cols, ref oMissing, ref oMissing);

                //设置表格的字体大小粗细
                table.Range.Font.Size = 10;
                table.Range.Font.Bold = 0;
                table.Borders.Enable = 1;


                //设置表格标题
                int rowIndex = 1;
                table.Cell(rowIndex, 1).Merge(table.Cell(rowIndex + 1, 1));//合并班级
                table.Cell(rowIndex, 1).Range.Text = "标准名称";
                table.Cell(rowIndex, 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 2).Merge(table.Cell(rowIndex, 2 + 1));//合并班级
                table.Cell(rowIndex, 2).Range.Text = "幢牌";
                table.Cell(rowIndex, 2).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 3).Merge(table.Cell(rowIndex, 3 + 1));//合并班级
                table.Cell(rowIndex, 3).Range.Text = "单元牌";
                table.Cell(rowIndex, 3).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 4).Merge(table.Cell(rowIndex, 4 + 1));//合并班级
                table.Cell(rowIndex, 4).Range.Text = "户室牌";
                table.Cell(rowIndex, 4).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 5).Merge(table.Cell(rowIndex, 5 + 2));//合并班级
                table.Cell(rowIndex, 5).Range.Text = "道路门牌";
                table.Cell(rowIndex, 5).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                table.Cell(rowIndex, 6).Merge(table.Cell(rowIndex, 6 + 1));//合并班级
                table.Cell(rowIndex, 6).Range.Text = "农村门牌";
                table.Cell(rowIndex, 6).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                rowIndex++;
                table.Cell(rowIndex, 2).Range.Text = "幢号";
                table.Cell(rowIndex, 3).Range.Text = "数量";
                table.Cell(rowIndex, 4).Range.Text = "单元号";
                table.Cell(rowIndex, 5).Range.Text = "数量";
                table.Cell(rowIndex, 6).Range.Text = "户室号";
                table.Cell(rowIndex, 7).Range.Text = "数量";
                table.Cell(rowIndex, 8).Range.Text = "门牌号";
                table.Cell(rowIndex, 9).Range.Text = "规格（CM）";
                table.Cell(rowIndex, 10).Range.Text = "数量";
                table.Cell(rowIndex, 11).Range.Text = "门牌号";
                table.Cell(rowIndex, 12).Range.Text = "数量";

                //导出到文件
                string newFile = DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "MPProduce");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string physicNewFile = Path.Combine(path, newFile);
                doc.SaveAs(physicNewFile,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                ////构造数据
                //List<Student> datas = new List<Student>();
                //datas.Add(new Student { Leader = "小李", Name = "张三", Score = 498, StuClass = "一班" });
                //datas.Add(new Student { Leader = "陈飞", Name = "李四", Score = 354, StuClass = "二班" });
                //datas.Add(new Student { Leader = "陈飞", Name = "小红", Score = 502, StuClass = "二班" });
                //datas.Add(new Student { Leader = "王林", Name = "丁爽", Score = 566, StuClass = "三班" });
                //var cate = datas.GroupBy(s => s.StuClass);

                //int rows = datas.Count + 1;
                //int cols = 5;//表格列数
                //object oMissing = System.Reflection.Missing.Value;
                //app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                //doc = app.Documents.Add();//添加一个word文档

                ////输出大标题加粗加大字号水平居中
                //app.Selection.Font.Bold = 700;
                //app.Selection.Font.Size = 16;
                //app.Selection.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //app.Selection.Text = "班级成绩统计单";

                ////换行添加表格
                //object line = Microsoft.Office.Interop.Word.WdUnits.wdLine;
                //app.Selection.MoveDown(ref line, oMissing, oMissing);
                //app.Selection.TypeParagraph();//换行
                //Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
                //Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, rows, cols, ref oMissing, ref oMissing);

                ////设置表格的字体大小粗细
                //table.Range.Font.Size = 10;
                //table.Range.Font.Bold = 0;

                ////设置表格标题
                //int rowIndex = 1;
                //table.Cell(rowIndex, 1).Range.Text = "班级";
                //table.Cell(rowIndex, 2).Range.Text = "姓名";
                //table.Cell(rowIndex, 3).Range.Text = "成绩";
                //table.Cell(rowIndex, 4).Range.Text = "人数";
                //table.Cell(rowIndex, 5).Range.Text = "班主任";

                ////循环数据创建数据行
                //rowIndex++;
                //foreach (var i in cate)
                //{
                //    int moveCount = i.Count() - 1;//纵向合并行数
                //    if (moveCount.ToString() != "0")
                //    {
                //        table.Cell(rowIndex, 1).Merge(table.Cell(rowIndex + moveCount, 1));//合并班级
                //        table.Cell(rowIndex, 4).Merge(table.Cell(rowIndex + moveCount, 4));//合并人数
                //        table.Cell(rowIndex, 5).Merge(table.Cell(rowIndex + moveCount, 5));//合并班主任
                //    }
                //    //写入合并的数据并垂直居中
                //    table.Cell(rowIndex, 1).Range.Text = i.Key;
                //    table.Cell(rowIndex, 4).Range.Text = i.Count().ToString();
                //    table.Cell(rowIndex, 5).Range.Text = i.First().Leader;
                //    table.Cell(rowIndex, 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //    table.Cell(rowIndex, 4).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //    table.Cell(rowIndex, 5).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //    //构建姓名，成绩数据
                //    foreach (var x in i)
                //    {
                //        table.Cell(rowIndex, 2).Range.Text = x.Name;
                //        table.Cell(rowIndex, 3).Range.Text = x.Score.ToString();
                //        rowIndex++;
                //    }
                //}
                ////导出到文件
                //string newFile = DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
                //string physicNewFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "MPProduce");
                //doc.SaveAs(physicNewFile,
                //oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();//关闭文档
                }
                if (app != null)
                {
                    app.Quit();//退出应用程序
                }
            }
        }
    }
}

public class PLMPHZ
{
    public string Type { get; set; }
    public string CountryName { get; set; }
    public string PlaceName { get; set; }
    public string SBDW { get; set; }
    public string Postcode { get; set; }
    public List<PLMPSL> LZP { get; set; }
    public List<PLMPSL> DYP { get; set; }
    public List<PLMPSL> HSP { get; set; }
    public List<PLMPSL> DLP { get; set; }
    public List<PLMPSL> NCP { get; set; }
}
public class PLMPSL
{
    public string Number { get; set; }
    public string MPSize { get; set; }
    public int Count { get; set; }
}
public class LXMPHZ
{
    public string Type { get; set; }
    public string CountryName { get; set; }
    public string PlaceName { get; set; }
    public string MPSize { get; set; }
    public string MPNumber { get; set; }
    public string Postcode { get; set; }
    public int Count { get; set; }
}

public class Student
{
    public string Name;//姓名
    public int Score;//成绩
    public string StuClass;//班级
    public string Leader;//班主任
}