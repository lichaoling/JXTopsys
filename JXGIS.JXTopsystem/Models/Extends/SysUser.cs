using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    public partial class SysUser
    {
        [NotMapped]
        public List<TPrivilege> TPrivileges { get; set; }

        public List<TPrivilege> GetTPrivileges(SqlDBContext db)
        {
            var sqlPrvs = @"with prv as(
  --用户所有组件权限
  select tt.CPrivilegeId,tt.Privilege from SYSUSER_CROLE t 
  inner join CROLE_CPRIVILEGE tt on t.croleid=tt.CRoleId
  where t.UserID= @userid),
  pprvs as (
  --获取最终权限
  --权限交叉时取最高权限
  select t.CPrivilegeId,(case when l=1 then 'view' when l=2 then 'edit' else 'none' end) Privilege from (
  select t.CPrivilegeId,max(l) l from (
  select t.CPrivilegeId,
  (case when Privilege='view' then 1 when Privilege='edit' then 2 else 0 end) l from prv t) t
  group by t.CPrivilegeId) t)
  select * from CPRIVILEGE t left join pprvs p on t.Id=p.CPrivilegeId;";
            var prvs = db.Database.SqlQuery<TPrivilege>(sqlPrvs, new SqlParameter("@userid", this.UserID)).ToList();

            var group = (from d in prvs
                         group d by d.PId into g
                         select new GroupX
                         {
                             P_ID = g.Key,
                             Privileges = g.ToList()
                         }).ToList();

            var pIDNull = group.Where(t => t.P_ID == null).FirstOrDefault();

            var start = pIDNull.Privileges.ToList();

            TPrivilege.GetTree(start, group, "none");

            var prv = new List<TPrivilege>();

            foreach (var g in group)
            {
                foreach (var gg in g.Privileges)
                {
                    gg.SubPrivileges = null;
                }
                prv.AddRange(g.Privileges);
            }
            prv = prv.Where(d => d.Privilege != "none" && !string.IsNullOrEmpty(d.Privilege)).ToList();
            return prv;
        }

        // 获取前台用户，将后台用户转为前台用户，提供给前台使用
        public static object GetWebUser()
        {
            var user = LoginUtils.CurrentUser as SysUser;
            if (user == null)
            {
                return null;
            }
            var prvs = new Dictionary<string, object>();
            foreach (var i in user.TPrivileges)
            {
                prvs[i.Id] = new
                {
                    pass = i.PassPrivilege.Contains(i.Privilege),
                    edit = i.Privilege == "edit"
                };
            }

            return new
            {
                userId = user.UserName,
                userName = user.Name,
                privileges = prvs
            };
        }
    }
}