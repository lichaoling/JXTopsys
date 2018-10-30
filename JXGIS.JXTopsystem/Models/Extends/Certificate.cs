using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [NotMapped]
    public class Certificate : MPOfCertificate
    {
        public string AddressCoding { get; set; }
        public string CountyID { get; set; }
        public string NeighborhoodsID { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string Applicant { get; set; }
        public string ApplicantPhone { get; set; }
        public string StandardAddress { get; set; }
    }
}