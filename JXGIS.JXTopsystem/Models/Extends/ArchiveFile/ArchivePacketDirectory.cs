using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.ArchiveFile
{
    public class ArchivePacketDirectory
    {
        public string systemcode { get; set; }
        public string batchname { get; set; }
        public string sendtime { get; set; }
        public string sendnumber { get; set; }
        List<Directories> directories { get; set; }
    }
    public class Directories
    {
        public string documentnumber { get; set; }
        public string projectname { get; set; }
        public string archivetime { get; set; }
        public string deptname { get; set; }
    }
}