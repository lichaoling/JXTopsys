using JXGIS.JXTopsystem.Business.RPModify;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class RPModifyController : Controller
    {
        public JsonResult ModifyRP(RP newData, string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RPModifyUtils.ModifyRP(newData, oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
    }
}