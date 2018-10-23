using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.ArchiveFile
{
    public class ProcessInfoDescription
    {
        List<Opinions> Opinions { get; set; }
    }
    public class Opinions
    {
        public string nodename { get; set; }
        public string author { get; set; }
        public string type { get; set; }
        public string body { get; set; }
        public string modified { get; set; }
    }
}