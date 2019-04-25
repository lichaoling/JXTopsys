using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business.Schedule;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
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
        public ActionResult GetHomePageDetailData(DateTime start, DateTime end)
        {
            RtObj rt = null;
            try
            {
                var d = HomePage.GetHomePageDetailData(start, end);
                rt = new RtObj(d);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
        [LoggerFilter(Description = "获取首页统计数据")]
        public ActionResult GetHomePageTotalData()
        {
            RtObj rt = null;
            try
            {
                var d = HomePage.GetHomePageTotalData();
                rt = new RtObj(d);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        [LoggerFilter(Description = "查询申报的已办或代办事项")]
        public ActionResult GetTodoItems(int pageNum, int pageSize, string sbly, string lx)
        {
            RtObj rt = null;
            try
            {
                var d = HomePage.GetTodoItems(pageNum, pageSize, sbly, lx);
                rt = new RtObj(d);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Content(s);
        }
        [LoggerFilter(Description = "查询申报的已办或代办事项")]
        public ActionResult GetDoneItems(int pageNum, int pageSize, string sbly, string lx, DateTime start, DateTime end)
        {
            RtObj rt = null;
            try
            {
                var d = HomePage.GetDoneItems(pageNum, pageSize, sbly, lx, start, end);
                rt = new RtObj(d);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Content(s);
        }
    }
}