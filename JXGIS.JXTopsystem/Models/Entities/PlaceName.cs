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
    [Table("PLACENAME")]
    public class PlaceName : IBaseEntityWithNeighborhoodsID
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }
        [NotMapped]
        public string CountyName
        {
            get
            {
                var data = string.Empty;
                if (!string.IsNullOrEmpty(this.CountyID))
                {
                    data = this.CountyID.Split('.').Last();
                }
                return data;
            }
            set
            {
                //this.CountyName = value;
            }
        }
        public string NeighborhoodsID { get; set; }
        [NotMapped]
        public string NeighborhoodsName
        {
            get
            {
                var data = string.Empty;
                if (!string.IsNullOrEmpty(this.NeighborhoodsID))
                {
                    data = this.NeighborhoodsID.Split('.').Last();
                }
                return data;
            }
            set
            {
                //this.NeighborhoodsName = value;
            }
        }
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
        public string ZYSSType { get; set; }
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
        public decimal? Lng { get; set; }
        public decimal? Lat { get; set; }
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
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        public DateTime? CancelTime { get; set; }
        public string CancelUser { get; set; }

        public string Postcode { get; set; }
        public int State { get; set; }
        [NotMapped]
        public List<Pictures> SBBG { get; set; }
        [NotMapped]
        public List<Pictures> LXPFWJ { get; set; }
        [NotMapped]
        public List<Pictures> SJT { get; set; }

        private static PropertyInfo[] props = typeof(PlaceName).GetProperties();
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