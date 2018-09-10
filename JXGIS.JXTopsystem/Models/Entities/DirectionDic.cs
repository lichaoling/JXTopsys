using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("DIRECTIONDIC")]
    public class DirectionDic
    {
        [Key]
        public string ID { get; set; }
        public string Diretion { get; set; }
    }
}