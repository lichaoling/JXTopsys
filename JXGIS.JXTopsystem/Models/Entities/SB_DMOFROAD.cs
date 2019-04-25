using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SB_DMOFROAD")]
    public class SB_DMOFROAD
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
        public string SBDW { get; set; }
        public string SHXYDM { get; set; }
        public string RoadDirection { get; set; }
        public string RoadStart { get; set; }
        public string RoadEnd { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public string RoadNature { get; set; }
        public DateTime? SJTime { get; set; }
        public DateTime? JCTime { get; set; }
        public string DLSTGK { get; set; }
        public string DMHY { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string ApplicantAddress { get; set; }
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
        public List<SPFile> lxpfss { get; set; }
        [NotMapped]
        public List<SPFile> dltzs { get; set; }
    }
}