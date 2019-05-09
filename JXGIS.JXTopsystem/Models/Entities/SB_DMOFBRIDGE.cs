using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SB_DMOFBRIDGE")]
    public class SB_DMOFBRIDGE
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
        public string Road { get; set; }
        public string CrossRiver { get; set; }
        public double? PositionX { get; set; }
        public double? PositionY { get; set; }
        public string BridgeDirection { get; set; }
        public double? ZZL { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? LDBG { get; set; }
        public double? ZDKD { get; set; }
        public string BridgeNature { get; set; }
        public string SJTime { get; set; }
        public string JCTime { get; set; }
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
        public string LastModifyUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        [NotMapped]
        public List<SPFile> lxpfss { get; set; }
        [NotMapped]
        public List<SPFile> qltzs { get; set; }
    }
}