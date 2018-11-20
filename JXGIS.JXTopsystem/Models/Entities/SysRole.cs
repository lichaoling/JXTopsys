using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SYSROLE")]
    public class SysRole
    {
        [Key]
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }

        [NotMapped]
        public virtual List<SysRole_SysPrivilige> PriviligeList { get; set; }
        [NotMapped]
        public virtual string PriviligeNames { get; set; }

    }
}