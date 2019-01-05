using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    [Table("CROLE")]
    public class CRole
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Describe { get; set; }

        [NotMapped]
        public List<CPrivilege> Privileges { get; set; }

        [NotMapped]
        public List<TPrivilege> TPrivileges { get; set; }

        [NotMapped]
        public TPrivilege TPrivilege { get; set; }
    }
}