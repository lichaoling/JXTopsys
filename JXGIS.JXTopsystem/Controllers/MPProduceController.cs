using JXGIS.JXTopsystem.Business.MPProduce;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class MPProduceController : Controller
    {
        public ContentResult GetLXMPProduce(int PageSize, int PageNum, int LXMPProduceComplete)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetLXMPProduce(PageSize, PageNum, LXMPProduceComplete);
                rt = new RtObj(r);
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
        public JsonResult ProduceLXMP(List<LXMPProduceList> mpLists)
        {
            RtObj rt = null;
            try
            {
                MPProduceUtils.ProduceLXMP(mpLists);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Json(s);
        }

        public ContentResult GetPLMPProduce(int PageSize, int PageNum, int PLMPProduceComplete)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetPLMPProduce(PageSize, PageNum, PLMPProduceComplete);
                rt = new RtObj(r);
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
        public JsonResult ProducePLMP(List<PLMPProduceList> mpLists)
        {
            RtObj rt = null;
            try
            {
                MPProduceUtils.ProducePLMP(mpLists);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Json(s);
        }
    }
}