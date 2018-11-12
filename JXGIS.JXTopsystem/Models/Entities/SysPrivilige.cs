using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SYSPRIVILIGE")]
    public class SysPrivilige
    {
        [Key]
        public string PriviligeID { get; set; }
        public string PriviligeName { get; set; }
        public string PriviligeModule { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
    }
}