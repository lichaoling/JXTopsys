using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("DMOFUPLOADFILES")]
    public class DMOFUPLOADFILES
    {
        [Key]
        public string ID { get; set; }
        public string DMID { get; set; }

        public string Name { get; set; }
        public string FileEx { get; set; }
        public string DocType { get; set; }
        public int State { get; set; }
    }
}