using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class MPProduceStatistic
    {
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityID { get; set; }
        public string MPSizeType { get; set; }//门牌规格的类型 1 大门牌 2 小门牌，30*20CM；40*60CM为大门牌，21*15MM；18*14MM；15*10MM为小门牌
        public string MPType { get; set; }//门牌类型 1住宅门牌 2 道路门牌 3 农村门牌（住宅门牌不制作）
        public int RoadMPCount { get; set; }
        public int CountryMPCount { get; set; }
        public int BigMPCount { get; set; }
        public int SmallMPCount { get; set; }
        public int TotalMPCount { get; set; }
    }
}