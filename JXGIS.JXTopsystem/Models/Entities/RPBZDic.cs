using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("RPBZDIC")]
    public class RPBZDic
    {
        [Key]
        public int IndetityID { get; set; }
        public string Category { get; set; }
        public string Data { get; set; }
        public int State { get; set; }  //使用状态 1 使用 0 删除
    }
}