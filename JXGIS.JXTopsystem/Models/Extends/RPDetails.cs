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

        //加其中一个维修记录的维修内容、维修部位、报修时间
        public string RepairParts { get; set; }
        public string RepairContent { get; set; }
        public DateTime? RepairTime { get; set; }



        private static PropertyInfo[] props = typeof(RPDetails).GetProperties();
        public object this[string key]
        {
            get
            {
                var prop = props.Where(p => p.Name == key).FirstOrDefault();
                return prop != null ? prop.GetValue(this) : null;
            }
        }
    }
    [NotMapped]
    public class RPRepareInfos : RP
    {
        public string RPID { get; set; }
        public string RepairParts { get; set; }
        public string RepairFactory { get; set; }
        public string RepairContent { get; set; }
        public string RepairMode { get; set; }
        public DateTime? RepairTime { get; set; }
        public DateTime? FinishRepaireTime { get; set; }

    }
}