using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Login(string userName, string password, string securityCode)
        {
            RtObj rt = null;
            try
            {
                var msg = string.Empty;
                if (string.IsNullOrWhiteSpace(userName))
                {
                    msg = "请输入用户名";
                }
                else if (string.IsNullOrWhiteSpace(password))
                {
                    msg = "请输入密码";
                }
                else if (string.IsNullOrWhiteSpace(securityCode))
                {
                    msg = "请输入验证码";
                }
                else if (!LoginUtils.CurrentValidateGraphicCode.Validate(securityCode))
                {
                    msg = "验证码输入有误";
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    throw new Error(msg);
                }

                password = SecurityUtils.RSADecrypt(password);
                password = SecurityUtils.MD5Encrypt(password);
                var user = LoginUtils.Login(userName, password);
                if (user == null)
                {
                    throw new Error("输入的用户名或密码有误");
                }
                // 成功之后刷新验证码
                LoginUtils.CurrentValidateGraphicCode.Refresh();
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt);
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ContentResult Logout()
        {
            RtObj rt = null;
            try
            {
                LoginUtils.Logout();
                rt = new RtObj(new object() { });
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult GetCurrentUser()
        {
            RtObj rt = null;
            try
            {
                var user = LoginUtils.CurrentUser as SysUser;
                rt = new RtObj();
                if (user != null)
                {
                    rt.Data = new
                    {
                        Name = user.Name,
                        UserName = user.UserName,
                        DistrictID = user.DistrictID
                    };
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