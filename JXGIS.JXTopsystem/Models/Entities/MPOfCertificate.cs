using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    /// <summary>
    /// 地名证明
    /// </summary>
    [Table("MPOFCERTIFICATE")]
    public class MPOfCertificate
    {
        [Key]
        public string ID { get; set; }
        public string MPID { get; set; }
        public string MPType { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public string Window { get; set; }
        public string CertificateType { get; set; }
        /// <summary>
        /// 电子文件归档状态 0未归档  1已归档
        /// </summary>
        public string ArchiveFileStatus { get; set; }
        /// <summary>
        /// 办件回传状态 0未上报  1已上报
        /// </summary>
        public int InfoReportStatus { get; set; }
        /// <summary>
        /// 数据上报省系统 0未上报 1已上报
        /// </summary>
        public int DataPushStatus { get; set; }
        /// <summary>
        /// 申报来源：线上一窗受理平台收件  线下一窗受理平台申报   部门业务系统窗口收件
        /// </summary>
        public string SBLY { get; set; }
        /// <summary>
        /// projid
        /// </summary>
        public string ProjID { get; set; }
        public DateTime? InfoReportTime { get; set; }
        public DateTime? ArchiveFileTime { get; set; }
        public DateTime? DataPushTime { get; set; }
    }
}