using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    [Serializable]
    public class ResidenceMPErrors : MPErrors
    {
        public ResidenceMPDetails mp { get; set; }
    }
}