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
    [Table("RP")]
    [Serializable]
    public class RP : IBaseEntityWithNeighborhoodsID
    {
        [Key]
        public string ID { get; set; }
        public string AddressCoding { get; set; }
        public int? Code { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityName { get; set; }
        public string RoadID { get; set; }
        public string RoadName { get; set; }
        public string Intersection { get; set; }
        public string Direction { get; set; }
        public string RoadStart { get; set; }
        public string RoadEnd { get; set; }
        public string BZRules { get; set; }
        public string StartEndNum { get; set; }
        public double? PositionX { get; set; }
        public double? PositionY { get; set; }
        public string Model { get; set; }
        public string Size { get; set; }
        public string Material { get; set; }
        public string Manufacturers { get; set; }
        public string FrontTagline { get; set; }
        public string BackTagline { get; set; }
        public string Management { get; set; }
        public int RepairedCount { get; set; }
        public int FinishRepaire { get; set; }
        public DateTime? FinishRepaireTime { get; set; }
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
        public string BXFS { get; set; }
    }
}