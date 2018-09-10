using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class PLMPProduceList
    {
        public string PLID { get; set; }
        public string CountyID { get; set; }
        public string CountyName { get; set; }
        public string NeighborhoodsID { get; set; }
        public string NeighborhoodsName { get; set; }
        public int MPType { get; set; }
        public string MPTypeName { get; set; }
        public int? PLMPProduceComplete { get; set; } //门牌制作完成情况 0未完成 1已完成
        public string PLMPProduceCompleteName { get; set; } //门牌制作完成情况 0未完成 1已完成
        public string ResidenceName { get; set; }
        public string RoadName { get; set; }
        public string ViligeName { get; set; }
        public int MPCount { get; set; }
        public string SBDW { get; set; }
        public string Postcode { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string MPBZTime { get; set; }
    }
}