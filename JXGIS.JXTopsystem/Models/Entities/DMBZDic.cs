using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("DMBZDIC")]
    public class DMBZDic
    {
        [Key]
        public int IndetityID { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Material { get; set; }
        public int State { get; set; }
    }
}