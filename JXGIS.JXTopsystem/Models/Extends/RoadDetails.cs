﻿using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    //[NotMapped]
    //public class RoadDetails:Road
    //{
    //    public string CountyName { get; set; }
    //    public string NeighborhoodsName { get; set; }
    //    public string RoadGeomStr { get; set; }

    //    private static PropertyInfo[] props = typeof(RoadMPDetails).GetProperties();
    //    public object this[string key]
    //    {
    //        get
    //        {
    //            var prop = props.Where(p => p.Name == key).FirstOrDefault();
    //            return prop != null ? prop.GetValue(this) : null;
    //        }
    //    }
    //}
}