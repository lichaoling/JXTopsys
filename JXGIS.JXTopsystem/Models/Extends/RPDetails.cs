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
        public static System.Reflection.PropertyInfo[] ps = typeof(RP).GetProperties();
        public RPDetails(RP rp)
        {
            if (rp != null)
            {
                foreach (var pi in ps)
                {
                    pi.SetValue(this, pi.GetValue(rp));
                }
            }
        }
        public RPDetails() { }
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public Pictures CodeFile { get; set; }
        public List<Pictures> RPBZPhoto { get; set; }
        public List<RPRepair> RepairInfos { get; set; }

    }
    [NotMapped]
    public class RPRepareInfos : RP
    {
        public string RPID { get; set; }
        public string RepairParts { get; set; }
        public string RepairFactory { get; set; }
        public string RepairContent { get; set; }
        public int RepairMode { get; set; }
        public DateTime? RepairTime { get; set; }
        public DateTime? FinishRepaireTime { get; set; }

    }
}