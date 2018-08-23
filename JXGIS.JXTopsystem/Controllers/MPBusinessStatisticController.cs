using JXGIS.JXTopsystem.Business;
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
    public class MPBusinessStatisticController : Controller
    {
        // GET: MPBusinessStatistic
        public ContentResult GetMPBusinessDatas(int PageSize, int PageNum, string start, string end, string DistrictID, string Window, string CreateUser, int CertificateType = Enums.CertificateType.All)
        {
            RtObj rt = null;
            try
            {
                var r = StatisticUtils.GetMPBusinessDatas(PageSize, PageNum, start, end, DistrictID, Window, CreateUser, CertificateType);
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
        public ContentResult GetMPProduceStatistic(int PageSize, int PageNum, string Districts, string start, string end)
        {
            RtObj rt = null;
            try
            {
                var r = StatisticUtils.GetMPProduceStatistic(PageSize, PageNum, Districts, start, end);
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