using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    [Table("SYSUSER")]
    [Serializable]
    public partial class SysUser : IUser
    {
        [Key]
        public string UserID { get; set; }
        public string CountyID { get; set; }  //*
        public string NeighborhoodsID { get; set; }  //*
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Window { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public string Telephone { get; set; }
        public string BGDZ { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUser { get; set; }
        [NotMapped]
        public virtual List<string> DistrictIDList { get; set; }
        [NotMapped]
        public virtual string DistrictName { get; set; }
        [NotMapped]
        public virtual List<SysRole> RoleList { get; set; }
        [NotMapped]
        public virtual string RoleName { get; set; }
        [NotMapped]
        public virtual List<SysRole_SysPrivilige> PriviligeList { get; set; }


    }
}