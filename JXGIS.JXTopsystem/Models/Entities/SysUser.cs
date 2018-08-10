using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SYSUSER")]
    [Serializable]
    public class SysUser : IUser
    {
        [Key]
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public string Telephone { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        public int? State { get; set; }
        public DateTime? CancelTime { get; set; }
        public string CancelUser { get; set; }
        public DateTime? DelTime { get; set; }
        public string DelUser { get; set; }
        [NotMapped]
        public virtual List<string> DistrictID { get; set; }
    }
}