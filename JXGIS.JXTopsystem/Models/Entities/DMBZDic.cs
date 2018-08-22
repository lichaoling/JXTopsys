using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("DMBZDic")]
    public class DMBZDic
    {
        [Key]
        public string ID { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Material { get; set; }
    }
}