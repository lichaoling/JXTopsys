using JXGIS.JXTopsystem.Controllers;
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
        public static readonly string auxiliaryone_FBY = "非必要";
        public static readonly string auxiliaryone_BY = "必要";
        public static readonly string retentionperio = "30年";
        public static readonly string departmentNum = "33040113";
        public static readonly string gd_dz = "电子归档";
        public static readonly string gd_zz = "纸质归档";

        public static void SearchMPArchiveStatus(string DistrictID,int ArchiveState, DateTime? start, DateTime? end)
        {

        }




        public static void ArchiveMPFile(DateTime? start, DateTime? end)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var uploadFilesIDs = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Select(t => t.MPID).Distinct().ToList();

                #region 住宅门牌
                var residencemps = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ArchiveFileStatus == null).Where(t => uploadFilesIDs.Contains(t.ID));//必须是有上传过附件的

                if (start != null || end != null)
                {
                    if (start != null)
                        residencemps = residencemps.Where(t => t.BZTime >= start);
                    if (end != null)
                        residencemps = residencemps.Where(t => t.BZTime <= end);
                }
                var residencedatas = residencemps.ToList();
                List<directory> directorys = new List<directory>();
                foreach (var data in residencedatas)
                {
                    var docTypes = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == data.ID).Select(t => t.DocType).Distinct().ToList();
                    var deptname = GetDeptname(data.CountyID);
                    var FileName = GetDocumentNumber(ArchiveFileUtils.eventtypeid.mpzm, ArchiveFileUtils.eventtypebigid.mpzm, ArchiveFileUtils.eventtypesmallid, data.ID, data.BZTime);
                    var FilePath = Path.Combine(StaticVariable.basePathArchiveFile, FileName);
                    var projid = "33040113" + DateTime.Now.Year.ToString().Substring(2) + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0" + FileName.Split('-').Last();
                    var pwd = GenerateRandomCode(6);
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    XmlTextWriter xmlWriter;
                    #region 归档配置表
                    string strFileName = Path.Combine(FilePath, "归档配置表.XML");
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

                    var SQCLPath = Path.Combine(FilePath, "申请材料");
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    foreach (var docType in docTypes)
                    {
                        CopyMPUploadFilesToDest(data.ID, docType, SQCLPath, Enums.TypeInt.Residence);//将申请门牌时上传的证件复制到申请材料文件夹中
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
                    xmlWriter.WriteAttributeString("nodename", "申请");
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteString(data.Applicant);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteString("申请意见");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Body");
                    xmlWriter.WriteString("同意");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Modified");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

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
                        documentnumber = FileName,
                        projectname = $"{data.StandardAddress}-" + ArchiveFileUtils.servicename.mpzm,
                        archivetime = gdsj,
                        daptname = deptname
                    });
                    data.ArchiveFileStatus = FileName;
                    dbContext.SaveChanges();
                }
                #endregion

                #region 道路门牌
                var roadmps = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ArchiveFileStatus == null).Where(t => uploadFilesIDs.Contains(t.ID));//必须是有上传过附件的
                if (start != null || end != null)
                {
                    if (start != null)
                        roadmps = roadmps.Where(t => t.BZTime >= start);
                    if (end != null)
                        roadmps = roadmps.Where(t => t.BZTime <= end);
                }
                var roaddatas = roadmps.ToList();
                foreach (var data in roaddatas)
                {
                    var docTypes = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == data.ID).Select(t => t.DocType).Distinct().ToList();
                    var deptname = GetDeptname(data.CountyID);
                    var FileName = GetDocumentNumber(ArchiveFileUtils.eventtypeid.mpzm, ArchiveFileUtils.eventtypebigid.mpzm, ArchiveFileUtils.eventtypesmallid, data.ID, data.BZTime);
                    var FilePath = Path.Combine(StaticVariable.basePathArchiveFile, FileName);
                    var projid = "33040113" + DateTime.Now.Year.ToString().Substring(2) + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0" + FileName.Split('-').Last();
                    var pwd = GenerateRandomCode(6);
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    XmlTextWriter xmlWriter;
                    #region 归档配置表
                    string strFileName = Path.Combine(FilePath, "归档配置表.XML");
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

                    var SQCLPath = Path.Combine(FilePath, "申请材料");
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    foreach (var docType in docTypes)
                    {
                        CopyMPUploadFilesToDest(data.ID, docType, SQCLPath, Enums.TypeInt.Road);//将申请门牌时上传的证件复制到申请材料文件夹中
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
                    strFileName = Path.Combine(FilePath, "基本信息描述.XML");
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
                    strFileName = Path.Combine(FilePath, "办理流程.XML");
                    xmlWriter = new XmlTextWriter(strFileName, Encoding.UTF8);//创建一个xml文档
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Opinions");

                    xmlWriter.WriteStartElement("Opinion");
                    xmlWriter.WriteAttributeString("nodename", "申请");
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteString(data.Applicant);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteString("申请意见");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Body");
                    xmlWriter.WriteString("同意");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Modified");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

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
                        documentnumber = FileName,
                        projectname = $"{data.StandardAddress}-" + ArchiveFileUtils.servicename.mpzm,
                        archivetime = gdsj,
                        daptname = deptname
                    });
                    data.ArchiveFileStatus = FileName;
                    dbContext.SaveChanges();
                }
                #endregion

                #region 农村门牌
                var countrymps = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ArchiveFileStatus == null).Where(t => uploadFilesIDs.Contains(t.ID));//必须是有上传过附件的
                if (start != null || end != null)
                {
                    if (start != null)
                        roadmps = roadmps.Where(t => t.BZTime >= start);
                    if (end != null)
                        roadmps = roadmps.Where(t => t.BZTime <= end);
                }
                var countrydatas = roadmps.ToList();
                foreach (var data in countrydatas)
                {
                    var docTypes = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == data.ID).Select(t => t.DocType).Distinct().ToList();
                    var deptname = GetDeptname(data.CountyID);
                    var FileName = GetDocumentNumber(ArchiveFileUtils.eventtypeid.mpzm, ArchiveFileUtils.eventtypebigid.mpzm, ArchiveFileUtils.eventtypesmallid, data.ID, data.BZTime);
                    var FilePath = Path.Combine(StaticVariable.basePathArchiveFile, FileName);
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

                    var SQCLPath = Path.Combine(FilePath, "申请材料");
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    foreach (var docType in docTypes)
                    {
                        CopyMPUploadFilesToDest(data.ID, docType, SQCLPath, Enums.TypeInt.Country);//将申请门牌时上传的证件复制到申请材料文件夹中
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
                    xmlWriter.WriteAttributeString("nodename", "申请");
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteString(data.Applicant);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteString("申请意见");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Body");
                    xmlWriter.WriteString("同意");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Modified");
                    xmlWriter.WriteString(((DateTime)data.BZTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

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
                        documentnumber = FileName,
                        projectname = $"{data.StandardAddress}-" + ArchiveFileUtils.servicename.mpzm,
                        archivetime = gdsj,
                        daptname = deptname
                    });
                    data.ArchiveFileStatus = FileName;
                    dbContext.SaveChanges();
                }
                #endregion

                #region 地名证明或门牌证打印

                var zm = dbContext.MPOfCertificate.Where(t => t.ArchiveFileStatus == null);
                var certificatesDM = (from s in zm

                                      join a in dbContext.MPOfResidence
                                      on s.MPID equals a.ID into aa
                                      from at in aa.DefaultIfEmpty()
                                      where s.MPType == Enums.MPTypeCh.Residence
                                      select new Certificate
                                      {
                                          AddressCoding = at.AddressCoding,
                                          CountyID = at.CountyID,
                                          NeighborhoodsID = at.NeighborhoodsID,
                                          Applicant = at.Applicant,
                                          ApplicantPhone = at.ApplicantPhone,
                                          IDType = at.IDType,
                                          IDNumber = at.IDNumber,
                                          ID = s.ID,
                                          CreateUser = s.CreateUser,
                                          CreateTime = s.CreateTime,
                                          CertificateType = s.CertificateType,
                                          MPType = s.MPType,
                                          StandardAddress = at.StandardAddress,
                                      }).ToList().Concat(
                             from s in zm

                             join b in dbContext.MPOfRoad
                             on s.MPID equals b.ID into bb
                             from bt in bb.DefaultIfEmpty()
                             where s.MPType == Enums.MPTypeCh.Road
                             select new Certificate
                             {
                                 AddressCoding = bt.AddressCoding,
                                 CountyID = bt.CountyID,
                                 NeighborhoodsID = bt.NeighborhoodsID,
                                 Applicant = bt.Applicant,
                                 ApplicantPhone = bt.ApplicantPhone,
                                 IDType = bt.IDType,
                                 IDNumber = bt.IDNumber,
                                 ID = s.ID,
                                 CreateUser = s.CreateUser,
                                 CreateTime = s.CreateTime,
                                 CertificateType = s.CertificateType,
                                 MPType = s.MPType,
                                 StandardAddress = bt.StandardAddress,
                             }).ToList().Concat(
                             from s in zm

                             join c in dbContext.MPOfCountry
                             on s.MPID equals c.ID into cc
                             from ct in cc.DefaultIfEmpty()
                             where s.MPType == Enums.MPTypeCh.Country
                             select new Certificate
                             {
                                 AddressCoding = ct.AddressCoding,
                                 CountyID = ct.CountyID,
                                 NeighborhoodsID = ct.NeighborhoodsID,
                                 Applicant = ct.Applicant,
                                 ApplicantPhone = ct.ApplicantPhone,
                                 IDType = ct.IDType,
                                 IDNumber = ct.IDNumber,
                                 ID = s.ID,
                                 CreateUser = s.CreateUser,
                                 CreateTime = s.CreateTime,
                                 CertificateType = s.CertificateType,
                                 MPType = s.MPType,
                                 StandardAddress = ct.StandardAddress,
                             });
                if (start != null || end != null)
                {
                    if (start != null)
                        certificatesDM = certificatesDM.Where(t => t.CreateTime >= start);
                    if (end != null)
                        certificatesDM = certificatesDM.Where(t => t.CreateTime <= end);
                }
                var certificatesDMData = certificatesDM.ToList();
                foreach (var data in certificatesDMData)
                {
                    var deptname = GetDeptname(data.CountyID);
                    var eventtypeid = data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.eventtypeid.dmzm : ArchiveFileUtils.eventtypeid.mpzm;
                    var eventtypebigid = data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.eventtypebigid.dmzm : ArchiveFileUtils.eventtypebigid.mpzm;
                    var FileName = GetDocumentNumber(eventtypeid, eventtypebigid, ArchiveFileUtils.eventtypesmallid, data.ID, data.CreateTime);
                    var FilePath = Path.Combine(StaticVariable.basePathArchiveFile, FileName);
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
                    xmlWriter.WriteString(data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.servicecode.dmzm : ArchiveFileUtils.servicecode.mpzm);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("servicename");
                    xmlWriter.WriteAttributeString("title", "权力事项名称");
                    xmlWriter.WriteString(data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.servicename.dmzm : ArchiveFileUtils.servicename.mpzm);
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
                    xmlWriter.WriteAttributeString("title", "证明材料");

                    var ZMCLPath = Path.Combine(FilePath, "证明材料");
                    if (!Directory.Exists(ZMCLPath))
                    {
                        Directory.CreateDirectory(ZMCLPath);
                    }

                    CopyDMZMandMPZToDest(data.CertificateType, data.MPType, data.ID, data.StandardAddress, ZMCLPath);//将地名证明或门牌证复制到证明材料文件夹

                    xmlWriter.WriteStartElement("filename");
                    xmlWriter.WriteAttributeString("title", "归档材料名称");
                    xmlWriter.WriteString(data.CertificateType == Enums.CertificateType.Placename ? "地名证明" : "门牌证明");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("auxiliaryone");
                    xmlWriter.WriteAttributeString("title", "归档配置辅助信息一");
                    xmlWriter.WriteString(auxiliaryone_BY);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("deptid");
                    xmlWriter.WriteAttributeString("title", "部门编码");
                    xmlWriter.WriteString(GetDeptid(data.CountyID));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("eventtype");
                    xmlWriter.WriteAttributeString("title", "权力事项类型");
                    xmlWriter.WriteString(data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.eventtype.dmzm : ArchiveFileUtils.eventtype.mpzm);
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
                    xmlWriter.WriteString(data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.eventtype.dmzm : ArchiveFileUtils.eventtype.mpzm);
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
                    xmlWriter.WriteString($"{data.StandardAddress}-" + (data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.servicename.dmzm : ArchiveFileUtils.servicename.mpzm));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("receivedepartmentname");
                    xmlWriter.WriteAttributeString("title", "承办单位");
                    xmlWriter.WriteString(ArchiveFileUtils.deptname.smzj);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("receivetime");
                    xmlWriter.WriteAttributeString("title", "受理（立案）时间");
                    xmlWriter.WriteString(((DateTime)data.CreateTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("transacttime");
                    xmlWriter.WriteAttributeString("title", "办结时间");
                    xmlWriter.WriteString(((DateTime)data.CreateTime).ToString("yyyy-mm-dd hh: mm:ss"));
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
                    xmlWriter.WriteAttributeString("nodename", "申请");
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteString(data.Applicant);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteString("申请意见");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Body");
                    xmlWriter.WriteString("同意");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Modified");
                    xmlWriter.WriteString(((DateTime)data.CreateTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

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
                    xmlWriter.WriteString(((DateTime)data.CreateTime).ToString("yyyy-mm-dd hh: mm:ss"));
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
                    xmlWriter.WriteString(((DateTime)data.CreateTime).ToString("yyyy-mm-dd hh: mm:ss"));
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
                    xmlWriter.WriteString(((DateTime)data.CreateTime).ToString("yyyy-mm-dd hh: mm:ss"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.Close();
                    #endregion

                    directorys.Add(new directory()
                    {
                        documentnumber = FileName,
                        projectname = $"{data.StandardAddress}-" + (data.CertificateType == Enums.CertificateType.Placename ? ArchiveFileUtils.servicename.dmzm : ArchiveFileUtils.servicename.mpzm),
                        archivetime = gdsj,
                        daptname = deptname
                    });
                    data.ArchiveFileStatus = FileName;
                    dbContext.SaveChanges();
                }

                #endregion

                #region 存档信息包目录规范
                XmlTextWriter xmlWriterDirectory;
                var name = $"存档信息包目录清单-（{departmentNum}）-（{DateTime.Now.ToString("yyyyMMdd")}";
                var strFileNameDirectory = Path.Combine(StaticVariable.basePathArchiveFile, name + ".XML"); //DateTime.Now.ToShortDateString()只显示日期 xxxx-xx-xx 一个是短日期
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
        private static string GetNewPathForDupes(string path)
        {
            string newFullPath = path.Trim();
            if (System.IO.File.Exists(path))
            {
                string directory = Path.GetDirectoryName(path);
                string filename = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                int counter = 1;
                do
                {
                    string newFilename = string.Format("{0}{1}{2}", filename, counter, extension);
                    newFullPath = Path.Combine(directory, newFilename);
                    counter++;
                } while (System.IO.File.Exists(newFullPath));
            }
            return newFullPath;
        }
        private static void CopyMPUploadFilesToDest(string MPID, string docType, string SQCLPath, int MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var MPRelativePath = MPType == Enums.TypeInt.Residence ? StaticVariable.residenceMPPath : (MPType == Enums.TypeInt.Road ? StaticVariable.roadMPPath : StaticVariable.countryMPPath);
                var pictures = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == MPID).Where(t => t.DocType == docType).ToList();
                var picturename = GetFilename(docType);
                foreach (var picture in pictures)
                {
                    var PName = picturename + picture.FileEx;
                    var PPath = Path.Combine(SQCLPath, PName);
                    var PNewPath = GetNewPathForDupes(PPath);//如果重名就在名字后加数字
                    var srcPath = Path.Combine(MPRelativePath, MPID, picture.ID + picture.FileEx);
                    File.Copy(srcPath, PNewPath);
                }
            }
        }
        private static void CopyDMZMandMPZToDest(string CertificateType, string MPType, string MPID, string StandardAddress, string ZMCLPath)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var a = CertificateType == Enums.CertificateType.Placename ? StaticVariable.DZZMPrintPath : StaticVariable.MPZPrintPath;
                var b = MPType == Enums.MPTypeCh.Residence ? Enums.MPTypeCh.Residence : (MPType == Enums.MPTypeCh.Road ? Enums.MPTypeCh.Road : Enums.MPTypeCh.Country);
                var srcPath = Path.Combine(a, b, MPID, StandardAddress + "-地址证明.pdf");
                var destPath = Path.Combine(ZMCLPath, StandardAddress + "-地址证明.pdf");
                File.Copy(srcPath, destPath);
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