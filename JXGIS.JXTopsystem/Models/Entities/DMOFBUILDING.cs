using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("DMOFBUILDING")]
    public class DMOFBUILDING
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityName { get; set; }
        public string Type { get; set; }
        public string Postcode { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name { get; set; }
        public string RomanSpell { get; set; }
        public string TMRomanSpell { get; set; }
        public string SBDW { get; set; }
        public string SHXYDM { get; set; }
        public string PZDW { get; set; }
        public DateTime? PFTime { get; set; }
        public string PFWH { get; set; }
        public string East { get; set; }
        public string South { get; set; }
        public string West { get; set; }
        public string North { get; set; }
        public double? PositionX { get; set; }
        public double? PositionY { get; set; }
        public double? ZDArea { get; set; }
        public double? JZArea { get; set; }
        public double? Height { get; set; }
        public double? FloorNum { get; set; }
        public DateTime? JCTime { get; set; }
        public string DLSTGK { get; set; }
        public string DMLL { get; set; }
        public string DMHY { get; set; }
        public string ZLLY { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string ApplicantAddress { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? SHTime { get; set; }
        public string SHUser { get; set; }
        public string SHResult { get; set; }
        public DateTime? SPTime { get; set; }
        public string SPUser { get; set; }
        public string UseState { get; set; }
        public DateTime? GMTime { get; set; }
        public string GMUser { get; set; }
        public DateTime? XMTime { get; set; }
        public string XMUser { get; set; }
        public string XMWH { get; set; }
        public string CheckUser { get; set; }
        public string SBLY { get; set; }
        public string ProjID { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public List<SPFile> jsydxkzs { get; set; }
        [NotMapped]
        public List<SPFile> jsgcghxkzs { get; set; }
        [NotMapped]
        public List<SPFile> zpmts { get; set; }
        [NotMapped]
        public List<SPFile> xgts { get; set; }
    }
}