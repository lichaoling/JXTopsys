using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("RPPEPAIRUPLOADFILES")]
    public class RPPepairUploadFiles
    {
        [Key]
        public string ID { get; set; }
        public string RPRepairID { get; set; }

        public string Name { get; set; }
        public string FileEx { get; set; }
        public int? RepairType { get; set; } //0 维修前 1维修后
        public int State { get; set; }
    }
}