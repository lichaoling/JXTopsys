using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SYSUSER_DISTRICT")]
    public class SysUser_District
    {
        [Key]
        public int IndetityID { get; set; }
        public string UserID { get; set; }
        public string DistrictID { get; set; }
    }
}