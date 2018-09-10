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
        public ContentResult GetProduceMP(int PageSize, int PageNum, string DistrictID, int MPProduce, int MPType, string Name)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProduceMP(PageSize, PageNum, DistrictID, MPProduce, MPType, Name);
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

        public ContentResult SaveAndGetProduceMPList(int PageSize, int PageNum, List<LXMPProduceList> mps)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.SaveAndGetProduceMPList(PageSize, PageNum, mps);
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
    }
}