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
        public static void GetLeaf(List<DistrictNode> nodes, List<DistrictNode> all)
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

        #region 行政区划字典管理
        /// <summary>
        /// 新增或者修改行政区划
        /// </summary>
        /// <param name="CountyName"></param>
        /// <param name="Code"></param>
        public static void ModifyDist(string oldDataJson)  //ID = $"嘉兴市.{CountyName}";
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<DistrictDetails>(oldDataJson);

                if (sourceData.ID == null) //新增
                {
                    var targetData = new District();
                    targetData.ID = "嘉兴市." + sourceData.CountyName + (!string.IsNullOrEmpty(sourceData.NeighborhoodsName) ? "." + sourceData.NeighborhoodsName : "");
                    targetData.ParentID = "嘉兴市" + (!string.IsNullOrEmpty(sourceData.NeighborhoodsName) ? "." + sourceData.CountyName : "");
                    targetData.Code = sourceData.Code;
                    targetData.Name = !string.IsNullOrEmpty(sourceData.NeighborhoodsName) ? sourceData.NeighborhoodsName : sourceData.CountyName;
                    targetData.State = Enums.UseState.Enable;
                    dbContext.District.Add(targetData);
                }
                else //修改
                {
                    var targetData = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                    var dist = targetData.ID.Split('.');

                    if (dist.Count() == 2 && !string.IsNullOrEmpty(sourceData.CountyName))//如果修改了区县,并且是区县的名字，要修改掉所有这个区县底下所有街道的ID和PID
                    {
                        var county = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ParentID.Contains(targetData.Name)).ToList();
                        foreach (var ct in county)
                        {
                            var dis = ct.ID.Split('.');
                            ct.ID = dis[0] + "." + sourceData.CountyName + "." + dis[2];
                            ct.ParentID = dis[0] + "." + sourceData.CountyName;
                        }
                        targetData.ID = dist[0] + "." + sourceData.CountyName;
                        targetData.Name = sourceData.CountyName;
                    }
                    //如果是镇街修改
                    if (dist.Count() == 3 && (!string.IsNullOrEmpty(sourceData.CountyName) || !string.IsNullOrEmpty(sourceData.NeighborhoodsName)))
                    {
                        targetData.ID = "嘉兴市" + (!string.IsNullOrEmpty(sourceData.CountyName) ? "." + sourceData.CountyName : "." + dist[1]) + (!string.IsNullOrEmpty(sourceData.NeighborhoodsName) ? "." + sourceData.NeighborhoodsName : "." + dist[2]);
                        targetData.ParentID = "嘉兴市" + (!string.IsNullOrEmpty(sourceData.CountyName) ? "." + sourceData.CountyName : "." + dist[1]);
                        targetData.Code = !string.IsNullOrEmpty(sourceData.Code) ? sourceData.Code : targetData.Code;
                        targetData.Name = !string.IsNullOrEmpty(sourceData.NeighborhoodsName) ? sourceData.NeighborhoodsName : targetData.Name;
                    }
                    //如果修改了区县，但不是名字
                    if (dist.Count() == 2 && !string.IsNullOrEmpty(sourceData.Code))
                    {
                        targetData.Code = sourceData.Code;
                    }

                }
                dbContext.SaveChanges();
            }
        }
        public static void DeleteDist(DistrictDetails dis)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var dist = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == dis.ID).FirstOrDefault();
                if (dist == null)
                    throw new Exception("该行政区划已经被删除！");
                dist.State = Enums.UseState.Cancel;
                if (dis.ID.Split('.').Length < 3)//区县数据的删除，还要删除区县所属的街道
                {
                    var neighbors = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ParentID == dis.ID).ToList();
                    foreach (var n in neighbors)
                        n.State = Enums.UseState.Cancel;
                }
                dbContext.SaveChanges();
            }
        }
        public static object SearchDist()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var rt = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ParentID != null).ToList();
                var data = (from t in rt
                            select new
                            {
                                ID = t.ID,
                                CountyName = !string.IsNullOrEmpty(t.ID) ? t.ID.Split('.')[1] : null,
                                NeighborhoodsName = (t.ID.Split('.')).Count() > 2 ? t.ID.Split('.')[2] : null,
                                Code = t.Code,
                            }).OrderBy(t => t.CountyName).ToList();
                return data;
            }
        }
        public static List<string> GetCountys()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var countys = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ParentID == "嘉兴市").Select(t => t.Name).ToList();
                return countys;
            }
        }
        #endregion


        /// <summary>
        /// 新增镇街
        /// </summary>
        /// <param name="CountyName"></param>
        /// <param name="NeighborhoodsName"></param>
        /// <param name="Code"></param>
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
        #endregion  行政区划



        /// <summary>
        /// 获取当前用户权限内的受理窗口
        /// </summary>
        /// <param name="districtIDs"></param>
        /// <returns></returns>
        public static List<string> GetWindows(List<string> districtIDs)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var userDistrict = dbContext.UserDistrict.Where(t => true);

                if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<SysUser_District>();
                    foreach (var ID in districtIDs)
                    {
                        where = where.Or(t => t.DistrictID.IndexOf(ID + ".") == 0 || t.DistrictID == ID);
                    }
                    userDistrict = userDistrict.Where(where);
                }
                var uids = userDistrict.Select(t => t.UserID).Distinct().ToList();
                var data = dbContext.SysUser.Where(t => uids.Contains(t.UserID)).Select(t => t.Window).Distinct().ToList();
                return data;
            }
        }
        /// <summary>
        /// 获取当前用户权限内的经办人
        /// </summary>
        /// <param name="districtIDs"></param>
        /// <returns></returns>
        public static List<string> GetCreateUsers(List<string> districtIDs, string window)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var userDistrict = dbContext.UserDistrict.Where(t => true);
                if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<SysUser_District>();
                    foreach (var ID in districtIDs)
                    {
                        where = where.Or(t => t.DistrictID.IndexOf(ID + ".") == 0 || t.DistrictID == ID);
                    }
                    userDistrict = userDistrict.Where(where).Distinct();
                }
                var uids = userDistrict.Select(t => t.UserID).Distinct().ToList();
                var users = dbContext.SysUser.Where(t => uids.Contains(t.UserID));
                if (!string.IsNullOrEmpty(window))
                {
                    users = users.Where(t => t.Window == window);
                }
                var names = users.Select(t => t.UserName).Distinct().ToList();
                return names;
            }
        }


        #region 权限管理

        #endregion

        #region 角色管理
        public static List<DistrictNode> GetDistrictTreeFromRole()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var neighborhoodsIDs = dbContext.UserDistrict.Select(t => t.DistrictID).Distinct().ToList();
                var nIDs = neighborhoodsIDs.Where(t => t.Split('.').Length > 2).Select(t => t.Split('.')).ToList();
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
        public static void ModifyRole(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<SysRole>(oldDataJson);

                if (sourceData.RoleID == null) //新增
                {
                    var targetData = new SysRole();
                    //targetData.RoleID = Guid.NewGuid().ToString();
                    //targetData.Role = sourceData.Role;
                    //targetData.Window = sourceData.Window;
                    //targetData.DistrictID = sourceData.DistrictID;
                    //targetData.CreateTime = DateTime.Now.Date;
                    //targetData.CreateUser = LoginUtils.CurrentUser.UserName;
                    //dbContext.SysRole.Add(targetData);
                }
                else //修改
                {
                    var targetData = dbContext.SysRole.Where(t => t.RoleID == sourceData.RoleID).FirstOrDefault();
                    //targetData.Role = sourceData.Role;
                    //targetData.Window = sourceData.Window;
                    //targetData.DistrictID = sourceData.DistrictID;
                    //targetData.LastModifyTime = DateTime.Now.Date;
                    //targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;
                }
                dbContext.SaveChanges();
            }
        }
        public static void DeleteRole(SysRole role)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var r = dbContext.SysRole.Where(t => t.RoleID == role.RoleID).FirstOrDefault();
                if (r == null)
                    throw new Exception("该角色已经被删除！");
                var rs = dbContext.UserRole.Where(t => t.RoleID == role.RoleID).ToList();
                dbContext.UserRole.RemoveRange(rs);
                dbContext.SysRole.Remove(r);
                dbContext.SaveChanges();
            }
        }
        public static List<SysRole> SearchRole(string DistrictID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                //var query = dbContext.SysRole.Where(t => true);
                //if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "嘉兴市"))
                //{
                //    query = query.Where(t => t.DistrictID == DistrictID || t.DistrictID.IndexOf(DistrictID + ".") == 0);
                //}
                //var data = query.OrderBy(t => t.DistrictID).ToList();
                //return data;
                return null;
            }
        }
        public static List<string> GetWindows()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.SysUser.Select(t => t.Window).Distinct().ToList();
                return data;
            }
        }
        public static List<string> GetRoleNames()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.SysRole.Select(t => t.RoleName).Distinct().ToList();
                return data;
            }
        }
        #endregion

        #region 用户管理
        public static void ModifyUser(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<SysUser>(oldDataJson);
                if (sourceData.UserID == null) //新增
                {
                    var targetData = new SysUser();
                    targetData.UserID = Guid.NewGuid().ToString();
                    targetData.UserName = sourceData.UserName;
                    targetData.Password = sourceData.Password;
                    targetData.Name = sourceData.Name;
                    targetData.Gender = sourceData.Gender;
                    targetData.Email = sourceData.Email;
                    targetData.Birthday = sourceData.Birthday;
                    targetData.Telephone = sourceData.Telephone;
                    targetData.CreateTime = DateTime.Now.Date;
                    targetData.CreateUser = LoginUtils.CurrentUser.UserName;
                    dbContext.SysUser.Add(targetData);

                    List<SysUser_SysRole> userroles = new List<SysUser_SysRole>();
                    List<SysUser_District> userdistricts = new List<SysUser_District>();
                    foreach (var role in sourceData.RoleList)
                    {
                        SysUser_SysRole userrole = new SysUser_SysRole();
                        userrole.UserID = targetData.UserID;
                        userrole.RoleID = role.RoleID;
                        userroles.Add(userrole);
                    }
                    foreach (var did in sourceData.DistrictIDList)
                    {
                        SysUser_District userDistrict = new SysUser_District();
                        userDistrict.UserID = targetData.UserID;
                        userDistrict.DistrictID = did;
                        userdistricts.Add(userDistrict);
                    }

                    dbContext.UserRole.AddRange(userroles);
                    dbContext.UserDistrict.AddRange(userdistricts);
                }
                else //修改
                {
                    var targetData = dbContext.SysUser.Where(t => t.UserID == sourceData.UserID).FirstOrDefault();
                    targetData.UserID = Guid.NewGuid().ToString();
                    targetData.UserName = sourceData.UserName;
                    targetData.Password = sourceData.Password;
                    targetData.Name = sourceData.Name;
                    targetData.Gender = sourceData.Gender;
                    targetData.Email = sourceData.Email;
                    targetData.Birthday = sourceData.Birthday;
                    targetData.Telephone = sourceData.Telephone;
                    targetData.LastModifyTime = DateTime.Now.Date;
                    targetData.LastModifyUser = LoginUtils.CurrentUser.UserName;

                    var userrolesRe = dbContext.UserRole.Where(t => t.UserID == targetData.UserID).ToList();
                    dbContext.UserRole.RemoveRange(userrolesRe);

                    var userdistrictRe = dbContext.UserDistrict.Where(t => t.UserID == targetData.UserID).ToList();
                    dbContext.UserDistrict.RemoveRange(userdistrictRe);

                    List<SysUser_SysRole> userroles = new List<SysUser_SysRole>();
                    List<SysUser_District> userdistricts = new List<SysUser_District>();
                    foreach (var role in sourceData.RoleList)
                    {
                        SysUser_SysRole userrole = new SysUser_SysRole();
                        userrole.UserID = targetData.UserID;
                        userrole.RoleID = role.RoleID;
                        userroles.Add(userrole);
                    }
                    foreach (var did in sourceData.DistrictIDList)
                    {
                        SysUser_District userDistrict = new SysUser_District();
                        userDistrict.UserID = targetData.UserID;
                        userDistrict.DistrictID = did;
                        userdistricts.Add(userDistrict);
                    }

                    dbContext.UserRole.AddRange(userroles);
                    dbContext.UserDistrict.AddRange(userdistricts);

                }
                dbContext.SaveChanges();
            }
        }
        public static void DeleteUser(SysUser user)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var u = dbContext.SysUser.Where(t => t.UserID == user.UserID).FirstOrDefault();
                if (u == null)
                    throw new Exception("该用户已经被删除！");

                var rs = dbContext.UserRole.Where(t => t.UserID == user.UserID).ToList();
                dbContext.UserRole.RemoveRange(rs);
                dbContext.SysUser.Remove(u);
                dbContext.SaveChanges();
            }
        }
        public static List<SysUser> SearchUser()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var users = dbContext.SysUser.ToList();

                List<SysUser> sysUserDetails = new List<SysUser>();
                foreach (var user in users)
                {
                    var roleids = dbContext.UserRole.Where(t => t.UserID == user.UserID).Select(t => t.RoleID).ToList();
                    var rolesList = dbContext.SysRole.Where(t => roleids.Contains(t.RoleID)).ToList();
                    var districtIDList = dbContext.UserDistrict.Where(t => t.UserID == user.UserID).Select(t => t.DistrictID).Distinct().ToList();
                    var districtName = districtIDList.Select(t => t.Replace('.', '/')).ToList();

                    List<SysRole_SysPrivilige> rolePriviliges = new List<Models.Entities.SysRole_SysPrivilige>();
                    foreach (var role in rolesList)
                    {
                        rolePriviliges.AddRange(dbContext.RolePrivilige.Where(t => t.RoleID == role.RoleID).ToList());
                    }

                    SysUser sysUsers = new SysUser()
                    {
                        UserID = user.UserID,
                        UserName = user.UserName,
                        Password = user.Password,
                        Name = user.Name,
                        Gender = user.Gender,
                        Email = user.Email,
                        Birthday = user.Birthday,
                        Telephone = user.Telephone,
                        Window = user.Window,
                        DistrictName = string.Join("；", districtName),
                        RoleList = rolesList,
                        RoleName = string.Join("；", rolesList.Select(t => t.RoleName).ToList()),
                        DistrictIDList = districtIDList,
                        PriviligeList = rolePriviliges.Distinct().ToList(),
                    };
                    sysUserDetails.Add(sysUsers);
                }
                return sysUserDetails.OrderBy(t => t.DistrictName).ToList();
            }
        }
        public static List<SysRole> GetRoles(string DistrictID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                //var data = dbContext.SysRole.Where(t => t.DistrictID == DistrictID).ToList();
                //return data;
                return null;
            }
        }
        #endregion


        /// <summary>
        /// 在修改或新增数据时判断目前用户是否有权限操作这个区域的数据
        /// </summary>
        /// <param name="CommunityID"></param>
        /// <returns></returns>
        public static bool CheckPermission(string NeighborhoodsID)
        {
            var districtIDs = LoginUtils.CurrentUser.DistrictIDList;
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