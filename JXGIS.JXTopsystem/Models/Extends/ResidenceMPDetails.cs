﻿using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    [Serializable]
    public class ResidenceMPDetails : MPOfResidence
    {
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        //public string CommunityName { get; set; }
        public List<Pictures> FCZ { get; set; }
        public List<Pictures> TDZ { get; set; }
        public List<Pictures> BDCZ { get; set; }
        public List<Pictures> HJ { get; set; }
        public List<Pictures> SQB { get; set; }

        private static PropertyInfo[] props = typeof(ResidenceMPDetails).GetProperties();
        public object this[string key]
        {
            get
            {
                var prop = props.Where(p => p.Name == key).FirstOrDefault();
                return prop != null ? prop.GetValue(this) : null;
            }
        }

    }
    public class Pictures
    {
        public string FileID { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public string TRelativePath { get; set; }
    }
}