using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    [Table("ROADDic")]
    public class RoadDic
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityID { get; set; }
        public string RoadName { get; set; }
        public string RoadStart { get; set; }
        public string RoadEnd { get; set; }
        public string MPRules { get; set; }
        public int State { get; set; }
    }
}