using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    /// <summary>
    /// 住宅门牌
    /// </summary>
    [Table("MPOFRESIDENCE")]
    [Serializable]
    public class MPOfResidence: IBaseEntityWithNeighborhoodsID
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public string AddressCoding2 { get; set; }
        public string CountyID { get; set; }  //*
        public string NeighborhoodsID { get; set; }  //*
        public string CommunityName { get; set; }  //*
        public string MPNumber { get; set; } //*
        public string ResidenceID { get; set; } //小区名
        public string ResidenceName { get; set; } //小区名
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
        public string AddType { get; set; }//门牌新增方式
        public int? MPProduce { get; set; } //门牌制作 0不制作 1制作
        public string MPProduceUser { get; set; } //门牌制作完成情况
        public DateTime? MPProduceTime { get; set; }//门牌制作完成时间
        public string LXProduceID { get; set; } //零星制作批次号
        public string PLProduceID { get; set; } //批量制作批次号
        public string PLID { get; set; } //批量导入的一个批次GUID
        public int? MPZPrintComplete { get; set; }//门牌证是否已打印
        public int? DZZMPrintComplete { get; set; }//地址证明是否已打印
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        public int State { get; set; }  //使用状态 1 使用 2 注销 0 删除
        public DateTime? CancelTime { get; set; }
        public string CancelUser { get; set; }
        public DateTime? DelTime { get; set; }
        public string DelUser { get; set; }
        /// <summary>
        /// 电子文件归档状态 0未归档  1已归档
        /// </summary>
        public string ArchiveFileStatus { get; set; }
        /// <summary>
        /// 办件回传状态 0未上报  1已上报
        /// </summary>
        public int InfoReportStatus { get; set; }
        /// <summary>
        /// 数据上报省系统 0未上报 1已上报
        /// </summary>
        public int DataPushStatus { get; set; }
        /// <summary>
        /// 申报来源：线上一窗受理平台收件  线下一窗受理平台申报   部门业务系统窗口收件
        /// </summary>
        public string SBLY { get; set; }
        /// <summary>
        /// projid
        /// </summary>
        public string ProjID { get; set; }

        private static PropertyInfo[] props = typeof(MPOfResidence).GetProperties();
        public object this[string key]
        {
            get
            {
                var prop = props.Where(p => p.Name == key).FirstOrDefault();
                return prop != null ? prop.GetValue(this) : null;
            }
        }
    }

}