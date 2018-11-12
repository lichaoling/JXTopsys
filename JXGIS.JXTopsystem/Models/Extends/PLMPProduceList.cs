using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class NotProducedPLMPList
    {
        public string PLID { get; set; }
        public string MPType { get; set; }
        public string ResidenceName { get; set; }
        public string RoadName { get; set; }
        public string ViligeName { get; set; }
        public int MPCount { get; set; }
        public string SBDW { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public DateTime? CreateTime { get; set; }
    }
    public class ProducedPLMPList
    {
        //public string PLProduceID { get; set; }
        //public string MPType { get; set; }
        //public string ResidenceName { get; set; }
        //public string RoadName { get; set; }
        //public string ViligeName { get; set; }
        //public int MPCount { get; set; }
        //public string SBDW { get; set; }
        //public string Postcode { get; set; }
        //public string Applicant { get; set; }
        //public string ApplicantPhone { get; set; }
        //public DateTime? MPBZTime { get; set; }
        //public DateTime? MPProduceTime { get; set; }


        public string MPType { get; set; }
        public string PLProduceID { get; set; }
        public string MPProduceUser { get; set; } //门牌制作完成情况
        public DateTime? MPProduceTime { get; set; }//门牌制作完成时间
    }
}