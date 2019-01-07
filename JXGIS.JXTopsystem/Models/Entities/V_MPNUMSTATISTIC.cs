using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("V_MPNUMSTATISTIC")]
    public class V_MPNUMSTATISTIC
    {
        [Key]
        public string KeyID { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityName { get; set; }
        public DateTime? BZTime { get; set; }
        public int count { get; set; }
        public string type { get; set; }
    }
}