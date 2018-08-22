using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    public class MPBusiness : MPOfCertificate
    {
        public string CountyID { get; set; }
        public string CountyName { get; set; }
        public string NeighborhoodsID { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityID { get; set; }
        public string CommunityName { get; set; }
        public string StandardAddress { get; set; }
        //public string CreateUserName { get; set; }
        public string MPTypeName { get; set; }
        public string CertificateTypeName { get; set; }
        public DateTime? MPBZTime { get; set; }
        //public List<string> Window { get; set; }
    }
}