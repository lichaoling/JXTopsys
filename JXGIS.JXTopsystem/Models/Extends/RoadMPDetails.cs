using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    public class RoadMPDetails : MPOfRoad
    {
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public string RoadName { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public List<string> FCZName { get; set; }
        public List<string> FCZURL { get; set; }
        public List<string> TDZName { get; set; }
        public List<string> TDZURL { get; set; }
        public List<string> YYZZName { get; set; }
        public List<string> YYZZURL { get; set; }


        private static PropertyInfo[] props = typeof(RoadMPDetails).GetProperties();
        public object this[string key]
        {
            get
            {
                var prop = props.Where(p => p.Name == key).FirstOrDefault();
                return prop != null ? prop.GetValue(this) : null;
            }
        }
    }
}