using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("POSTCODEDIC")]
    public class PostcodeDic
    {
        [Key]
        public string ID { get; set; }
        public string Postcode { get; set; }
        public string PCode { get; set; }
    }
}