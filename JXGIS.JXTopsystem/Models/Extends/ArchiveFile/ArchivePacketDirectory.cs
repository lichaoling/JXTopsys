using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace JXGIS.JXTopsystem.Models.Extends.ArchiveFile
{
    [XmlRootAttribute("description")]
    public class ArchivePacketDirectory
    {
        [XmlAttribute("title")]
        public string title
        {
            get;
            set;
        }
        [XmlElementAttribute("systemcode")]
        public string systemcode { get; set; }
        public string batchname { get; set; }
        public string sendtime { get; set; }
        public string sendnumber { get; set; }
        List<Directories> directories { get; set; }
    }
    [XmlRootAttribute("directories")]
    public class Directories
    {
        [XmlAttribute("title")]
        public string title
        {
            get;
            set;
        } = "存档信息包明细";
        public string documentnumber { get; set; }
        public string projectname { get; set; }
        public string archivetime { get; set; }
        public string deptname { get; set; }
    }
}