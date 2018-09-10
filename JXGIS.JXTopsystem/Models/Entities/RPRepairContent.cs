using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("RPREPAIRCONTENT")]
    public class RPRepairContent
    {
        [Key]
        public string ID { get; set; }
        public string RepairContent { get; set; }
    }
}