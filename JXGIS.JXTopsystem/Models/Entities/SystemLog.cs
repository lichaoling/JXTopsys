using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("SYSTEMLOG")]
    [Serializable]
    public class SystemLog
    {
        [Key]
        public int IndetityID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public int OperateResult { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? OperateTime { get; set; }
    }
}