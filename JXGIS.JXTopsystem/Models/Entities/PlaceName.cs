using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("PlaceName")]
    public class PlaceName : IBaseEntityWithNeighborhoodsID
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityName { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 罗马字母拼写
        /// </summary>
        public string Pinyin { get; set; }
        /// <summary>
        /// 专名罗马字母拼写
        /// </summary>
        public string ZMPinyin { get; set; }
        /// <summary>
        /// 大类类别
        /// </summary>
        public string BigType { get; set; }
        /// <summary>
        /// 小类类别
        /// </summary>
        public string SmallType { get; set; }
        /// <summary>
        /// 地名类别
        /// </summary>
        public string DMType { get; set; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string XYDM { get; set; }
        /// <summary>
        /// 项目地址
        /// </summary>
        public string XMAddress { get; set; }
        /// <summary>
        /// 东至
        /// </summary>
        public string East { get; set; }
        /// <summary>
        /// 南至
        /// </summary>
        public string South { get; set; }
        /// <summary>
        /// 西至
        /// </summary>
        public string West { get; set; }
        /// <summary>
        /// 北至
        /// </summary>
        public string North { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public DbGeography Geom { get; set; }
        /// <summary>
        /// 地理实体概况
        /// </summary>
        public string DLST { get; set; }
        /// <summary>
        /// 地名含义
        /// </summary>
        public string DMHY { get; set; }
        /// <summary>
        /// 申办人
        /// </summary>
        public string Applicant { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string ContractAddress { get; set; }
        /// <summary>
        /// 申报单位
        /// </summary>
        public string SBDW { get; set; }
        /// <summary>
        /// 主管单位
        /// </summary>
        public string ZGDW { get; set; }
        public DateTime? ApplicantDate { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Postcode { get; set; }
        public int State { get; set; }
    }
}