using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    public class SysUserDetails : SysUser
    {
        public string Window { get; set; }
        public string DistrictID { get; set; }
        public string DistrictName { get; set; }
        public List<SysRole> Roles { get; set; }
    }
}