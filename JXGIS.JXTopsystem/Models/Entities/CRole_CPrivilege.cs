using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("CROLE_CPRIVILEGE")]
    public class CRole_CPrivilege
    {
        [Key]
        public string CRoleId { get; set; }

        public string CPrivilegeId { get; set; }

        public string Privilege { get; set; }

    }
}