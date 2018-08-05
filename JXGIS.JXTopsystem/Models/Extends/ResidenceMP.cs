using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class ResidenceMP
    {
        public string ID { get; set; }
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public string PlaceName { get; set; }
        public string StandardAddress { get; set; }
        public string PropertyOwner { get; set; }
        public DateTime CreateTime { get; set; }


        private static PropertyInfo[] props = typeof(ResidenceMP).GetProperties();
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