using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("BG_MPOFROAD")]
    public class BG_MPOFROAD
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public string MPID { get; set; }
        [NotMapped]
        public MPOfRoad O_Entity { get; set; }
        public string PropertyOwner { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string ShopName { get; set; }
        public string FCZAddress { get; set; }
        public string FCZNumber { get; set; }
        public string TDZAddress { get; set; }
        public string TDZNumber { get; set; }
        public string YYZZAddress { get; set; }
        public string YYZZNumber { get; set; }
        public string CertificateType { get; set; }
        public string CertificateAddress { get; set; }
        public string CertificateNumber { get; set; }
        public string OtherAddress { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string MailAddress { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public string CheckUser { get; set; }
        public string SBLY { get; set; }
        public string ProjID { get; set; }
        public int? IsFinish { get; set; }
        public string Remark { get; set; }
        public List<SPFile> fcz { get; set; }
        public List<SPFile> tdz { get; set; }
        public List<SPFile> yyzz { get; set; }

    }
}