using JXGIS.JXTopsystem.App_Start;
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
        [LoggerFilter(Description = "修改路牌")]
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
        [LoggerFilter(Description = "注销一个路牌")]
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