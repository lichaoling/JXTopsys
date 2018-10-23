using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("ARCHIVEFILE")]
    public class ARCHIVEFILE
    {
        [Key]
        public string ID { get; set; }
        public string DOCUMENTNUMBER { get; set; }
        public string EVENTID { get; set; }
    }
}