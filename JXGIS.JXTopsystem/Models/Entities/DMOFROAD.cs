using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("DMOFROAD")]
    public class DMOFROAD
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
        public string RoadDirection { get; set; }
        public string RoadStart { get; set; }
        public string RoadEnd { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public string RoadNature { get; set; }
        public string SJTime { get; set; }
        public string JCTime { get; set; }
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
        public int? IsFinish { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public List<SPFile> lxpfss { get; set; }
        [NotMapped]
        public List<SPFile> dltzs { get; set; }
    }
}