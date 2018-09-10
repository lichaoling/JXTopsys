using Aspose.Cells;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

namespace JXGIS.JXTopsystem.Business.MPModify
{

    public class MPModifyUtils
    {
        public static string GetGUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}