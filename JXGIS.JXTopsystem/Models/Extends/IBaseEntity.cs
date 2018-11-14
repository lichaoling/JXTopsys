using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public interface IBaseEntityWithNeighborhoodsID
    {
        string NeighborhoodsID { get; set; }
    }

    public interface IBaseEntityWitDistrictID
    {
        string DistrictID { get; set; }
    }

}