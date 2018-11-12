using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("DISTRICTNew")]
    public class District
    {
        [Key]
        public int IndetityID { get; set; }
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? State { get; set; }
    }
}