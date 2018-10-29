using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.ArchiveFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace JXGIS.JXTopsystem.Business.ArchiveFile
{
    public class ArchiveFile
    {
        public static readonly string ArchiveFileBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "ArchiveFiles");

        public static readonly string auxiliaryone_FBY = "非必要";
        public static readonly string auxiliaryone_BY = "必要";
        public static readonly string retentionperio = "30年";
        public static readonly string departmentNum = "33040113";
        public static readonly string gd_dz = "电子归档";
        public static readonly string gd_zz = "纸质归档";

        public static void ArchiveMPFile(DateTime? start, DateTime? end)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var uploadFilesIDs = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Select(t => t.MPID).Distinct().ToList();
                var mps = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ArchiveFileStatus == null).Where(t => uploadFilesIDs.Contains(t.ID));//必须是有上传过附件的

                if (start != null || end != null)
                {
                    if (start != null)
                        mps = mps.Where(t => t.BZTime >= start);
                    if (end != null)
                        mps = mps.Where(t => t.BZTime <= end);
                }
                var datas = mps.ToList();
                List<directory> directorys = new List<directory>();
                foreach (var data in datas)
                {
                    var docTypes = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == data.ID).Select(t => t.DocType).Distinct().ToList();
                    var deptname = GetDeptname(data.CountyID);
                    var FileName = GetDocumentNumber(ArchiveFileUtils.eventtypeid.mpzm, ArchiveFileUtils.eventtypebigid.dmzm, ArchiveFileUtils.eventtypesmallid, data.ID, data.BZTime);
                    var FilePath = Path.Combine(ArchiveFileBasePath, FileName);
                    var projid = "33040113" + DateTime.Now.Year.ToString().Substring(2) + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0" + FileName.Split('-').Last();
                    var pwd = GenerateRandomCode(6);
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    XmlTextWriter xmlWriter;
                    #region 归档配置表
                    string strFileName = Path.Combine(FilePath, "归档配置表.XML"); ;
                    xmlWriter = new XmlTextWriter(strFileName, Encoding.UTF8);//创建一个xml文档
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("description");
                    xmlWriter.WriteAttributeString("title", "归档配置表");

                    xmlWriter.WriteStartElement("servicecode");
                    xmlWriter.WriteAttributeString("title", "权力事项编码");
                    xmlWriter.WriteString(ArchiveFileUtils.servicecode.mpzm);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("servicename");
                    xmlWriter.WriteAttributeString("title", "权力事项名称");
                    xmlWriter.WriteString(ArchiveFileUtils.servicename.mpzm);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("deptname");
                    xmlWriter.WriteAttributeString("title", "部门名称");
                    xmlWriter.WriteString(deptname);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("retentionperio");
                    xmlWriter.WriteAttributeString("title", "保管期限");
                    xmlWriter.WriteString(retentionperio);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("filingranges");
                    xmlWriter.WriteAttributeString("title", "归档范围");

                    xmlWriter.WriteStartElement("filingrange");
                    xmlWriter.WriteAttributeString("title", "申请材料");
                    foreach (var docType in docTypes)
                    {
                        xmlWriter.WriteStartElement("file");

                        xmlWriter.WriteStartElement("filename");
                        xmlWriter.WriteAttributeString("title", "归档材料名称");
                        xmlWriter.WriteString(GetFilename(docType));
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("auxiliaryone");
                        xmlWriter.WriteAttributeString("title", "归档配置辅助信息一");
                        xmlWriter.WriteString(auxiliaryone_FBY);
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("auxiliarytwo");
                        xmlWriter.WriteAttributeString("title", "归档配置辅助信息二");
                        xmlWriter.WriteString(gd_dz);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("deptid");
                    xmlWriter.WriteAttributeString("title", "部门编码");
                    xmlWriter.WriteString(GetDeptid(data.CountyID));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("eventtype");
                    xmlWriter.WriteAttributeString("title", "权力事项类型");
                    xmlWriter.WriteString(ArchiveFileUtils.eventtype.mpzm);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.Close();
                    #endregion

                    #region 基本信息描述
                    strFileName = Path.Combine(FilePath, "基本信息描述.XML"); ;
                    xmlWriter = new XmlTextWriter(strFileName, Encoding.UTF8);//创建一个xml文档
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("description");
                    xmlWriter.WriteAttributeString("title", "基本信息描述");

                    xmlWriter.WriteStartElement("deptname");
                    xmlWriter.WriteAttributeString("title", "立档单位名称");
                    xmlWriter.WriteString(deptname);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("documentnumber");
                    xmlWriter.WriteAttributeString("title", "电子文件号");
                    xmlWriter.WriteString(FileName);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("retentionperiod");
                    xmlWriter.WriteAttributeString("title", "保管期限");
                    xmlWriter.WriteString(retentionperio);
                    xmlWriter.WriteEndElement();

                    var gdsj = DateTime.Now.ToString("yyyy-mm-dd hh: mm:ss");
                    xmlWriter.WriteStartElement("archivetime");
                    xmlWriter.WriteAttributeString("title", "归档时间");
                    xmlWriter.WriteString(gdsj);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("eventtype");
                    xmlWriter.WriteAttributeString("title", "权力事项类型");
                    xmlWriter.WriteString(ArchiveFileUtils.eventtype.mpzm);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("applyname");
                    xmlWriter.WriteAttributeString("title", "行政相对人名称");
                    xmlWriter.WriteString(data.Applicant);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("mobile");
                    xmlWriter.WriteAttributeString("title", "行政相对人手机");
                    xmlWriter.WriteString(data.ApplicantPhone);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("address");
                    xmlWriter.WriteAttributeString("title", "行政相对人地址");
                    xmlWriter.WriteString(data.StandardAddress);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("contactidcard");
                    xmlWriter.WriteAttributeString("title", "证件号码");
                    xmlWriter.WriteString(data.IDNumber);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("projectname");
                    xmlWriter.WriteAttributeString("title", "办件名称");
                    xmlWriter.WriteString($"{data.StandardAddress}-" + ArchiveFileUtils.servicename.mpzm);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("receivedepartmentname");
                    xmlWriter.WriteAttributeString("title", "承办单位");
                    xmlWriter.WriteString(ArchiveFileUtils.deptname.smzj);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("receivetime");
                    xmlWriter.WriteAttributeString("title", "受理（立案）时间");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("transacttime");
                    xmlWriter.WriteAttributeString("title", "办结时间");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("projid");
                    xmlWriter.WriteAttributeString("title", "业务流水号");
                    xmlWriter.WriteString(projid);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("projectpassword");
                    xmlWriter.WriteAttributeString("title", "查询密码");
                    xmlWriter.WriteString(pwd);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("servicetype");
                    xmlWriter.WriteAttributeString("title", "事项类型");
                    xmlWriter.WriteString(ArchiveFileUtils.servicetype.cnj);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.Close();
                    #endregion

                    #region 流程信息描述
                    strFileName = Path.Combine(FilePath, "办理流程.XML"); ;
                    xmlWriter = new XmlTextWriter(strFileName, Encoding.UTF8);//创建一个xml文档
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Opinions");

                    xmlWriter.WriteStartElement("Opinion");
                    xmlWriter.WriteAttributeString("nodename", "受理");
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteString(data.CreateUser);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteString("受理意见");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Body");
                    xmlWriter.WriteString("同意");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Modified");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Opinion");
                    xmlWriter.WriteAttributeString("nodename", "审核");
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteString(data.CreateUser);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteString("审核意见");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Body");
                    xmlWriter.WriteString("同意");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Modified");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Opinion");
                    xmlWriter.WriteAttributeString("nodename", "决定");
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteString(data.CreateUser);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteString("办结意见");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Body");
                    xmlWriter.WriteString("批准");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Modified");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.Close();
                    #endregion

                    directorys.Add(new directory()
                    {
                        documentnumber= FileName,
                        projectname= $"{data.StandardAddress}-" + ArchiveFileUtils.servicename.mpzm,
                        archivetime=gdsj,
                        daptname= deptname
                    });
                    data.ArchiveFileStatus = FileName;
                    dbContext.SaveChanges();
                }

                #region 存档信息包目录规范
                XmlTextWriter xmlWriterDirectory;
                var name = $"存档信息包目录清单-（{departmentNum}）-（{DateTime.Now.ToString("yyyyMMdd")}";
                var strFileNameDirectory = Path.Combine(ArchiveFileBasePath, name + ".XML"); //DateTime.Now.ToShortDateString()只显示日期 xxxx-xx-xx 一个是短日期
                xmlWriterDirectory = new XmlTextWriter(strFileNameDirectory, Encoding.UTF8);//创建一个xml文档
                xmlWriterDirectory.Formatting = Formatting.Indented;
                xmlWriterDirectory.WriteStartDocument();
                xmlWriterDirectory.WriteStartElement("description");
                xmlWriterDirectory.WriteAttributeString("title", name);

                xmlWriterDirectory.WriteStartElement("systemcode");
                xmlWriterDirectory.WriteAttributeString("title", "系统编码");
                xmlWriterDirectory.WriteString(departmentNum);
                xmlWriterDirectory.WriteEndElement();

                xmlWriterDirectory.WriteStartElement("batchname");
                xmlWriterDirectory.WriteAttributeString("title", "批次号");
                xmlWriterDirectory.WriteString(DateTime.Now.ToString("yyyyMMdd"));
                xmlWriterDirectory.WriteEndElement();

                xmlWriterDirectory.WriteStartElement("sendtime");
                xmlWriterDirectory.WriteAttributeString("title", "交换日期");
                xmlWriterDirectory.WriteString(DateTime.Now.ToShortDateString());
                xmlWriterDirectory.WriteEndElement();

                xmlWriterDirectory.WriteStartElement("sendnumber");
                xmlWriterDirectory.WriteAttributeString("title", "存档包数量");
                xmlWriterDirectory.WriteString(directorys.Count().ToString());
                xmlWriterDirectory.WriteEndElement();

                xmlWriterDirectory.WriteStartElement("directories");
                xmlWriterDirectory.WriteAttributeString("title", "存档信息包明细");
                foreach (var d in directorys)
                {
                    xmlWriterDirectory.WriteStartElement("directorie");
                    xmlWriterDirectory.WriteStartElement("documentnumber");
                    xmlWriterDirectory.WriteAttributeString("title", "电子文件号");
                    xmlWriterDirectory.WriteString(d.documentnumber);
                    xmlWriterDirectory.WriteEndElement();
                    xmlWriterDirectory.WriteStartElement("projectname");
                    xmlWriterDirectory.WriteAttributeString("title", "办件名称");
                    xmlWriterDirectory.WriteString(d.projectname);
                    xmlWriterDirectory.WriteEndElement();
                    xmlWriterDirectory.WriteStartElement("archivetime");
                    xmlWriterDirectory.WriteAttributeString("title", "归档时间");
                    xmlWriterDirectory.WriteString(d.archivetime);
                    xmlWriterDirectory.WriteEndElement();
                    xmlWriterDirectory.WriteStartElement("daptname");
                    xmlWriterDirectory.WriteAttributeString("title", "部门名称");
                    xmlWriterDirectory.WriteString(d.daptname);
                    xmlWriterDirectory.WriteEndElement();
                    xmlWriterDirectory.WriteEndElement();
                }
                xmlWriterDirectory.WriteEndElement();
                xmlWriterDirectory.WriteEndElement();
                xmlWriterDirectory.Close();
                #endregion
            }
        }

        public static string GetFilename(string docType)
        {
            switch (docType)
            {
                case Enums.DocType.FCZ:
                    return "房产证";
                case Enums.DocType.TDZ:
                    return "土地证";
                case Enums.DocType.BDCZ:
                    return "不动产证";
                case Enums.DocType.HJ:
                    return "户籍";
                case Enums.DocType.YYZZ:
                    return "营业执照";
                case Enums.DocType.QQZ:
                    return "确权证";
                default:
                    return "其他";
            }
        }
        public static string GenerateRandomCode(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }
        public static string GetDeptname(string CountyID)
        {
            if (CountyID.Contains("南湖区"))
                return ArchiveFileUtils.deptname.nhqmzj;
            else if (CountyID.Contains("秀洲区"))
                return ArchiveFileUtils.deptname.xzqmzj;
            else if (CountyID.Contains("海宁市"))
                return ArchiveFileUtils.deptname.hnsmzj;
            else if (CountyID.Contains("平湖市"))
                return ArchiveFileUtils.deptname.phsmzj;
            else if (CountyID.Contains("桐乡市"))
                return ArchiveFileUtils.deptname.txsmzj;
            else if (CountyID.Contains("嘉善县"))
                return ArchiveFileUtils.deptname.jsxmzj;
            else if (CountyID.Contains("海盐县"))
                return ArchiveFileUtils.deptname.hyxmzj;
            else
                return ArchiveFileUtils.deptname.smzj;
        }
        public static string GetDeptid(string CountyID)
        {
            if (CountyID.Contains("南湖区"))
                return ArchiveFileUtils.deptid.nhqmzj;
            else if (CountyID.Contains("秀洲区"))
                return ArchiveFileUtils.deptid.xzqmzj;
            else if (CountyID.Contains("海宁市"))
                return ArchiveFileUtils.deptid.hnsmzj;
            else if (CountyID.Contains("平湖市"))
                return ArchiveFileUtils.deptid.phsmzj;
            else if (CountyID.Contains("桐乡市"))
                return ArchiveFileUtils.deptid.txsmzj;
            else if (CountyID.Contains("嘉善县"))
                return ArchiveFileUtils.deptid.jsxmzj;
            else if (CountyID.Contains("海盐县"))
                return ArchiveFileUtils.deptid.hyxmzj;
            else
                return ArchiveFileUtils.deptid.smzj;
        }
        public static string GetDocumentNumber(string eventtypeid, string eventtypebigid, string eventtypesmallid, string EventID, DateTime? time)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                ARCHIVEFILE archiveFile = new Models.Entities.ARCHIVEFILE()
                {
                    ID = Guid.NewGuid().ToString(),
                    DOCUMENTNUMBER = $"{departmentNum}-{eventtypeid}-{eventtypebigid}-{eventtypesmallid}-{((DateTime)time).Year.ToString()}-{retentionperio}",
                    EVENTID = EventID
                };
                dbContext.ArchiveFile.Add(archiveFile);
                dbContext.SaveChanges();
                var documentNum = dbContext.ArchiveFile.Where(t => t.ID == archiveFile.ID).Select(t => t.DOCUMENTNUMBER).FirstOrDefault();
                return documentNum;
            }
        }
    }

    public class directory
    {
        public string documentnumber { get; set; }
        public string projectname { get; set; }
        public string archivetime { get; set; }
        public string daptname { get; set; }
    }

}