using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("POSTCODEDIC")]
    public class PostcodeDic
    {
        [Key]
        public string ID { get; set; }
        public string Postcode { get; set; }
        public string CountyID { get; set; }  //*
        public string NeighborhoodsID { get; set; }  //*
        public string CommunityID { get; set; }  //*
        public int State { get; set; }  //使用状态 1 使用 0 删除
    }
}