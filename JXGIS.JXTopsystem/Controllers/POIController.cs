using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class POIController : Controller
    {
        // GET: POI
        public ActionResult GetPOIPosition(int PageSize, int PageNum, string name)
        {
            RtObj rt = null;
            try
            {
                using (var dbContext = SystemUtils.NewOrclEFDbContext)
                {
                    var query = dbContext.POI.Where(t => t.PAC == 330402 || t.PAC == 330411);
                    if (!string.IsNullOrEmpty(name))
                        query = query.Where(t => t.NAME.Contains(name) || t.SHORTNAME.Contains(name));
                    var count = query.Count();
                    var data = query.OrderBy(t => t.NAME).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                    Dictionary<string, object> re = new Dictionary<string, object>
                    {
                       { "Data",data},
                       { "Count",count}
                    };
                    rt = new RtObj(re);
                }
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