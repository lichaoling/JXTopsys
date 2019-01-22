using iTextSharp.text;
using iTextSharp.text.pdf;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Controllers;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace JXGIS.JXTopsystem.Business.MPPrintUtils
{
    public class MPPrintUtils
    {
        public static MemoryStream DZZMPrint(List<string> IDs, string MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<MPOfCertificate> mpOfCertificates = new List<Models.Entities.MPOfCertificate>();
                List<string> docNames = new List<string>();
                foreach (var ID in IDs)
                {
                    var isEnable = false;
                    if (MPType == Enums.MPTypeCh.Residence)
                    {
                        var mpOfResidence = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfResidence == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");
                        Dictionary<string, string> bookmarks = new Dictionary<string, string>();
                        bookmarks.Add("{PropertyOwner}", string.IsNullOrEmpty(mpOfResidence.PropertyOwner) ? "" : mpOfResidence.PropertyOwner);
                        bookmarks.Add("{StandardAddress}", mpOfResidence.StandardAddress);
                        bookmarks.Add("{FCZAddress}", string.IsNullOrEmpty(mpOfResidence.FCZAddress) ? "" : mpOfResidence.FCZAddress);
                        bookmarks.Add("{TDZAddress}", string.IsNullOrEmpty(mpOfResidence.TDZAddress) ? "" : mpOfResidence.TDZAddress);
                        bookmarks.Add("{YYZZAddress/HJAddress}", string.IsNullOrEmpty(mpOfResidence.HJAddress) ? "" : mpOfResidence.HJAddress);
                        bookmarks.Add("{OtherAddress}", string.IsNullOrEmpty(mpOfResidence.OtherAddress) ? "" : mpOfResidence.OtherAddress);
                        bookmarks.Add("{Year}", DateTime.Now.Year.ToString());
                        bookmarks.Add("{Month}", DateTime.Now.Month.ToString());
                        bookmarks.Add("{Date}", DateTime.Now.Day.ToString());

                        string savePath = Path.Combine(StaticVariable.DZZMPrintPath, Enums.MPTypeCh.Residence, ID);
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        string fileNameWord = Path.Combine(savePath, mpOfResidence.StandardAddress + "-地址证明.docx");
                        string fileNamePdf = Path.Combine(savePath, mpOfResidence.StandardAddress + "-地址证明.pdf");
                        GenerateWord(StaticVariable.DZZMtemplateFile, fileNameWord, fileNamePdf, bookmarks, ID);
                        docNames.Add(fileNamePdf);
                        isEnable = true;
                    }
                    else if (MPType == Enums.MPTypeCh.Road)
                    {
                        var mpOfRoad = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfRoad == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");
                        Dictionary<string, string> bookmarks = new Dictionary<string, string>();
                        bookmarks.Add("{PropertyOwner}", string.IsNullOrEmpty(mpOfRoad.PropertyOwner) ? "" : mpOfRoad.PropertyOwner);
                        bookmarks.Add("{StandardAddress}", mpOfRoad.StandardAddress);
                        bookmarks.Add("{FCZAddress}", string.IsNullOrEmpty(mpOfRoad.FCZAddress) ? "" : mpOfRoad.FCZAddress);
                        bookmarks.Add("{TDZAddress}", string.IsNullOrEmpty(mpOfRoad.TDZAddress) ? "" : mpOfRoad.TDZAddress);
                        bookmarks.Add("{YYZZAddress/HJAddress}", string.IsNullOrEmpty(mpOfRoad.YYZZAddress) ? "" : mpOfRoad.YYZZAddress);
                        bookmarks.Add("{OtherAddress}", string.IsNullOrEmpty(mpOfRoad.OtherAddress) ? "" : mpOfRoad.OtherAddress);
                        bookmarks.Add("{Year}", DateTime.Now.Year.ToString());
                        bookmarks.Add("{Month}", DateTime.Now.Month.ToString());
                        bookmarks.Add("{Date}", DateTime.Now.Day.ToString());

                        string savePath = Path.Combine(StaticVariable.DZZMPrintPath, Enums.MPTypeCh.Road, ID);
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        string fileNameWord = Path.Combine(savePath, mpOfRoad.StandardAddress + "-地址证明.docx");
                        string fileNamePdf = Path.Combine(savePath, mpOfRoad.StandardAddress + "-地址证明.pdf");
                        GenerateWord(StaticVariable.DZZMtemplateFile, fileNameWord, fileNamePdf, bookmarks, ID);
                        docNames.Add(fileNamePdf);
                        isEnable = true;
                    }
                    else if (MPType == Enums.MPTypeCh.Country)
                    {
                        var mpOfCounty = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfCounty == null)
                            throw new Exception($"ID为{ID}的农村门牌已经注销，请重新查询！");
                        Dictionary<string, string> bookmarks = new Dictionary<string, string>();
                        bookmarks.Add("{PropertyOwner}", mpOfCounty.PropertyOwner);
                        bookmarks.Add("{StandardAddress}", mpOfCounty.StandardAddress);
                        bookmarks.Add("{FCZAddress}", "");
                        bookmarks.Add("{TDZAddress}", string.IsNullOrEmpty(mpOfCounty.TDZAddress) ? "" : mpOfCounty.TDZAddress);
                        bookmarks.Add("{YYZZAddress/HJAddress}", "");
                        bookmarks.Add("{OtherAddress}", string.IsNullOrEmpty(mpOfCounty.OtherAddress) ? "" : mpOfCounty.OtherAddress);
                        bookmarks.Add("{Year}", DateTime.Now.Year.ToString());
                        bookmarks.Add("{Month}", DateTime.Now.Month.ToString());
                        bookmarks.Add("{Date}", DateTime.Now.Day.ToString());

                        string savePath = Path.Combine(StaticVariable.DZZMPrintPath, Enums.MPTypeCh.Country, ID);
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        string fileNameWord = Path.Combine(savePath, mpOfCounty.StandardAddress + "-地址证明.docx");
                        string fileNamePdf = Path.Combine(savePath, mpOfCounty.StandardAddress + "-地址证明.pdf");
                        GenerateWord(StaticVariable.DZZMtemplateFile, fileNameWord, fileNamePdf, bookmarks, ID);
                        docNames.Add(fileNamePdf);
                        isEnable = true;
                    }

                    if (isEnable)
                    {
                        MPOfCertificate mpCertificate = new Models.Entities.MPOfCertificate();
                        mpCertificate.ID = Guid.NewGuid().ToString();
                        mpCertificate.MPID = ID;
                        mpCertificate.CreateTime = DateTime.Now;
                        mpCertificate.CreateUser = LoginUtils.CurrentUser.UserName;
                        mpCertificate.MPType = MPType;
                        mpCertificate.CertificateType = Enums.CertificateType.Placename;
                        mpCertificate.Window = LoginUtils.CurrentUser.Window;
                        mpOfCertificates.Add(mpCertificate);
                    }
                }
                dbContext.MPOfCertificate.AddRange(mpOfCertificates);
                dbContext.SaveChanges();
                var ms = MergePDF_DZZM(docNames);
                return ms;
            }
        }
        public static MemoryStream MPZPrint(List<string> IDs, string MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<MPOfCertificate> mpCertificates = new List<Models.Entities.MPOfCertificate>();
                List<string> docNames = new List<string>();
                foreach (var ID in IDs)
                {
                    var isEnable = false;
                    if (MPType == Enums.MPTypeCh.Residence)
                    {
                        var mpOfResidence = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfResidence == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");
                        var res = ReplaceBadChar(mpOfResidence.ResidenceName) + (string.IsNullOrEmpty(mpOfResidence.LZNumber) ? "" : mpOfResidence.LZNumber + "幢") + (string.IsNullOrEmpty(mpOfResidence.DYNumber) ? "" : mpOfResidence.DYNumber + "单元") + (string.IsNullOrEmpty(mpOfResidence.HSNumber) ? "" : mpOfResidence.HSNumber + "室");
                        Dictionary<string, string> bookmarks = new Dictionary<string, string>();
                        bookmarks.Add("{AddressCoding}", mpOfResidence.AddressCoding);
                        bookmarks.Add("{PropertyOwner}", mpOfResidence.PropertyOwner == null ? "" : mpOfResidence.PropertyOwner);
                        bookmarks.Add("{CountyName}", mpOfResidence.CountyID.Split('.').Last());
                        bookmarks.Add("{NeighborhoodsName}", mpOfResidence.NeighborhoodsID.Split('.').Last());
                        bookmarks.Add("{RoadName}", "");
                        bookmarks.Add("{MPNumber}", "");
                        bookmarks.Add("{ResidenceName}", res);
                        bookmarks.Add("{OriginalMPAddress}", "");
                        bookmarks.Add("{Year}", DateTime.Now.Year.ToString());
                        bookmarks.Add("{Month}", DateTime.Now.Month.ToString());
                        bookmarks.Add("{Date}", DateTime.Now.Day.ToString());

                        string savePath = Path.Combine(StaticVariable.MPZPrintPath, Enums.MPTypeCh.Residence, ID);
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        var name = ReplaceBadChar(mpOfResidence.StandardAddress);
                        string fileNameWord = Path.Combine(savePath, name + "-门牌证.docx");
                        string fileNamePdf = Path.Combine(savePath, name + "-门牌证.pdf");
                        GenerateWord(StaticVariable.MPZtemplateFile, fileNameWord, fileNamePdf, bookmarks, ID);
                        docNames.Add(fileNamePdf);
                        isEnable = true;
                    }
                    else if (MPType == Enums.MPTypeCh.Road)
                    {
                        var mpOfRoad = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfRoad == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");

                        Dictionary<string, string> bookmarks = new Dictionary<string, string>();
                        bookmarks.Add("{AddressCoding}", mpOfRoad.AddressCoding);
                        bookmarks.Add("{PropertyOwner}", mpOfRoad.PropertyOwner == null ? "" : mpOfRoad.PropertyOwner);
                        bookmarks.Add("{CountyName}", mpOfRoad.CountyID.Split('.').Last());
                        bookmarks.Add("{NeighborhoodsName}", mpOfRoad.NeighborhoodsID.Split('.').Last());
                        bookmarks.Add("{RoadName}", ReplaceBadChar(mpOfRoad.RoadName));
                        bookmarks.Add("{MPNumber}", mpOfRoad.MPNumber == null ? "" : mpOfRoad.MPNumber);
                        bookmarks.Add("{ResidenceName}", "");
                        bookmarks.Add("{OriginalMPAddress}", mpOfRoad.OriginalMPAddress == null ? "" : mpOfRoad.OriginalMPAddress);
                        bookmarks.Add("{Year}", DateTime.Now.Year.ToString());
                        bookmarks.Add("{Month}", DateTime.Now.Month.ToString());
                        bookmarks.Add("{Date}", DateTime.Now.Day.ToString());

                        string savePath = Path.Combine(StaticVariable.MPZPrintPath, Enums.MPTypeCh.Road, ID);
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        var name = ReplaceBadChar(mpOfRoad.StandardAddress);
                        string fileNameWord = Path.Combine(savePath, name + "-门牌证.docx");
                        string fileNamePdf = Path.Combine(savePath, name + "-门牌证.pdf");
                        if (!File.Exists(fileNameWord) || !File.Exists(fileNamePdf))
                            GenerateWord(StaticVariable.MPZtemplateFile, fileNameWord, fileNamePdf, bookmarks, ID);
                        docNames.Add(fileNamePdf);
                        isEnable = true;
                    }
                    else if (MPType == Enums.MPTypeCh.Country)
                    {
                        var mpOfCounty = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfCounty == null)
                            throw new Exception($"ID为{ID}的农村门牌已经注销，请重新查询！");
                        var res = mpOfCounty.CommunityName + ReplaceBadChar(mpOfCounty.ViligeName) + (string.IsNullOrEmpty(mpOfCounty.MPNumber) ? "" : mpOfCounty.MPNumber + "号");

                        Dictionary<string, string> bookmarks = new Dictionary<string, string>();
                        bookmarks.Add("{AddressCoding}", mpOfCounty.AddressCoding);
                        bookmarks.Add("{PropertyOwner}", mpOfCounty.PropertyOwner == null ? "" : mpOfCounty.PropertyOwner);
                        bookmarks.Add("{CountyName}", mpOfCounty.CountyID.Split('.').Last());
                        bookmarks.Add("{NeighborhoodsName}", mpOfCounty.NeighborhoodsID.Split('.').Last());
                        bookmarks.Add("{RoadName}", "");
                        bookmarks.Add("{MPNumber}", "");
                        bookmarks.Add("{ResidenceName}", res);
                        bookmarks.Add("{OriginalMPAddress}", mpOfCounty.OriginalMPAddress == null ? "" : mpOfCounty.OriginalMPAddress);
                        bookmarks.Add("{Year}", DateTime.Now.Year.ToString());
                        bookmarks.Add("{Month}", DateTime.Now.Month.ToString());
                        bookmarks.Add("{Date}", DateTime.Now.Day.ToString());

                        string savePath = Path.Combine(StaticVariable.MPZPrintPath, Enums.MPTypeCh.Country, ID);
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        var name = ReplaceBadChar(mpOfCounty.StandardAddress);
                        string fileNameWord = Path.Combine(savePath, name + "-门牌证.docx");
                        string fileNamePdf = Path.Combine(savePath, name + "-门牌证.pdf");
                        GenerateWord(StaticVariable.MPZtemplateFile, fileNameWord, fileNamePdf, bookmarks, ID);
                        docNames.Add(fileNamePdf);
                        isEnable = true;
                    }
                    if (isEnable)
                    {
                        MPOfCertificate mpCertificate = new Models.Entities.MPOfCertificate();
                        mpCertificate.ID = Guid.NewGuid().ToString();
                        mpCertificate.MPID = ID;
                        mpCertificate.CreateTime = DateTime.Now;
                        mpCertificate.CreateUser = LoginUtils.CurrentUser.UserName;
                        mpCertificate.MPType = MPType;
                        mpCertificate.CertificateType = Enums.CertificateType.MPZ;
                        mpCertificate.Window = LoginUtils.CurrentUser.Window;
                        mpCertificates.Add(mpCertificate);
                    }
                }
                dbContext.MPOfCertificate.AddRange(mpCertificates);
                dbContext.SaveChanges();
                var ms = MergePDF_MPZ(docNames);
                return ms;
            }
        }
        public static MemoryStream MergePDF_MPZ(List<string> docNames)
        {
            //iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(360, 510);
            //iTextSharp.text.Document document = new iTextSharp.text.Document(pageSize);
            //var targetPDF = Path.Combine(StaticVariable.MergeFilePath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            //if (!Directory.Exists(StaticVariable.MergeFilePath))
            //{
            //    Directory.CreateDirectory(StaticVariable.MergeFilePath);
            //}
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(targetPDF, FileMode.Create));
            //document.Open();
            //PdfContentByte cb = writer.DirectContent;
            //PdfImportedPage newPage;
            //PdfReader reader;
            //for (int i = 0; i < docNames.Count; i++)
            //{
            //    reader = new PdfReader(docNames[i]);
            //    int iPageNum = reader.NumberOfPages;
            //    for (int j = 1; j <= iPageNum; j++)
            //    {
            //        document.NewPage();
            //        newPage = writer.GetImportedPage(reader, j);
            //        cb.AddTemplate(newPage, 0, 0);
            //    }
            //}
            //document.Close();
            //var ms = DownLoad(targetPDF);
            //return ms;

            var targetPDF = Path.Combine(StaticVariable.MergeFilePath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            if (!Directory.Exists(StaticVariable.MergeFilePath))
            {
                Directory.CreateDirectory(StaticVariable.MergeFilePath);
            }
            Aspose.Pdf.Document a = new Aspose.Pdf.Document();
            foreach (var file in docNames)//遍历源路径，获取该路径下所有PDF文件的path
            {
                Aspose.Pdf.Document b = new Aspose.Pdf.Document(file);
                foreach (Aspose.Pdf.Page item in b.Pages)
                {
                    a.Pages.Add(item);
                }

            }
            a.Save(targetPDF);
            var ms = DownLoad(targetPDF);
            return ms;
        }
        public static MemoryStream MergePDF_DZZM(List<string> docNames)
        {
            //iTextSharp.text.Document document = new iTextSharp.text.Document();
            //var targetPDF = Path.Combine(StaticVariable.MergeFilePath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            //if (!Directory.Exists(StaticVariable.MergeFilePath))
            //{
            //    Directory.CreateDirectory(StaticVariable.MergeFilePath);
            //}
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(targetPDF, FileMode.Create));
            //document.Open();
            //PdfContentByte cb = writer.DirectContent;
            //PdfImportedPage newPage;
            //PdfReader reader;
            //for (int i = 0; i < docNames.Count; i++)
            //{
            //    reader = new PdfReader(docNames[i]);
            //    int iPageNum = reader.NumberOfPages;
            //    for (int j = 1; j <= iPageNum; j++)
            //    {
            //        document.NewPage();
            //        newPage = writer.GetImportedPage(reader, j);
            //        cb.AddTemplate(newPage, 0, 0);
            //    }
            //}
            //document.Close();
            //var ms = DownLoad(targetPDF);
            //return ms;
            var targetPDF = Path.Combine(StaticVariable.MergeFilePath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            if (!Directory.Exists(StaticVariable.MergeFilePath))
            {
                Directory.CreateDirectory(StaticVariable.MergeFilePath);
            }
            Aspose.Pdf.Document a = new Aspose.Pdf.Document();
            foreach (var file in docNames)//遍历源路径，获取该路径下所有PDF文件的path
            {
                Aspose.Pdf.Document b = new Aspose.Pdf.Document(file);
                foreach (Aspose.Pdf.Page item in b.Pages)
                {
                    a.Pages.Add(item);
                }

            }
            a.Save(targetPDF);
            var ms = DownLoad(targetPDF);
            return ms;
        }
        public static MemoryStream DownLoad(string FilePath)
        {
            WebClient wc = new WebClient();
            var bytes = wc.DownloadData(FilePath);
            MemoryStream ms = new MemoryStream(bytes);
            //删除文件
            File.Delete(FilePath);
            return ms;
        }

        /// <summary>
        /// 根据word模板文件导出word/pdf文件
        /// </summary>
        /// <param name="templateFile">模板路径</param>
        /// <param name="fileNameWord">导出文件名称</param>
        /// <param name="fileNamePdf">pdf文件名称</param>
        /// <param name="bookmarks">模板内书签集合</param>
        public static void GenerateWord(string templateFile, string fileNameWord, string fileNamePdf, Dictionary<string, string> bookmarks, string ID)
        {
            //Application app = new Application();
            //File.Copy(templateFile, fileNameWord, true);
            //Microsoft.Office.Interop.Word.Document doc = new Microsoft.Office.Interop.Word.Document();
            //object Obj_FileName = fileNameWord;
            //object Visible = false;
            //object ReadOnly = false;
            //object missing = System.Reflection.Missing.Value;
            //object IsSave = true;
            //object FileName = fileNamePdf;
            //object FileFormat = WdSaveFormat.wdFormatPDF;
            //object LockComments = false;
            //object AddToRecentFiles = true;
            //object ReadOnlyRecommended = false;
            //object EmbedTrueTypeFonts = false;
            //object SaveNativePictureFormat = true;
            //object SaveFormsData = false;
            //object SaveAsAOCELetter = false;
            //object Encoding = MsoEncoding.msoEncodingSimplifiedChineseGB18030;
            //object InsertLineBreaks = false;
            //object AllowSubstitutions = false;
            //object LineEnding = WdLineEndingType.wdCRLF;
            //object AddBiDiMarks = false;

            ////object tempPdfFile = Path.Combine(TempPdf, ID + ".pdf");

            //try
            //{
            //    doc = app.Documents.Open(ref Obj_FileName, ref missing, ref ReadOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref Visible, ref missing, ref missing, ref missing, ref missing);
            //    //if (doc == null) throw new Exception(fileNameWord);
            //    doc.Activate();

            //    foreach (string bookmarkName in bookmarks.Keys)
            //    {
            //        replace(doc, bookmarkName, bookmarks[bookmarkName]);//替换内容
            //    }
            //    //replace(doc, "hello", "shalv");
            //    //此处存储时，参数可选填，如需另外生成pdf，加入一个参数ref FileName,
            //    doc.SaveAs(ref FileName, ref FileFormat, ref LockComments,
            //            ref missing, ref AddToRecentFiles, ref missing,
            //            ref ReadOnlyRecommended, ref EmbedTrueTypeFonts,
            //            ref SaveNativePictureFormat, ref SaveFormsData,
            //            ref SaveAsAOCELetter, ref Encoding, ref InsertLineBreaks,
            //            ref AllowSubstitutions, ref LineEnding, ref AddBiDiMarks);
            //    doc.Close(ref IsSave, ref missing, ref missing);
            //    doc = null;
            //}
            //catch (Exception ex)
            //{
            //    if (doc != null) doc.Close(ref IsSave, ref missing, ref missing);
            //    throw ex;
            //}
            var doc = new Aspose.Words.Document(templateFile);
            var builder = new Aspose.Words.DocumentBuilder(doc);
            //var options = new Aspose.Words.Replacing.FindReplaceOptions() { MatchCase = true, FindWholeWordsOnly = true };
            foreach (string name in bookmarks.Keys)
            {
                var x = doc.Range.Replace(name, bookmarks[name], true, false);
            }
            doc.Save(fileNameWord);
            doc.Save(fileNamePdf, Aspose.Words.SaveFormat.Pdf);
        }
        /// <summary>
        /// 在word 中查找一个字符串直接替换所需要的文本
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="strOldText">原文本</param>
        /// <param name="strNewText">新文本</param>
        public static void replace(Microsoft.Office.Interop.Word.Document doc, string strOldText, string strNewText)
        {
            doc.Content.Find.Text = strOldText;
            object FindText, ReplaceWith, Replace;// 
            object MissingValue = Type.Missing;
            FindText = strOldText;//要查找的文本
            ReplaceWith = strNewText;//替换文本
            Replace = WdReplace.wdReplaceAll;
            doc.Content.Find.ClearFormatting();//移除Find的搜索文本和段落格式设置
            doc.Content.Find.Execute(
                ref FindText, ref MissingValue,
                ref MissingValue, ref MissingValue,
                ref MissingValue, ref MissingValue,
                ref MissingValue, ref MissingValue, ref MissingValue,
                ref ReplaceWith, ref Replace,
                ref MissingValue, ref MissingValue,
                ref MissingValue, ref MissingValue);
        }

        /// <summary>
        /// 地名证明打印或门牌证打印
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="MPType"></param>
        /// <param name="CertificateType"></param>
        public static List<MPCertificate> MPCertificate(List<string> IDs, string MPType, string CertificateType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<MPCertificate> certificates = new List<Models.Extends.MPCertificate>();
                List<MPOfCertificate> mpOfCertificates = new List<Models.Entities.MPOfCertificate>();

                foreach (var ID in IDs)
                {
                    MPOfCertificate mpOfCertificate = new Models.Entities.MPOfCertificate();
                    var GUID = Guid.NewGuid().ToString();
                    var CreateTime = DateTime.Now;
                    mpOfCertificate.ID = GUID;
                    mpOfCertificate.MPID = ID;
                    mpOfCertificate.CreateTime = CreateTime;
                    mpOfCertificate.CreateUser = LoginUtils.CurrentUser.UserName;
                    mpOfCertificate.MPType = MPType;
                    mpOfCertificate.CertificateType = CertificateType;
                    mpOfCertificate.Window = LoginUtils.CurrentUser.Window;
                    mpOfCertificates.Add(mpOfCertificate);

                    MPCertificate certificate = new Models.Extends.MPCertificate();
                    certificate.ID = ID;
                    if (MPType == Enums.MPTypeCh.Residence)
                    {
                        var mpOfResidence = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfResidence == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");

                        certificate.PropertyOwner = mpOfResidence.PropertyOwner;
                        certificate.StandardAddress = mpOfResidence.StandardAddress;
                        certificate.FCZAddress = mpOfResidence.FCZAddress;
                        certificate.TDZAddress = mpOfResidence.TDZAddress;
                        certificate.BDCZAddress = mpOfResidence.BDCZAddress;
                        certificate.HJAddress = mpOfResidence.HJAddress;
                        certificate.OtherAddress = mpOfResidence.OtherAddress;
                        certificate.CountyID = mpOfResidence.CountyID;
                        certificate.CountyName = mpOfResidence.CountyID.Split('.').Last();
                        certificate.NeighborhoodsID = mpOfResidence.NeighborhoodsID;
                        certificate.NeighborhoodsName = mpOfResidence.NeighborhoodsID.Split('.').Last();
                        certificate.CommunityName = mpOfResidence.CommunityName;
                        certificate.MPNumber = mpOfResidence.MPNumber;
                        certificate.ResidenceName = mpOfResidence.ResidenceName;
                        certificate.Dormitory = mpOfResidence.Dormitory;
                        certificate.LZNumber = mpOfResidence.LZNumber;
                        certificate.DYNumber = mpOfResidence.DYNumber;
                        certificate.HSNumber = mpOfResidence.HSNumber;
                        certificate.AddressCoding = mpOfResidence.AddressCoding;

                        if (CertificateType == Enums.CertificateType.MPZ)
                            mpOfResidence.MPZPrintComplete = Enums.Complete.Yes;
                        else if (CertificateType == Enums.CertificateType.Placename)
                            mpOfResidence.DZZMPrintComplete = Enums.Complete.Yes;

                        BaseUtils.UpdateAddressCode(mpOfResidence, null, null, null, Enums.TypeInt.Residence);
                    }
                    else if (MPType == Enums.MPTypeCh.Road)
                    {
                        var mpOfRoad = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfRoad == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");

                        certificate.PropertyOwner = mpOfRoad.PropertyOwner;
                        certificate.StandardAddress = mpOfRoad.StandardAddress;
                        certificate.FCZAddress = mpOfRoad.FCZAddress;
                        certificate.TDZAddress = mpOfRoad.TDZAddress;
                        certificate.YYZZAddress = mpOfRoad.YYZZAddress;
                        certificate.OtherAddress = mpOfRoad.OtherAddress;
                        certificate.CountyID = mpOfRoad.CountyID;
                        certificate.CountyName = mpOfRoad.CountyID.Split('.').Last();
                        certificate.NeighborhoodsID = mpOfRoad.NeighborhoodsID;
                        certificate.NeighborhoodsName = mpOfRoad.NeighborhoodsID.Split('.').Last();
                        certificate.CommunityName = mpOfRoad.CommunityName;
                        certificate.RoadName = mpOfRoad.RoadName;
                        certificate.MPNumber = mpOfRoad.MPNumber;
                        certificate.OriginalMPAddress = mpOfRoad.OriginalMPAddress;
                        certificate.ResidenceName = mpOfRoad.ResidenceName;
                        certificate.AddressCoding = mpOfRoad.AddressCoding;

                        if (CertificateType == Enums.CertificateType.MPZ)
                            mpOfRoad.MPZPrintComplete = Enums.Complete.Yes;
                        else if (CertificateType == Enums.CertificateType.Placename)
                            mpOfRoad.DZZMPrintComplete = Enums.Complete.Yes;

                        BaseUtils.UpdateAddressCode(null, mpOfRoad, null, null, Enums.TypeInt.Road);
                    }
                    else if (MPType == Enums.MPTypeCh.Country)
                    {
                        var mpOfCounty = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
                        if (mpOfCounty == null)
                            throw new Exception($"ID为{ID}的农村门牌已经注销，请重新查询！");

                        certificate.PropertyOwner = mpOfCounty.PropertyOwner;
                        certificate.StandardAddress = mpOfCounty.StandardAddress;
                        certificate.TDZAddress = mpOfCounty.TDZAddress;
                        certificate.QQZAddress = mpOfCounty.QQZAddress;
                        certificate.OtherAddress = mpOfCounty.OtherAddress;
                        certificate.CountyID = mpOfCounty.CountyID;
                        certificate.CountyName = mpOfCounty.CountyID.Split('.').Last();
                        certificate.NeighborhoodsID = mpOfCounty.NeighborhoodsID;
                        certificate.NeighborhoodsName = mpOfCounty.NeighborhoodsID.Split('.').Last();
                        certificate.CommunityName = mpOfCounty.CommunityName;
                        certificate.ViligeName = mpOfCounty.ViligeName;
                        certificate.MPNumber = mpOfCounty.MPNumber;
                        certificate.HSNumber = mpOfCounty.HSNumber;
                        certificate.AddressCoding = mpOfCounty.AddressCoding;

                        if (CertificateType == Enums.CertificateType.MPZ)
                            mpOfCounty.MPZPrintComplete = Enums.Complete.Yes;
                        else if (CertificateType == Enums.CertificateType.Placename)
                            mpOfCounty.DZZMPrintComplete = Enums.Complete.Yes;

                        BaseUtils.UpdateAddressCode(null, null, mpOfCounty, null, Enums.TypeInt.Country);
                    }
                    certificates.Add(certificate);
                }
                dbContext.MPOfCertificate.AddRange(mpOfCertificates);
                dbContext.SaveChanges();
                return certificates;
            }
        }


        /// <summary>
        /// 过滤非法字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceBadChar(string src)
        {
            if (src == null)
                return "";
            StringBuilder result = new StringBuilder();
            if (src != null)
            {
                src = src.Trim();
                for (int pos = 0; pos < src.Length; pos++)
                {
                    switch (src[pos])
                    {
                        case '\"': result.Append(""); break;
                        case '\n': result.Append(""); break;
                        case '\r': result.Append(""); break;
                        case '\t': result.Append(""); break;
                        case '<': result.Append(""); break;
                        case '>': result.Append(""); break;
                        case '\'': result.Append(""); break;
                        case '&': result.Append(""); break;
                        case '%': result.Append(""); break;
                        case '_': result.Append(""); break;
                        case '#': result.Append(""); break;
                        case '?': result.Append(""); break;
                        default: result.Append(src[pos]); break;
                    }
                }
            }
            return result.ToString();
        }


        //************************************不使用*****************************************
        /// <summary>
        /// 读取合并的pdf文件名称
        /// </summary>
        /// <param name="Directorypath">目录</param>
        /// <param name="outpath">导出的路径</param>
        public static void MergePDF(string Directorypath)
        {
            List<string> filelist2 = new List<string>();
            System.IO.DirectoryInfo di2 = new System.IO.DirectoryInfo(Directorypath);
            FileInfo[] ff2 = di2.GetFiles("*.pdf");
            foreach (FileInfo temp in ff2)
            {
                filelist2.Add(Directorypath + "\\" + temp.Name);
            }
            mergePDFFiles(filelist2);
            //DeleteAllPdf(Directorypath);
        }
        /// <summary>
        /// 合成pdf文件
        /// </summary>
        /// <param name="fileList">文件名list</param>
        /// <param name="outMergeFile">输出路径</param>
        public static void mergePDFFiles(List<string> fileList)
        {
            DeleteAllPdf(StaticVariable.MergeFilePath);
            PdfReader reader;
            //iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(144, 720);
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(StaticVariable.MergeFile, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            for (int i = 0; i < fileList.Count; i++)
            {
                reader = new PdfReader(fileList[i]);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
        }
        public static void DeleteAllPdf(string Directorypath)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Directorypath);
            if (di.Exists)
            {
                FileInfo[] ff = di.GetFiles("*.pdf");
                foreach (FileInfo temp in ff)
                {
                    File.Delete(Directorypath + "\\" + temp.Name);
                }
            }
        }
    }
}