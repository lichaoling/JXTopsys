using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("BG_MPOFRESIDENCE")]
    public class BG_MPOFRESIDENCE
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public string MPID { get; set; }
        [NotMapped]
        public MPOfResidence O_Entity { get; set; }
        public string PropertyOwner { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string FCZAddress { get; set; }
        public string FCZNumber { get; set; }
        public string TDZAddress { get; set; }
        public string TDZNumber { get; set; }
        public string BDCZAddress { get; set; }
        public string BDCZNumber { get; set; }
        public string HJAddress { get; set; }
        public string HJNumber { get; set; }
        public string CertificateType { get; set; }
        public string CertificateAddress { get; set; }
        public string CertificateNumber { get; set; }
        public string OtherAddress { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string MailAddress { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string CreateUser { get; set; }
        public string CheckUser { get; set; }
        public string SBLY { get; set; }
        public string ProjID { get; set; }
        public int? IsFinish { get; set; }
        public string Remark { get; set; }
        public string State { get; set; }
        public string Opinion { get; set; }
        [NotMapped]
        public List<SPFile> fcz { get; set; }
        [NotMapped]
        public List<SPFile> tdz { get; set; }
        [NotMapped]
        public List<SPFile> bdcz { get; set; }
        [NotMapped]
        public List<SPFile> hj { get; set; }
    }
}