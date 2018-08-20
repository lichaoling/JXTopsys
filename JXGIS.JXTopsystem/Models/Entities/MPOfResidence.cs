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
    /// 住宅门牌
    /// </summary>
    [Table("MPOFRESIDENCE")]
    [Serializable]
    public class MPOfResidence
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public string CountyID { get; set; }  //*
        public string NeighborhoodsID { get; set; }  //*
        public string CommunityID { get; set; }  //*
        //public string RoadID { get; set; } //*
        public string MPNumber { get; set; } //*
        public string ResidenceName { get; set; } //小区名 可为空
        public string Dormitory { get; set; }
        public string LZNumber { get; set; } //没有楼幢时可为空
        public string DYNumber { get; set; } //没有单元时可为空
        public DbGeography DYPosition { get; set; }
        [NotMapped]
        public double? Lat { get; set; }
        [NotMapped]
        public double? Lng { get; set; }
        public string HSNumber { get; set; }
        public string MPSize { get; set; }
        public string Postcode { get; set; }  //*
        public string PropertyOwner { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string StandardAddress { get; set; }
        public string FCZAddress { get; set; }
        public string FCZNumber { get; set; }
        [NotMapped]
        public string FCZFile { get; set; }
        public string TDZAddress { get; set; }
        public string TDZNumber { get; set; }
        [NotMapped]
        public string TDZFile { get; set; }
        public string BDCZAddress { get; set; }
        public string BDCZNumber { get; set; }
        [NotMapped]
        public string BDCZFile { get; set; }
        public string HJAddress { get; set; }
        public string HJNumber { get; set; }
        [NotMapped]
        public string HJFile { get; set; }
        public string OtherAddress { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string SBDW { get; set; }
        public DateTime? BZTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        public int State { get; set; }  //使用状态 1 使用 2 注销 0 删除
        public DateTime? CancelTime { get; set; }
        public string CancelUser { get; set; }
        public DateTime? DelTime { get; set; }
        public string DelUser { get; set; }
    }
}