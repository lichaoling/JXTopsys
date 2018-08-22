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
        public int? MPProduce { get; set; } //门牌制作情况 1待制作 2不制作 3已制作
        public string MPProduceName { get; set; }
        public string PlaceName { get; set; }//道路名称和自然村名称
        public string MPNumber { get; set; }//门牌号码
        public string MPSize { get; set; }
        public string Postcode { get; set; }
        public DateTime? MPBZTime { get; set; }
    }
}