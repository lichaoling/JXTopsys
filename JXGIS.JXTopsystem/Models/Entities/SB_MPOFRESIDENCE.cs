using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SB_MPOFRESIDENCE")]
    public class SB_MPOFRESIDENCE
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityName { get; set; }
        public string MPNumber { get; set; }
        public string ResidenceName { get; set; }
        public string LZNumber { get; set; }
        public string DYNumber { get; set; }
        public double? DYPositionX { get; set; }
        public double? DYPositionY { get; set; }
        public string HSNumber { get; set; }
        public string Postcode { get; set; }
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
        public string OtherAddress { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string MailAddress { get; set; }
        public DateTime? BZTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public string CheckUser { get; set; }
        public string SBLY { get; set; }
        public string ProjID { get; set; }
        public int? IsFinish { get; set; }
        public string Remark { get; set; }

    }
}