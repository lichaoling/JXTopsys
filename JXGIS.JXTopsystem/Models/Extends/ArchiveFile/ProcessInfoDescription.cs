using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace JXGIS.JXTopsystem.Models.Extends.ArchiveFile
{
    [XmlRootAttribute("Opinions")]
    public class ProcessInfoDescription
    {
        public List<Opinions> Opinions { get; set; }
    }
    [XmlRootAttribute("Opinion")]
    public class Opinions
    {
        [XmlAttribute("nodename")]
        public string nodename { get; set; }
        public string author { get; set; }
        public string type { get; set; }
        public string body { get; set; }
        public string modified { get; set; }
    }
}