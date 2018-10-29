using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace JXGIS.JXTopsystem.Models.Extends.ArchiveFile
{
    [XmlRootAttribute("description")]
    public class ArchiveConfigTab
    {
        [XmlAttribute("title")]
        public string title
        {
            get;
            set;
        } = "归档配置表";
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
        [XmlAttribute("title")]
        public string title
        {
            get;
            set;
        } = "申请材料";
        public List<file> files { get; set; }
    }
   
    public class file
    {
        public string filename { get; set; }
        public string auxiliaryone { get; set; }
        public string auxiliarytwo { get; set; }
        public string auxiliarythree { get; set; }
    }
}