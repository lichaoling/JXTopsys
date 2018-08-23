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
        public static List<string> getNamesByDistrict(string CommunityID, int MPType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                if (MPType == Enums.MPType.Residence)
                    name = dbContext.MPOFResidence.Where(t => t.CommunityID == CommunityID).Select(t => t.ResidenceName).Distinct().ToList();
                else if (MPType == Enums.MPType.Road)
                    name = dbContext.MPOfRoad.Where(t => t.CommunityID == CommunityID).Select(t => t.RoadName).Distinct().ToList();
                else
                    name = dbContext.MPOfCountry.Where(t => t.CommunityID == CommunityID).Select(t => t.ViligeName).Distinct().ToList();
                return name;
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
        public static bool CheckPermission(string CommunityID)
        {
            var flag = false;
            var districtIDs = LoginUtils.CurrentUser.DistrictID;
            if (districtIDs != null && districtIDs.Count > 0 && !districtIDs.Contains("1"))
            {
                foreach (var districtID in districtIDs)
                {
                    if (CommunityID.IndexOf(districtID + ".") == 0)
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