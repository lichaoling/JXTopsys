using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("VILIGEDIC")]
    public class ViligeDic
    {
        [Key]
        public string ID { get; set; }
        public string CountyID { get; set; }  //*
        public string NeighborhoodsID { get; set; }  //*
        public string CommunityName { get; set; }  //*
        public string ViligeName { get; set; } //* 
    }
}