using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class SummarySheet
    {
        public List<Record> StandardName { get; set; }
        public List<Record> LZMP { get; set; }
        public List<Record> DYMP { get; set; }
        public List<Record> HSMP { get; set; }
        public List<Record> RoadMP { get; set; }
        public List<Record> CountryMP { get; set; }
    }

    public class Record
    {
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string CommunityID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public int Count { get; set; }
    }
}