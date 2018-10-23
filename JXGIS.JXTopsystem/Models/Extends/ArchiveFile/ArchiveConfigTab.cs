using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.ArchiveFile
{
    public class ArchiveConfigTab
    {
        public string servicecode { get; set; }
        public string servicename { get; set; }
        public string deptname { get; set; }
        public string retentionperio { get; set; }
        public string version { get; set; }
        public List<Filingrange> filingranges { get; set; }
        public string deptid { get; set; }
        public string eventtype { get; set; }

    }
    public class Filingrange
    {
        public List<file> file { get; set; }
        public string filename { get; set; }
        public string auxiliaryone { get; set; }
        public string auxiliarytwo { get; set; }
        public string auxiliarythree { get; set; }

    }
    public class file
    {
        public string filename { get; set; }
        public string auxiliaryone { get; set; }
        public string auxiliarytwo { get; set; }
        public string auxiliarythree { get; set; }
    }
}