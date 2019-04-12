using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("MPOFUPLOADFILES")]
    public class MPOfUploadFiles
    {
        [Key]
        public string ID { get; set; }
        public string MPID { get; set; }

        public string Name { get; set; }
        public string FileEx { get; set; }
        public string DocType { get; set; }
        public int State { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}