using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("RPREPAIR")]
    [Serializable]
    public class RPRepair
    {
        [Key]
        public string ID { get; set; }
        public string RPID { get; set; }
        public string RepairParts { get; set; }
        public string RepairFactory { get; set; }
        public string RepairContent { get; set; }
        public string RepairMode { get; set; }
        public string Model { get; set; }
        public string Size { get; set; }
        public string Material { get; set; }
        public string Manufacturers { get; set; }
        public DateTime? RepairTime { get; set; }
        public string RepairUser { get; set; }
        //public int IsFinish { get; set; }
        public DateTime? FinishRepaireTime { get; set; }
        public string FinishRepaireUser { get; set; }
    }
}