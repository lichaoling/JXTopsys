using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    [Serializable]
    public class TPrivilege
    {
        public string Id { get; set; }

        public string PId { get; set; }

        public string Name { get; set; }

        public string PassPrivilege { get; set; }

        public string Privilege { get; set; }

        public List<TPrivilege> SubPrivileges { get; set; }

        public static string GetTree(List<TPrivilege> nodes, List<GroupX> groups, string Privilege)
        {
            var prv = Privilege;
            foreach (var node in nodes)
            {
                var g = groups.Where(x => x.P_ID == node.Id).FirstOrDefault();

                // 自己是否设了权限？
                if (!string.IsNullOrEmpty(node.Privilege))
                {
                    if (node.Privilege == "edit" || node.Privilege == "view")
                    {
                        prv = node.Privilege;
                    }


                    if (g != null && g.Privileges != null)
                    {
                        node.SubPrivileges = g.Privileges;
                        var tmp = GetTree(node.SubPrivileges, groups, node.Privilege);

                        if (tmp != "none" && node.Privilege != "edit")
                        {
                            node.Privilege = "view";
                        }
                    }
                }
                else
                {
                    node.Privilege = Privilege;
                    if (g != null && g.Privileges != null)
                    {
                        node.SubPrivileges = g.Privileges;
                        var tmp = GetTree(node.SubPrivileges, groups, Privilege);

                        if (tmp != "none")
                        {
                            prv = tmp;
                            if (node.Privilege != "edit") node.Privilege = "view";
                        }
                    }
                }
            }
            return prv;
        }

        /// <summary>
        /// 不上传和回溯权限
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="groups"></param>
        /// <param name="Privilege"></param>
        /// <returns></returns>
        public static void GetTree2(List<TPrivilege> nodes, List<GroupX> groups)
        {
            foreach (var node in nodes)
            {
                var g = groups.Where(x => x.P_ID == node.Id).FirstOrDefault();

                if (g != null && g.Privileges != null)
                {
                    node.SubPrivileges = g.Privileges;
                    GetTree2(node.SubPrivileges, groups);
                }
            }
        }
    }
}