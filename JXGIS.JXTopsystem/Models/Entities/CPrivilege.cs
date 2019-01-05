using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    [Table("CPRIVILEGE")]
    public partial class CPrivilege
    {
        [Key]
        public string Id { get; set; }

        public string PId { get; set; }

        public string Name { get; set; }

        public string PassPrivilege { get; set; }

    }
}