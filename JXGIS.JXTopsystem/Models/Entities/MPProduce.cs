using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("MPPRODUCE")]
    public class MPProduce
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityID { get; set; }
        public int MPType { get; set; }//门牌类型 1住宅门牌 2 道路门牌 3 农村门牌
        public int BigMPCount { get; set; }
        public int SmallMPCount { get; set; }
        public int LZMPCount { get; set; }
        public int DYMPCount { get; set; }
        public int HSMPCount { get; set; }
        public int CountryMPCount { get; set; }
        public int TotalCount { get; set; }
        public string MPID { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
    }
}