using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.ArchiveFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace JXGIS.JXTopsystem.Business.ArchiveFile
{
    public class ArchiveFile
    {
        public static readonly string ArchiveFileBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "ArchiveFiles");

        public static readonly string auxiliaryone_FBY = "非必要";
        public static readonly string auxiliaryone_BY = "必要";
        public static readonly string retentionperio = "30年";
        public static readonly string departmentNum = "33040013";

        public static void ArchiveMPFile(DateTime? start, DateTime? end)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {

                var uploadFilesIDs = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Select(t => t.MPID).Distinct().ToList();
                var mps = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => uploadFilesIDs.Contains(t.ID));//必须是有上传过附件的

                if (start != null || end != null)
                {
                    if (start != null)
                        mps = mps.Where(t => t.BZTime >= start);
                    if (end != null)
                        mps = mps.Where(t => t.BZTime <= end);
                }
                var datas = mps.ToList();
                foreach (var data in datas)
                {
                    var docTypes = dbContext.MPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPID == data.ID).Select(t => t.DocType).Distinct().ToList();
                    List<file> files = new List<Models.Extends.ArchiveFile.file>();
                    foreach (var docType in docTypes)
                    {
                        file file = new file();
                        file.filename = GetFilename(docType);
                        file.auxiliaryone = auxiliaryone_FBY;
                        files.Add(file);
                    }
                    List<Filingrange> filingranges = new List<Models.Extends.ArchiveFile.Filingrange>();
                    Filingrange filingrange = new Models.Extends.ArchiveFile.Filingrange();
                    filingrange.file = files;
                    filingranges.Add(filingrange);

                    var deptname = GetDeptname(data.CountyID);

                    ArchiveConfigTab archiveConfigTab = new ArchiveConfigTab()
                    {
                        servicecode = ArchiveFileUtils.servicecode.mpzm,
                        servicename = ArchiveFileUtils.servicename.mpzm,
                        deptname = deptname,
                        retentionperio = retentionperio,
                        version = "1",
                        filingranges = filingranges,
                        deptid = GetDeptid(data.CountyID),
                        eventtype = ArchiveFileUtils.eventtype.mpzm
                    };
                    BaseInfoDescription baseInfoDescription = new Models.Extends.ArchiveFile.BaseInfoDescription()
                    {
                        deptname = deptname,
                        documentnumber = GetDocumentnumber(ArchiveFileUtils.eventtypeid.mpzm, ArchiveFileUtils.eventtypebigid.dmzm, ArchiveFileUtils.eventtypesmallid, data.BZTime),
                        retentionperiod = retentionperio,


                    };


                }

                string fileName = "";
                using (Stream fStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    ArchiveConfigTab ArchiveConfigTab = new Models.Extends.ArchiveFile.ArchiveConfigTab();
                    XmlSerializer xmlFormat = new XmlSerializer(typeof(ArchiveConfigTab));
                    xmlFormat.Serialize(fStream, ArchiveConfigTab);
                }
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
        public static string GetDocumentnumber(string eventtypeid, string eventtypebigid, string eventtypesmallid, DateTime? time)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                ARCHIVEFILE archiveFile = new Models.Entities.ARCHIVEFILE()
                {
                    ID = Guid.NewGuid().ToString(),
                    DOCUMENTNUMBER = $"{departmentNum}-{eventtypeid}-{eventtypebigid}-{eventtypesmallid}-{((DateTime)time).Year.ToString()}-{retentionperio}"
                };
                dbContext.ArchiveFile.Add(archiveFile);
                var documentNum = dbContext.ArchiveFile.Where(t => t.ID == archiveFile.ID).Select(t => t.DOCUMENTNUMBER).FirstOrDefault();
                return documentNum;
            }
        }
    }
}