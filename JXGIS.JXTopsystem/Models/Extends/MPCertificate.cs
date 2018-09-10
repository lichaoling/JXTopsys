using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class MPCertificate
    {
        public string ID { get; set; }
        public string PropertyOwner { get; set; }
        public string StandardAddress { get; set; }
        public string FCZAddress { get; set; }
        public string TDZAddress { get; set; }
        public string BDCZAddress { get; set; }
        public string HJAddress { get; set; }
        public string YYZZAddress { get; set; }
        public string QQZAddress { get; set; }
        public string OtherAddress { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public string RoadName { get; set; }//道路
        public string ViligeName { get; set; }
        public string MPNumber { get; set; }
        public string ResidenceName { get; set; }
        public string Dormitory { get; set; }
        public string LZNumber { get; set; }
        public string DYNumber { get; set; }
        public string HSNumber { get; set; }
        public string OriginalMPAddress { get; set; }
        public string AddressCoding { get; set; }
    }
}