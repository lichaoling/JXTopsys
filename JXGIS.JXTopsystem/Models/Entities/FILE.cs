using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("FILE")]
    public class FILE
    {
        [Key]
        public string ID { get; set; }

        public string FormID { get; set; }

        public string BusinessType { get; set; }

        public string CertificateType { get; set; }

        public string FileName { get; set; }

        public string FileOrginalName { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? IsValid { get; set; }
    }
}