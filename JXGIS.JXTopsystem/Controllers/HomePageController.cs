using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business.Schedule;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class HomePageController : Controller
    {
        [LoggerFilter(Description = "获取首页统计数据")]
        public ActionResult GetHomePageData()
        {
            RtObj rt = null;
            try
            {
                HomePage.GetHomePageData();
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
    }
}