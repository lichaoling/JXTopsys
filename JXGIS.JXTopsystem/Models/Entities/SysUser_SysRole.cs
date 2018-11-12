using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SYSUSER_SYSROLE")]
    public class SysUser_SysRole
    {
        [Key]
        public int IndetityID { get; set; }
        public string UserID { get; set; }
        public string RoleID { get; set; }
    }
}