using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
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
            var districts = SystemUtils.Districts.Select(t => new DistrictNode()
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
        /// <summary>
        /// 根据行政区划id获取行政区划树
        /// </summary>
        /// <param name="districtIDs"></param>
        /// <returns></returns>
        public static List<DistrictNode> getDistrictTree(List<string> districtIDs)
        {
            var districtTree = new List<District>();

            if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("1"))
            {
                foreach (var districtID in districtIDs)
                {
                    var query1 = SystemUtils.Districts.Where(t => t.ID.IndexOf(districtID + ".") == 0).ToList();
                    districtTree.AddRange(query1);

                    var ids = districtID.Split('.');
                    var concat = "";
                    foreach (var id in ids)
                    {
                        concat = (concat + '.' + id).Trim('.');
                        var query2 = SystemUtils.Districts.Where(t => t.ID == concat).ToList();
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
        /// <summary>
        /// 根据行政区划和门牌类型来获取所有的小区名称或道路名称或自然村名称
        /// </summary>
        /// <param name="CommunityID"></param>
        /// <param name="MPType"></param>
        /// <returns></returns>
        public static List<string> getNamesByDistrict(string CountyID, string NeighbourhoodID, string CommunityID, int MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                if (MPType == Enums.MPType.Residence)
                {
                    IQueryable<MPOfResidence> query = null;
                    if (!string.IsNullOrEmpty(CountyID))
                        query = dbContext.MPOFResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighbourhoodID))
                        query = query.Where(t => t.NeighborhoodsID == NeighbourhoodID);
                    if (!string.IsNullOrEmpty(CommunityID))
                        query = query.Where(t => t.CommunityID == CommunityID);
                    name = query.Select(t => t.ResidenceName).Distinct().ToList();
                }
                else if (MPType == Enums.MPType.Road)
                {
                    IQueryable<MPOfRoad> query = null;
                    if (!string.IsNullOrEmpty(CountyID))
                        query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighbourhoodID))
                        query = query.Where(t => t.NeighborhoodsID == NeighbourhoodID);
                    if (!string.IsNullOrEmpty(CommunityID))
                        query = query.Where(t => t.CommunityID == CommunityID);
                    name = query.Select(t => t.RoadName).Distinct().ToList();
                }
                else if (MPType == Enums.MPType.Country)
                {
                    IQueryable<MPOfCountry> query = null;
                    if (!string.IsNullOrEmpty(CountyID))
                        query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighbourhoodID))
                        query = query.Where(t => t.NeighborhoodsID == NeighbourhoodID);
                    if (!string.IsNullOrEmpty(CommunityID))
                        query = query.Where(t => t.CommunityID == CommunityID);
                    name = query.Select(t => t.ViligeName).Distinct().ToList();
                }

                return name;
            }
        }
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
                if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("1"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<SysRole>();
                    foreach (var ID in districtIDs)
                    {
                        where = where.Or(t => t.DistrictID.IndexOf(ID + ".") == 0);
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
        public static List<string> getCreateUsers(List<string> districtIDs)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var users = dbContext.SysUser.Where(t => t.State == Enums.UseState.Enable);
                var roles = dbContext.SysRole.Where(t => t.State == Enums.UseState.Enable);
                if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("1"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<SysRole>();
                    foreach (var ID in districtIDs)
                    {
                        where = where.Or(t => t.DistrictID.IndexOf(ID + ".") == 0);
                    }
                    var query = roles.Where(where.Compile());
                    var roleIDS = query.Select(t => t.RoleID).ToList();
                    var userIDS = dbContext.UserRole.Where(t => roleIDS.Contains(t.RoleID)).Select(t => t.UserID).ToList();
                    users = users.Where(t => userIDS.Contains(t.UserID));
                }
                var createUsers = users.Select(t => t.UserName).ToList();
                return createUsers;
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
        /// <summary>
        /// 在修改或新增数据时判断目前用户是否有权限操作这个区域的数据
        /// </summary>
        /// <param name="CommunityID"></param>
        /// <returns></returns>
        public static bool CheckPermission(string Neighbourhood)
        {
            var flag = false;
            var districtIDs = LoginUtils.CurrentUser.DistrictID;
            if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("1"))
            {
                foreach (var districtID in districtIDs)
                {
                    if (Neighbourhood.IndexOf(districtID + ".") == 0 || Neighbourhood == districtID)
                        flag = true;
                }
            }
            else
            {
                flag = true;
            }
            return flag;
        }
        #endregion
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