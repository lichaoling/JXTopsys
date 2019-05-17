using JXGIS.JXTopsystem.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("BA_DMOFZYSS")]
    public class BA_DMOFZYSS
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityName { get; set; }
        public string Name { get; set; }
        public string Pinyin { get; set; }
        public string ZMPinyin { get; set; }
        public string ZYSSType { get; set; }
        public string SmallType { get; set; }
        public string DMType { get; set; }
        public string XYDM { get; set; }
        public string XMAddress { get; set; }
        public string East { get; set; }
        public string South { get; set; }
        public string West { get; set; }
        public string North { get; set; }
        public double? Lng { get; set; }
        public double? Lat { get; set; }
        public string DLST { get; set; }
        public string DMHY { get; set; }
        public string Applicant { get; set; }
        public string Telephone { get; set; }
        public string ContractAddress { get; set; }
        public string SBDW { get; set; }
        /// <summary>
        /// 申报来源：9：线上一窗受理平台收件  8：线下一窗受理平台申报
        /// </summary>
        public string SBLY { get; set; }
        public string ZGDW { get; set; }
        public DateTime? ApplicantDate { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Postcode { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string CreateUser { get; set; }
        public string CheckUser { get; set; }
        public string LastModifyUser { get; set; }
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string ProjID { get; set; }
        public int? IsFinish { get; set; }
        public string State { get; set; }
        public string Opinion { get; set; }

        public int IsSync { get; set; }
        public int InfoReportStatus { get; set; }
        [NotMapped]
        public List<SPFile> sqb { get; set; }
        [NotMapped]
        public List<SPFile> sjt { get; set; }
        [NotMapped]
        public List<SPFile> lxpfs { get; set; }

    }
}