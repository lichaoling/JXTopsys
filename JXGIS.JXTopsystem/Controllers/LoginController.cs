using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class LoginController : Controller
    {
        //public static string userName = System.Configuration.ConfigurationManager.AppSettings["username"];
        //public static string password = System.Configuration.ConfigurationManager.AppSettings["password"];

        //public ActionResult Test()
        //{
        //    var s = SecurityUtils.MD5Encrypt("@123456");
        //    var key = "12abcdef";
        //    var x = SecurityUtils.DESEncryt("哈哈哈哈", key);
        //    x = SecurityUtils.DESDecrypt(x, key);

        //    x = SecurityUtils.RSAEncrypt(x);
        //    x = SecurityUtils.RSADecrypt(x);

        //    return null;
        //}

        public ContentResult LoginTmp(string userName, string password)
        {
            Dictionary<string, object> rt = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    throw new Exception("用户名、密码不能为空！");
                }

                using (var db = SystemUtils.NewEFDbContext)
                {
                    var user = db.SysUser.Where(u => u.UserName == userName).FirstOrDefault();
                    if (user == null)
                    {
                        throw new Exception("用户不存在！");
                    }
                    else if (user.Password != password)
                    {
                        throw new Exception("用户名或密码错误！");
                    }
                    else
                    {
                        //user.DistrictIDList = new List<string>() { "嘉兴市" };
                        user.DistrictIDList = db.Database.SqlQuery<string>("select districtid from SYSUSER_DISTRICT where userid=@userid", new SqlParameter("@userid", user.UserID)).ToList();
                        user.TPrivileges = user.GetTPrivileges(db);
                        LoginUtils.CurrentUser = user;
                        rt.Add("Data", Models.SysUser.GetWebUser());
                    }
                }
            }
            catch (Exception ex)
            {
                rt.Add("ErrorMessage", ex.Message);
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
                rt = new RtObj();
                rt.Data = Models.SysUser.GetWebUser();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult GetCPrivileges()
        {
            RtObj rt = null;
            try
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    var prvs = db.CPrivilege.ToList();
                    var prvsn = (from p in prvs
                                 select new Models.TPrivilege
                                 {
                                     Id = p.Id,
                                     Name = p.Name,
                                     PId = p.PId,
                                     PassPrivilege = p.PassPrivilege,
                                 }).ToList();

                    var group = (from d in prvsn
                                 group d by d.PId into g
                                 select new Models.GroupX
                                 {
                                     P_ID = g.Key,
                                     Privileges = g.ToList()
                                 }).ToList();

                    var pIDNull = group.Where(t => t.P_ID == null).FirstOrDefault();

                    var start = pIDNull.Privileges.ToList();

                    Models.TPrivilege.GetTree(start, group, "none");

                    rt = new RtObj(start[0].SubPrivileges);
                }
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }

            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }


        public ContentResult GetPrivilege(string id)
        {
            RtObj rt = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("未找到指定数据");
                }

                using (var db = SystemUtils.NewEFDbContext)
                {
                    var prv = db.CPrivilege.Find(id);
                    rt = new RtObj(prv);
                }
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }

            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }


        public ContentResult ModifyPrivilege(string json)
        {
            RtObj rt = null;
            try
            {
                using (var db = SystemUtils.EFDbContext)
                {
                    // 未添加验证
                    var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<CPrivilege>(json);
                    var targetData = db.CPrivilege.Find(sourceData.Id);
                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    if (targetData == null)
                    {
                        db.CPrivilege.Add(sourceData);
                    }
                    else
                    {
                        ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }

            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult GetCRoles()
        {
            RtObj ro = null;
            try
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    var roles = db.CRole.ToList();
                    ro = new RtObj(roles);
                }
            }
            catch (Exception ex)
            {
                ro = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(ro);
            return Content(s);
        }

        public ContentResult GetCRole(string id)
        {
            RtObj ro = null;
            try
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    CRole role = null;
                    if (string.IsNullOrEmpty(id))
                    {
                        role = new CRole()
                        {
                            Id = Guid.NewGuid().ToString()
                        };
                    }
                    else
                    {
                        role = db.CRole.Find(id);
                    }
                    if (role != null)
                    {
                        var prvs = db.Database.SqlQuery<TPrivilege>(@"select t3.*,t.Privilege from CPRIVILEGE t3 left join (
select t2.* from crole t1 left join crole_cprivilege t2 on t1.id=t2.croleid  where t1.id=@id)t
on t.CPrivilegeId=t3.id;", new SqlParameter("@id", role.Id)).ToList();

                        var group = (from d in prvs
                                     group d by d.PId into g
                                     select new GroupX
                                     {
                                         P_ID = g.Key,
                                         Privileges = g.ToList()
                                     }).ToList();

                        var pIDNull = group.Where(t => t.P_ID == null).FirstOrDefault();

                        var start = pIDNull.Privileges.ToList();

                        TPrivilege.GetTree2(start, group);

                        role.TPrivilege = start[0];
                    }
                    ro = new RtObj(role);
                }
            }
            catch (Exception ex)
            {
                ro = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(ro);
            return Content(s);
        }

        public ContentResult ModifyCRole(string json)
        {
            RtObj rt = null;
            try
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<CRole>(json);
                    var targetData = db.CRole.Find(sourceData.Id);
                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    if (targetData == null)
                    {
                        db.CRole.Add(sourceData);
                    }
                    else
                    {
                        ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    }

                    string sql = "delete from crole_cprivilege where croleid=@roleid;";

                    List<SqlParameter> para = new List<SqlParameter>();
                    para.Add(new SqlParameter("@roleid", sourceData.Id));
                    int idx = 0;
                    foreach (var prv in sourceData.TPrivileges)
                    {
                        sql += string.Format("insert into crole_cprivilege(croleid,cprivilegeid,privilege) values(@roleid,@pid{0},@prv{0});", idx);
                        para.Add(new SqlParameter("@pid" + idx, prv.Id));
                        para.Add(new SqlParameter("@prv" + idx, prv.Privilege));
                        idx += 1;
                    }

                    db.Database.ExecuteSqlCommand(sql, para.ToArray());

                    db.SaveChanges();
                    rt = new RtObj();
                }
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }

            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }

        public ContentResult DeleteCRole(string id)
        {
            RtObj ro = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("没有提供有效的参数！");
                }
                using (var db = SystemUtils.NewEFDbContext)
                {
                    var count = db.Database.ExecuteSqlCommand("delete * from crole where roleid=@id;delete * from crole_cprivilege where croleid=@id;", new SqlParameter("@id", id));
                    // 是否需要saveChanges?
                    //db.SaveChanges();
                    ro = new RtObj();
                }
            }
            catch (Exception ex)
            {
                ro = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(ro);
            return Content(s);
        }

        public ContentResult GetUserWithPrivs(string id)
        {
            RtObj ro = null;
            try
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    var sql = @"with x as  
(select userid,id croleid,name crolename from SYSUSER_CROLE t1 left join crole t2 on t1.croleid=t2.Id ),
rs as (select userid,stuff((select '|'+croleid from x x1 where x1.userid=x2.userid
for xml path('')),1,1,'') croles from x x2
group by userid), 
d as (
select userid,districtid district from SYSUSER_DISTRICT t ),
ds as (select userid,stuff((select '|'+district from d d1 where d1.userid=d2.userid
for xml path('')),1,1,'') districts from d d2
group by userid)

select t.*,rs.croles RoleName,ds.districts DistrictName from SYSUSER t 
left join rs on rs.UserID=t.userid
left join ds on ds.userid=t.UserID
where t.userid=@userid";

                    var user = db.Database.SqlQuery<SysUserEx>(sql, new SqlParameter("@userid", id ?? string.Empty)).FirstOrDefault();
                    if (user == null)
                    {
                        user = new SysUserEx()
                        {
                            UserID = Guid.NewGuid().ToString()
                        };
                    }
                    ro = new RtObj(user);
                }
            }
            catch (Exception ex)
            {
                ro = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(ro);
            return Content(s);
        }


        public ContentResult ModifyUser(string json)
        {
            RtObj ro = null;
            try
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    var sourceData1 = Newtonsoft.Json.JsonConvert.DeserializeObject<SysUserEx>(json);
                    var sourceData2 = Newtonsoft.Json.JsonConvert.DeserializeObject<SysUser>(json);
                    var targetData = db.SysUser.Find(sourceData2.UserID);

                    var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    if (targetData == null)
                    {
                        if (db.SysUser.Where(i => i.UserName == sourceData2.UserName).Count() > 0) throw new Exception("已存在相同用户名用户，请重新命名！");
                        db.SysUser.Add(sourceData2);
                    }
                    else
                    {
                        if (db.SysUser.Where(i => i.UserName == sourceData2.UserName && i.UserID != sourceData2.UserID).Count() > 0) throw new Exception("已存在相同用户名用户，请重新命名！");
                        ObjectReflection.ModifyByReflection(sourceData2, targetData, Dic);
                    }

                    var sql = "delete sysuser_crole where userid=@userid;delete sysuser_district where userid=@userid;";
                    var cnt = 0;
                    var pars = new List<SqlParameter>();
                    pars.Add(new SqlParameter("@userid", sourceData2.UserID));
                    if (!string.IsNullOrEmpty(sourceData1.RoleName))
                    {
                        var rs = sourceData1.RoleName.Split('|');
                        foreach (var r in rs)
                        {
                            sql += string.Format("insert into sysuser_crole(userid,croleid) values(@userid,@r{0});", cnt);
                            pars.Add(new SqlParameter("@r" + cnt, r));
                            cnt += 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(sourceData1.DistrictName))
                    {
                        var ds = sourceData1.DistrictName.Split('|');
                        foreach (var d in ds)
                        {
                            sql += string.Format("insert into sysuser_district(userid,districtid) values(@userid,@d{0});", cnt);
                            pars.Add(new SqlParameter("@d" + cnt, d));
                            cnt += 1;
                        }
                    }

                    db.Database.ExecuteSqlCommand(sql, pars.ToArray());
                    db.SaveChanges();
                    ro = new RtObj();
                }
            }
            catch (Exception ex)
            {

                ro = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(ro);
            return Content(s);
        }

        public ContentResult GetDistrictTree()
        {
            RtObj ro = null;
            try
            {
                var districts = DistrictUtils.GetDistrictTree();
                ro = new RtObj(districts);
            }
            catch (Exception ex)
            {
                ro = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(ro);
            return Content(s);
        }

        public ActionResult ModifyPassword(string OPassword, string NPassword)
        {
            RtObj ro = null;
            try
            {
                var user = LoginUtils.CurrentUser as SysUser;
                if (user == null)
                {
                    throw new Exception("请先登录！");
                }

                if (string.IsNullOrWhiteSpace(OPassword) || string.IsNullOrWhiteSpace(NPassword))
                {
                    throw new Exception("密码不能为空！");
                }

                using (var db = SystemUtils.NewEFDbContext)
                {
                    var u = db.SysUser.Find(user.UserID);
                    if (u == null) throw new Exception("未找到指定用户！");
                    if (u.Password != OPassword)
                    {
                        throw new Exception("原始密码验证有误！");
                    }
                    u.Password = NPassword;
                    db.SaveChanges();
                }

                ro = new RtObj();
            }
            catch (Exception ex)
            {
                ro = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(ro);
            return Content(s);
        }
    }
}