using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    [Table("ROADDIC")]
    public class RoadDic
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityName { get; set; }
        public string RoadName { get; set; }
        public string RoadStart { get; set; }
        public string RoadEnd { get; set; }
        public string BZRules { get; set; }
        public string StartEndNum { get; set; }
        public string Intersection { get; set; }
    }
}