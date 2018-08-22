using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class SummarySheet
    {
        public string standardName { get; set; }
        public List<Record> LZMP { get; set; }
        public List<Record> DYMP { get; set; }
        public List<Record> HSMP { get; set; }
        public List<Record> RoadMP { get; set; }
        public List<Record> CountryMP { get; set; }
    }

    public class Record
    {
        public string numName { get; set; }
        public string size { get; set; }
        public int count { get; set; }
    }
}