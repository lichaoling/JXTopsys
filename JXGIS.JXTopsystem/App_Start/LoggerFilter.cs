using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.App_Start
{
    public class LoggerFilter : FilterAttribute, IActionFilter
    {
        /// <summary>
        /// 日志描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Action执行后
        /// </summary>
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            ////获取 controllerName 名称
            var controllerName = filterContext.RouteData.Values["Controller"].ToString();
            ///获取你将要执行的Action的域名
            var actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["Action"].ToString();
            var cResult = filterContext.Result as ContentResult;
            RtObj rt = null;
            if (cResult != null)
            {
                var result = cResult.Content;
                rt = Newtonsoft.Json.JsonConvert.DeserializeObject<RtObj>(result);
            }
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.UserID = LoginUtils.CurrentUser.UserID;
                systemLog.UserName = LoginUtils.CurrentUser.UserName;
                systemLog.ActionName = controllerName + "/" + actionName;
                systemLog.Description = Description;
                if (rt != null)
                {
                    systemLog.OperateResult = rt.ErrorMessage == null ? 1 : 0;
                    systemLog.ErrorMessage = rt.ErrorMessage;
                }
                systemLog.OperateTime = DateTime.Now;
                dbContext.SystemLog.Add(systemLog);
                dbContext.SaveChanges();
            }

        }

        /// <summary>
        /// Action执行前
        /// </summary>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }
}