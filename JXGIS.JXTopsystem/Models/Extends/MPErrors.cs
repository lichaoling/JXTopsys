using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class MPErrors
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}