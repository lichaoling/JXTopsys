﻿using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class LoginUtils
    {
        public static IUser Login(string userName, string password)
        {
            IUser user = null;
            var b = ValidateUser(userName, password, ref user);
            CurrentUser = user;
            return user;
        }
        public static void Logout()
        {
            CurrentUser = null;
        }
        private static string _user = "USER";

        /// <summary>
        /// 当前用户
        /// </summary>
        public static IUser CurrentUser
        {
            get
            {
#if DEBUG
                SysUser user = new SysUser();
                user.UserName = "测试用户";
                user.UserID = "1";
                user.DistrictIDList = new List<string>() { "嘉兴市.秀洲区.新塍镇", "嘉兴市.南湖区.新兴街道", "嘉兴市" };
                //user.DistrictID = new List<string>() { "嘉兴市.南湖区.新兴街道", "嘉兴市.秀洲区.塘汇街道" };
                //user.DistrictID = new List<string>() { "嘉兴市.南湖区.建设街道"};
                user.WindowList = new List<string>() { "地名办公室", "便民窗口" };
                HttpContext.Current.Session[_user] = user;
                return HttpContext.Current != null ? (HttpContext.Current.Session[_user] as IUser) : null;
#endif
                return HttpContext.Current != null ? (HttpContext.Current.Session[_user] as IUser) : null;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Session[_user] = value;
                }
            }
        }

        /// <summary>
        /// 系统内是否有用户名为userName的用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool HasUser(string userName)
        {
            return SystemUtils.NewEFDbContext.SysUser.Where(t => t.State == Enums.UseState.Enable).Where(s => s.UserName == userName).Count() > 0;
        }

        /// <summary>
        /// 验证用户是否合法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool ValidateUser(string userName, string password, ref IUser user)
        {
            var bSuccess = false;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) return false;
            var us = SystemUtils.NewEFDbContext.SysUser.Where(t => t.State == Enums.UseState.Enable).Where(u => u.UserName == userName && u.Password == password).FirstOrDefault();
            if (us != null)
            {
                bSuccess = true;
                var roleID = SystemUtils.NewEFDbContext.UserRole.Where(t => t.UserID == us.UserID).Select(t => t.RoleID).ToList();
                var districtID = SystemUtils.NewEFDbContext.SysRole.Where(t => t.State == Enums.UseState.Enable).Where(t => roleID.Contains(t.RoleID)).Select(t => t.DistrictID).ToList();
                if (districtID.Count == 0)
                    throw new Exception("对不起，您目前没有任何权限，请联系管理员！");
                us.DistrictIDList = districtID;
                var Window = SystemUtils.NewEFDbContext.SysRole.Where(t => t.State == Enums.UseState.Enable).Where(t => roleID.Contains(t.RoleID)).Select(t => t.Window).Distinct().ToList();
                us.WindowList = Window;
            }
            user = us;
            return bSuccess;
        }

        /// <summary>
        /// 当前是否有用户
        /// </summary>
        public static bool IsLogin
        {
            get
            {
                return CurrentUser == null;
            }
        }

        private static string _validateGraphicCode = "VALIDATEGRAPHICCODE";

        private static ValidateGraphic validateGraphic = new ValidateGraphic();
        /// <summary>
        /// 当前验证码
        /// </summary>
        public static ValidateGraphic CurrentValidateGraphicCode
        {
            get
            {
                if (validateGraphic == null)
                {
                    validateGraphic = new ValidateGraphic();
                }
                return validateGraphic;
            }

            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Session[_validateGraphicCode] = value;
                }
            }
        }
    }
}