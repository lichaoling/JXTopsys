using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class LXMPProduceList
    {
        public string MPID { get; set; }
        public string CountyID { get; set; }
        public string CountyName { get; set; }
        public string NeighborhoodsID { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public int MPType { get; set; }
        public string MPTypeName { get; set; }
        public int? LXMPProduceComplete { get; set; } //门牌制作完成情况 0未完成 1已完成
        public string LXMPProduceCompleteName { get; set; } //门牌制作完成情况 0未完成 1已完成
        public string PlaceName { get; set; }//道路名称和自然村名称
        public string MPNumber { get; set; }//门牌号码
        public string MPSize { get; set; }
        public string Postcode { get; set; }
        public DateTime? MPBZTime { get; set; }
    }
}