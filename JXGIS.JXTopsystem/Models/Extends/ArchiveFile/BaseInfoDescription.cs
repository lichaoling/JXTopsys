using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace JXGIS.JXTopsystem.Models.Extends.ArchiveFile
{
    [XmlRootAttribute("description")]
    public class BaseInfoDescription
    {
        [XmlAttribute("title")]
        public string title
        {
            get;
            set;
        } = "基本信息描述";

        public string deptname { get; set; }
        public string documentnumber { get; set; }
        public string retentionperiod { get; set; }
        public string archivetime { get; set; }
        public string eventtype { get; set; }
        public string leaddepartment { get; set; }
        public string applyname { get; set; }
        public string mobile { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string contactidcard { get; set; }
        public string legalman { get; set; }
        public string projectname { get; set; }
        public string receivedepartmentname { get; set; }
        public string receivetime { get; set; }
        public string transacttime { get; set; }
        public string projid { get; set; }
        public string projectpassword { get; set; }
        public string servicetype { get; set; }
        public string infotype { get; set; }
        public string result { get; set; }
        public string resultcode { get; set; }
        public string eventcode { get; set; }
        public string version { get; set; }
    }
}