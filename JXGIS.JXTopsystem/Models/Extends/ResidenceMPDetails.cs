using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    public class ResidenceMPDetails : MPOfResidence
    {

        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public string RoadName { get; set; }
        public string PlaceName { get; set; }
        public List<string> FCZName { get; set; }
        public List<string> FCZURL { get; set; }
        public List<string> TDZName { get; set; }
        public List<string> TDZURL { get; set; }
        public List<string> BDCZName { get; set; }
        public List<string> BDCZURL { get; set; }
        public List<string> HJName { get; set; }
        public List<string> HJURL { get; set; }

    }
}