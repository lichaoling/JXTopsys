using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    public class RPDetails : RP
    {
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public Pictures CodeFile { get; set; }
        public List<Pictures> RPBZPhoto { get; set; }
        public List<RPRepair> RepairInfos { get; set; }
    }
}