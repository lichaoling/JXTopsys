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
        public ContentResult GetProducedLXMP(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedLXMP(PageSize, PageNum);
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
        public ContentResult GetNotProducedLXMP(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetNotProducedLXMP(PageSize, PageNum);
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
        public JsonResult ProduceLXMP(List<NotProducedLXMPList> mpLists)
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
        public ContentResult GetProducedLXMPDetails(ProducedLXMPList producedLXMPList)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedLXMPDetails(producedLXMPList);
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
        public ContentResult GetProducedPLMP(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedPLMP(PageSize, PageNum);
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
        public ContentResult GetNotProducedPLMP(int PageSize, int PageNum)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetNotProducedPLMP(PageSize, PageNum);
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
        public JsonResult ProducePLMP(List<NotProducedPLMPList> mpLists)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.ProducePLMP(mpLists);
                rt = new RtObj(r);
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
        public JsonResult GetProducedPLMPDetails(ProducedPLMPList producedPLMPList)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedPLMPDetails(producedPLMPList);
                rt = new RtObj(r);
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