using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    [Serializable]
    public class GroupX
    {
        public string P_ID { get; set; }

        public List<TPrivilege> Privileges { get; set; }
    }
}