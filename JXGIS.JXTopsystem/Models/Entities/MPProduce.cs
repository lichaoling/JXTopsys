using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("MPPRODUCE")]
    public class MPProduce
    {
        [Key]
        public string ID { get; set; }
        public int MPType { get; set; }//门牌类型 1住宅门牌 2 道路门牌 3 农村门牌（住宅门牌不制作）
        public string MPID { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
    }
}