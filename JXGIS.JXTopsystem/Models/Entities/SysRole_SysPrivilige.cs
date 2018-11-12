using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    public class SysRole_SysPrivilige
    {
        [Key]
        public int IndetityID { get; set; }
        public string RoleID { get; set; }
        public string PriviligeID { get; set; }
        public string PriviligeModule { get; set; }
        public string Describe { get; set; }
    }
}