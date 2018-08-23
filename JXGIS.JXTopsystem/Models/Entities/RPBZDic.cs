using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("RPBZDIC")]
    public class RPBZDic
    {
        [Key]
        public string ID { get; set; }
        public string Category { get; set; }
        public string Data { get; set; }
    }
}