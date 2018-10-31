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
        public JsonResult ModifyRP(string oldDataJson)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RPModifyUtils.ModifyRP(oldDataJson);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

        public JsonResult CancelRP(List<string> IDs)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                RPModifyUtils.CancelRP(IDs);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }
    }
}