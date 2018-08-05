using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    /// <summary>
    /// 农村门牌
    /// </summary>
    [Table("MPOFCOUNTRY")]
    public class MPOfCountry
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityID { get; set; }
        public string ViligeName { get; set; }
        public string MPNumber { get; set; }
        public DbGeography MPPosition { get; set; }
        [NotMapped]
        public double? Lat { get; set; }
        [NotMapped]
        public double? Lng { get; set; }
        public string OriginalNumber { get; set; }
        public string MPSize { get; set; }
        public string HSNumber { get; set; }
        public int? MPProduce { get; set; } //是否门牌制作 1 待制作 2不制作 3已制作
        public int? MPMail { get; set; } //是否门牌邮寄 1 邮寄 2 不邮寄
        public string MailAddress { get; set; }
        public string Postcode { get; set; }
        public string PropertyOwner { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string StandardAddress { get; set; }
        public string TDZAddress { get; set; }
        public string TDZNumber { get; set; }
        [NotMapped]
        public string TDZFile { get; set; } //
        public string QQZAddress { get; set; }
        public string QQZNumber { get; set; }
        [NotMapped]
        public string QQZFile { get; set; }//
        public string OtherAddress { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        public int State { get; set; }//使用状态 1 使用 2 注销 0删除
        public DateTime? CancelTime { get; set; }
        public string CancelUser { get; set; }
    }
}