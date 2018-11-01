﻿using JXGIS.JXTopsystem.Business;
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
        public ContentResult GetMPBusinessUserTJ(int PageSize, int PageNum, DateTime? start, DateTime? end, string Window, string CreateUser, string CertificateType)
        {
            RtObj rt = null;
            try
            {
                var r = MPStatisticUtils.GetMPBusinessUserTJ(PageSize, PageNum, start, end, Window, CreateUser, CertificateType);
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

        public ContentResult GetMPBusinessNumTJ(/*int PageSize, int PageNum,*/ DateTime? start, DateTime? end, string[] DistrictID, string CertificateType)
        {
            RtObj rt = null;
            try
            {
                var r = MPStatisticUtils.GetMPBusinessNumTJ(100, 1, start, end, DistrictID == null ? null : DistrictID.Last(), CertificateType);
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

        public ContentResult GetMPProduceTJ(/*int PageSize, int PageNum, */string DistrictID, string CommunityName, DateTime? start, DateTime? end)
        {
            RtObj rt = null;
            try
            {
                var r = MPStatisticUtils.GetMPProduceTJ(100, 1, DistrictID, CommunityName, start, end);
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