using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public interface IUser
    {
        string UserID { get; }

        string UserName { get; }
        string Password { get; }
        List<string> DistrictIDList { get; }
        List<string> WindowList { get; }
    }
}