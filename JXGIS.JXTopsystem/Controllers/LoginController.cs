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

        public static string userName = System.Configuration.ConfigurationManager.AppSettings["username"];
        public static string password = System.Configuration.ConfigurationManager.AppSettings["password"];

        public ActionResult Test()
        {
            var s = SecurityUtils.MD5Encrypt("@123456");
            var key = "12abcdef";
            var x = SecurityUtils.DESEncryt("哈哈哈哈", key);
            x = SecurityUtils.DESDecrypt(x, key);

            x = SecurityUtils.RSAEncrypt(x);
            x = SecurityUtils.RSADecrypt(x);

            return null;
        }

        public ContentResult LoginTmp(string userName, string password)
        {
            Dictionary<string, object> rt = new Dictionary<string, object>();
            try
            {
                if (!(userName == LoginController.userName && password == LoginController.password))
                {
                    throw new Error("输入的用户名或密码有误");
                }
                SysUser user = new SysUser()
                {
                    UserName = "测试用户",
                    UserID = userName,
                    DistrictIDList = new List<string>() { "嘉兴市" },
                    Window = "地名办公室"
                };
                LoginUtils.CurrentUser = user;
                rt.Add("Data", user);
            }
            catch (Exception ex)
            {
                rt.Add("ErrorMessage", ex.Message);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult GetCurrentUserTmp()
        {
            RtObj rt = null;
            try
            {
                var user = LoginUtils.CurrentUser as SysUser;
                rt = new RtObj();
                if (user != null)
                {
                    rt.Data = user;
                }
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }


        public ContentResult Login(string userName, string password, string securityCode)
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
                rt = new RtObj(user);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
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
                        DistrictID = user.DistrictIDList
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