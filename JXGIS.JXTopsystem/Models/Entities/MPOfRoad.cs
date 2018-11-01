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
    /// 道路门牌
    /// </summary>
    [Table("MPOFROAD")]
    public class MPOfRoad
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public string CountyID { get; set; }
        [NotMapped]
        public string CountyCode { get; set; }
        public string NeighborhoodsID { get; set; }
        [NotMapped]
        public string NeighborhoodsCode { get; set; }
        public string CommunityName { get; set; }
        public string RoadID { get; set; }
        public string RoadName { get; set; }
        public string RoadStart { get; set; }
        public string RoadEnd { get; set; }
        public string BZRules { get; set; }
        public string ShopName { get; set; } //如果这个门牌号对应是商铺，就填写商铺名称
        public string ResidenceName { get; set; } //如果这个门牌号对应是小区，就填写小区名称
        public string MPNumberRange { get; set; }
        public string MPNumber { get; set; }
        public int? MPNumberType { get; set; } //门牌号码类型 1单 2 双
        public DbGeography MPPosition { get; set; }
        [NotMapped]
        public double? Lat { get; set; }
        [NotMapped]
        public double? Lng { get; set; }
        public string ReservedNumber { get; set; }
        public string OriginalMPAddress { get; set; }
        public string MPSize { get; set; }
        public string AddType { get; set; }//门牌新增方式
        public int? MPProduce { get; set; } //门牌制作 0不制作 1制作
        public string MPProduceUser { get; set; } //门牌制作完成情况
        public DateTime? MPProduceTime { get; set; }//门牌制作完成时间
        public string LXProduceID { get; set; } //零星制作批次号
        public string PLProduceID { get; set; } //批量制作批次号
        public string PLID { get; set; } //批量导入的一个批次GUID
        public int? MPMail { get; set; } //是否门牌邮寄 1 邮寄 0不邮寄
        public string MailAddress { get; set; }
        public string Postcode { get; set; }
        public string PropertyOwner { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string StandardAddress { get; set; }
        public string FCZAddress { get; set; }
        public string FCZNumber { get; set; }
        public string TDZAddress { get; set; }
        public string TDZNumber { get; set; }
        public string YYZZAddress { get; set; }
        public string YYZZNumber { get; set; }
        public string OtherAddress { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string SBDW { get; set; }
        public DateTime? BZTime { get; set; }
        public int? MPZPrintComplete { get; set; }//门牌证是否已打印
        public int? DZZMPrintComplete { get; set; }//地址证明是否已打印
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        public int State { get; set; } //使用状态 1 使用 2 注销 0 删除
        public DateTime? CancelTime { get; set; }
        public string CancelUser { get; set; }
        public DateTime? DelTime { get; set; }
        public string DelUser { get; set; }
        public string ArchiveFileStatus { get; set; }
    }
}