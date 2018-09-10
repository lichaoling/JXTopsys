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
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string Window { get; set; }
        public string DistrictID { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        public int State { get; set; }
        public DateTime? CancelTime { get; set; }
        public string CancelUser { get; set; }
    }
}