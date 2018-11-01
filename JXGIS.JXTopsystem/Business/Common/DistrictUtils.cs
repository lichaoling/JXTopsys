using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Common
{
    public class DistrictUtils
    {
        #region 行政区划
        /// <summary>
        /// 获取行政区划树
        /// </summary>
        /// <returns></returns>
        public static List<DistrictNode> getDistrictTree()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var districts = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Select(t => new DistrictNode()
                {
                    ID = t.ID,
                    PID = t.ParentID,
                    Code = t.Code,
                    Name = t.Name
                }).ToList();
                var Newdistricts = districts.Where(t => t.PID == null).ToList();
                GetLeaf(Newdistricts, districts);
                return Newdistricts;
            }

        }
        /// <summary>
        /// 根据行政区划id获取行政区划树
        /// </summary>
        /// <param name="districtIDs"></param>
        /// <returns></returns>
        public static List<DistrictNode> getDistrictTreeFromDistrict(List<string> districtIDs)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var districtTree = new List<District>();
                if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                {
                    foreach (var districtID in districtIDs)
                    {
                        var query1 = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID.IndexOf(districtID + ".") == 0).ToList();
                        districtTree.AddRange(query1);

                        var ids = districtID.Split('.');
                        var concat = "";
                        foreach (var id in ids)
                        {
                            concat = (concat + '.' + id).Trim('.');
                            var query2 = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == concat).ToList();
                            districtTree.AddRange(query2);
                        }
                    }
                    var nodes = (from t in districtTree
                                 group t by t.ID into g
                                 select new DistrictNode
                                 {
                                     ID = g.Key,
                                     PID = g.First().ParentID,
                                     Code = g.First().Code,
                                     Name = g.First().Name
                                 }).ToList();
                    var Newnodes = nodes.Where(t => t.PID == null).ToList();
                    GetLeaf(Newnodes, nodes);
                    return Newnodes;
                }
                else
                    return getDistrictTree();
            }
        }
        /// <summary>
        /// 根据用户当前的行政区划ids找出三类门牌中的行政区划组织成树的形式
        /// </summary>
        /// <param name="type"></param>
        /// <param name="districtIDs"></param>
        /// <returns></returns>
        public static List<DistrictNode> getDistrictTreeFromData(int type, List<string> districtIDs)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var neighborhoodsIDs = new List<string>();
                if (type == Enums.TypeInt.Residence)
                {
                    if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                    {
                        foreach (var districtID in districtIDs)
                        {
                            var s = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(districtID + ".") == 0 || t.NeighborhoodsID == districtID).Select(t => t.NeighborhoodsID).Distinct().ToList();
                            neighborhoodsIDs.AddRange(s);
                        }
                    }
                    else
                    {
                        var s = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Select(t => t.NeighborhoodsID).Distinct().ToList();
                        neighborhoodsIDs.AddRange(s);
                    }
                }
                else if (type == Enums.TypeInt.Road)
                {
                    if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                    {
                        foreach (var districtID in districtIDs)
                        {
                            var s = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(districtID + ".") == 0 || t.NeighborhoodsID == districtID).Select(t => t.NeighborhoodsID).Distinct().ToList();
                            neighborhoodsIDs.AddRange(s);
                        }
                    }
                    else
                    {
                        var s = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Select(t => t.NeighborhoodsID).Distinct().ToList();
                        neighborhoodsIDs.AddRange(s);
                    }
                }
                else if (type == Enums.TypeInt.Country)
                {
                    if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                    {
                        foreach (var districtID in districtIDs)
                        {
                            var s = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(districtID + ".") == 0 || t.NeighborhoodsID == districtID).Select(t => t.NeighborhoodsID).Distinct().ToList();
                            neighborhoodsIDs.AddRange(s);
                        }
                    }
                    else
                    {
                        var s = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Select(t => t.NeighborhoodsID).Distinct().ToList();
                        neighborhoodsIDs.AddRange(s);
                    }
                }
                else if (type == Enums.TypeInt.MP)
                {
                    if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                    {
                        foreach (var districtID in districtIDs)
                        {
                            var s = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(districtID + ".") == 0 || t.NeighborhoodsID == districtID).Select(t => t.NeighborhoodsID).Concat(dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(districtID + ".") == 0 || t.NeighborhoodsID == districtID).Select(t => t.NeighborhoodsID)).Concat(dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(districtID + ".") == 0 || t.NeighborhoodsID == districtID).Select(t => t.NeighborhoodsID)).Distinct().ToList();
                            neighborhoodsIDs.AddRange(s);
                        }
                    }
                    else
                    {
                        var s = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Select(t => t.NeighborhoodsID).Distinct().ToList();
                        neighborhoodsIDs.AddRange(s);
                    }
                }

                else if (type == Enums.TypeInt.RP)
                {
                    if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                    {
                        foreach (var districtID in districtIDs)
                        {
                            var s = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(districtID + ".") == 0 || t.NeighborhoodsID == districtID).Select(t => t.NeighborhoodsID).Distinct().ToList();
                            neighborhoodsIDs.AddRange(s);
                        }
                    }
                    else
                    {
                        var s = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Select(t => t.NeighborhoodsID).Distinct().ToList();
                        neighborhoodsIDs.AddRange(s);
                    }
                }

                var nIDs = neighborhoodsIDs.Select(t => t.Split('.')).ToList();
                List<DistrictNode> tree = new List<Common.DistrictNode>();
                foreach (var nID in nIDs)
                {
                    var DistrictNode1 = new DistrictNode();
                    var DistrictNode2 = new DistrictNode();
                    var DistrictNode3 = new DistrictNode();

                    DistrictNode1.ID = nID[0];
                    DistrictNode1.PID = null;
                    DistrictNode1.Name = nID[0];

                    DistrictNode2.ID = DistrictNode1.ID + "." + nID[1];
                    DistrictNode2.PID = DistrictNode1.ID;
                    DistrictNode2.Name = nID[1];

                    DistrictNode3.ID = DistrictNode2.ID + "." + nID[2];
                    DistrictNode3.PID = DistrictNode2.ID;
                    DistrictNode3.Name = nID[2];

                    tree.Add(DistrictNode1);
                    tree.Add(DistrictNode2);
                    tree.Add(DistrictNode3);
                }
                tree = (from t in tree
                        group t by new
                        {
                            t.PID,
                            t.ID,
                            t.Name
                        } into g
                        select new DistrictNode()
                        {
                            ID = g.Key.ID,
                            PID = g.Key.PID,
                            Name = g.Key.Name
                        }).ToList();

                var parent = tree.Where(t => t.PID == null).ToList();
                GetLeaf(parent, tree);
                return parent;

            }
        }
        private static void GetLeaf(List<DistrictNode> nodes, List<DistrictNode> all)
        {
            foreach (var n in nodes)
            {
                var leafs = all.Where(t => t.PID == n.ID).ToList();
                if (leafs.Count != 0)
                {
                    n.SubDistrict = leafs;
                    GetLeaf(leafs, all);
                }
            }
        }

        public static void AddCounty(string CountyName, string Code)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                District dis = new District();
                dis.ID = $"嘉兴市.{CountyName}";
                dis.ParentID = "嘉兴市";
                dis.Code = Code;
                dis.Name = CountyName;
                dis.State = Enums.UseState.Enable;
                dbContext.District.Add(dis);
                dbContext.SaveChanges();
            }
        }
        public static void AddNeighborhoods(string CountyName, string NeighborhoodsName, string Code)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                District dis = new District();
                dis.ID = $"嘉兴市.{CountyName}.{NeighborhoodsName}";
                dis.ParentID = $"嘉兴市.{CountyName}";
                dis.Code = Code;
                dis.Name = NeighborhoodsName;
                dis.State = Enums.UseState.Enable;
                dbContext.District.Add(dis);
                dbContext.SaveChanges();
            }
        }
        public static void DeleteCounty(string CountyName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var County = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Name == CountyName).FirstOrDefault();
                if (County == null)
                    throw new Exception("该县区已经被删除！");
                County.State = Enums.UseState.Cancel;
                var neighbors = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ParentID == "嘉兴市." + CountyName).ToList();
                foreach (var n in neighbors)
                    n.State = Enums.UseState.Cancel;
                dbContext.SaveChanges();
            }
        }
        public static void DeleteNeighborhoods(string NeighborhoodsName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var neighbor = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Name == NeighborhoodsName).FirstOrDefault();
                if (neighbor == null)
                    throw new Exception("该乡镇已经被删除！");
                neighbor.State = Enums.UseState.Cancel;
                dbContext.SaveChanges();
            }
        }
        #endregion


        /// <summary>
        /// 获取当前用户权限内的受理窗口
        /// </summary>
        /// <param name="districtIDs"></param>
        /// <returns></returns>
        public static List<string> getWindows(List<string> districtIDs)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var windows = dbContext.SysRole.Where(t => t.State == Enums.UseState.Enable);
                var query = windows as IEnumerable<SysRole>;
                if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<SysRole>();
                    foreach (var ID in districtIDs)
                    {
                        where = where.Or(t => t.DistrictID.IndexOf(ID + ".") == 0 || t.DistrictID == ID);
                    }
                    query = windows.Where(where.Compile());
                }
                var data = query.Select(t => t.Window).Distinct().ToList();
                return data;
            }
        }
        /// <summary>
        /// 获取当前用户权限内的经办人
        /// </summary>
        /// <param name="districtIDs"></param>
        /// <returns></returns>
        public static List<string> getCreateUsers(List<string> districtIDs, string window)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var users = dbContext.SysUser.Where(t => t.State == Enums.UseState.Enable);
                var roles = dbContext.SysRole.Where(t => t.State == Enums.UseState.Enable);
                if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<SysRole>();
                    foreach (var ID in districtIDs)
                    {
                        where = where.Or(t => t.DistrictID.IndexOf(ID + ".") == 0 || t.DistrictID == ID);
                    }
                    var query = roles.Where(where.Compile()).Distinct();

                    if (!string.IsNullOrEmpty(window))
                    {
                        query = query.Where(t => t.Window == window);
                    }
                    var roleIDS = query.Select(t => t.RoleID).Distinct().ToList();
                    var userIDS = dbContext.UserRole.Where(t => roleIDS.Contains(t.RoleID)).Select(t => t.UserID).Distinct().ToList();
                    users = users.Where(t => userIDS.Contains(t.UserID));
                }
                var createUsers = users.Select(t => t.UserName).Distinct().ToList();
                return createUsers;
            }
        }

        /// <summary>
        /// 在修改或新增数据时判断目前用户是否有权限操作这个区域的数据
        /// </summary>
        /// <param name="CommunityID"></param>
        /// <returns></returns>
        public static bool CheckPermission(string NeighborhoodsID)
        {
            var districtIDs = LoginUtils.CurrentUser.DistrictID;
            return districtIDs.Where(t => NeighborhoodsID.IndexOf(t + ".") == 0 || NeighborhoodsID == t).Count() > 0;
        }
    }

    public class DistrictNode
    {
        public string ID { get; set; }
        public string PID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<DistrictNode> SubDistrict { get; set; }
    }
}