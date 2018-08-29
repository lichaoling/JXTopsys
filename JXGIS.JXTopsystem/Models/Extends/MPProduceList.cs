using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    public class MPProduceList : MPProduce
    {
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public string MPTypeName { get; set; }
        public int? MPProduce { get; set; } //是否门牌制作 0不制作 1制作
        public int? MPProduceComplete { get; set; } //门牌制作完成情况 0未完成 1已完成
        public string MPProduceName { get; set; }//是否门牌制作 0不制作 1制作
        public string MPProduceCompleteName { get; set; } //门牌制作完成情况 0未完成 1已完成
        public string PlaceName { get; set; }//道路名称和自然村名称
        public string MPNumber { get; set; }//门牌号码
        public string MPSize { get; set; }
        public string Postcode { get; set; }
        public DateTime? MPBZTime { get; set; }
    }
}