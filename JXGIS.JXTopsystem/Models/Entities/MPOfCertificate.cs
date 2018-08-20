using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    /// <summary>
    /// 地名证明
    /// </summary>
    [Table("MPOFCERTIFICATE")]
    public class MPOfCertificate
    {
        [Key]
        public string ID { get; set; }
        public string MPID { get; set; }
        public int MPType { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        //public string Window { get; set; }
        public int CertificateType { get; set; }
    }
}