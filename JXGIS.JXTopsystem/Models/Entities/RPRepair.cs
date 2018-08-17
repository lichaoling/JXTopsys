using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("RP")]
    [Serializable]
    public class RPRepair
    {
        [Key]
        public string ID { get; set; }
        public string RPID { get; set; }
        public string RepairParts { get; set; }
        public string RepairFactory { get; set; }
        public string RepairContent { get; set; }
        public int RepairMode { get; set; }
        public DateTime? RepairTime { get; set; }
        public string RepairUser { get; set; }
        public DateTime? FinishRepaireTime { get; set; }
        public string FinishRepaireUser { get; set; }
    }
}