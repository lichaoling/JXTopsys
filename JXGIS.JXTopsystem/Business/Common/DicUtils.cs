using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Common
{
    public class DicUtils
    {
        #region 从当前表中获取社区名、小区名、道路名和自然村名
        public static List<string> getCommunityNamesFromData(int type, string DistrictID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var s = new List<string>();
                if (type == Enums.TypeInt.Residence)
                {
                    s = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.Road)
                {
                    s = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.Country)
                {
                    s = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.MP)
                {
                    s = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Concat(dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName)).Concat(dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName)).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.RP)
                {
                    s = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.PlaceName)
                {
                    s = dbContext.PlaceName.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }

                return s;
            }
        }
        public static List<string> getResidenceNamesFromData(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<MPOfResidence> query = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(CountyID))
                    query = query.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Where(t => !string.IsNullOrEmpty(t.ResidenceName)).Select(t => t.ResidenceName).Distinct().ToList();

                return name;
            }
        }
        public static List<string> getRoadNamesFromData(string CountyID, string NeighborhoodsID, string CommunityName, int type)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                if (type == Enums.TypeInt.Road)
                {
                    IQueryable<MPOfRoad> query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable);
                    query = BaseUtils.DataFilterWithTown<MPOfRoad>(query);
                    if (!string.IsNullOrEmpty(CountyID))
                        query = query.Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighborhoodsID))
                        query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                    if (!string.IsNullOrEmpty(CommunityName))
                        query = query.Where(t => t.CommunityName == CommunityName);
                    name = query.Where(t => !string.IsNullOrEmpty(t.RoadName)).Select(t => t.RoadName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.RP)
                {
                    IQueryable<RP> query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable);
                    query = BaseUtils.DataFilterWithTown<RP>(query);
                    if (!string.IsNullOrEmpty(CountyID))
                        query = query.Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighborhoodsID))
                        query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                    if (!string.IsNullOrEmpty(CommunityName))
                        query = query.Where(t => t.CommunityName == CommunityName);
                    name = query.Where(t => !string.IsNullOrEmpty(t.RoadName)).Select(t => t.RoadName).Distinct().ToList();
                }
                return name;
            }
        }
        public static List<string> getViligeNamesFromData(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<MPOfCountry> query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable);
                query = BaseUtils.DataFilterWithTown<MPOfCountry>(query);

                if (!string.IsNullOrEmpty(CountyID))
                    query = query.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Where(t => !string.IsNullOrEmpty(t.ViligeName)).Select(t => t.ViligeName).Distinct().ToList();
                return name;
            }
        }
        #endregion

        #region 从字典表中获取社区名、小区名、道路名和自然村名
        public static List<string> getResidenceNamesFromDic(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<ResidenceDic> query = dbContext.ResidenceDic;
                if (!string.IsNullOrEmpty(CountyID))
                    query = query.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Where(t => !string.IsNullOrEmpty(t.ResidenceName)).Select(t => t.ResidenceName).Distinct().ToList();

                return name;
            }
        }
        public static List<string> getCommunityNamesFromDic(string CountyID, string NeighborhoodsID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<CommunityDic> query = dbContext.CommunityDic;
                if (!string.IsNullOrEmpty(CountyID))
                    query = query.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                name = query.Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                return name;
            }
        }
        public static List<string> getRoadNamesFromDic(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<RoadDic> query = dbContext.RoadDic;
                if (!string.IsNullOrEmpty(CountyID))
                    query = query.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Where(t => !string.IsNullOrEmpty(t.RoadName)).Select(t => t.RoadName).Distinct().ToList();
                return name;
            }
        }
        public static List<string> getViligeNamesFromDic(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<ViligeDic> query = dbContext.ViligeDic;
                if (!string.IsNullOrEmpty(CountyID))
                    query = query.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Where(t => !string.IsNullOrEmpty(t.ViligeName)).Select(t => t.ViligeName).Distinct().ToList();
                return name;
            }
        }
        public static List<RoadDic> GetRoadInfosFromDic(string NeighborhoodsID, string CommunityName, string RoadName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                IQueryable<RoadDic> query = dbContext.RoadDic;
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                if (!string.IsNullOrEmpty(RoadName))
                    query = query.Where(t => t.RoadName == RoadName);
                var data = query.ToList();
                return data;
            }
        }
        #endregion

        #region 社区名、道路名、小区名、自然村名是动态在字典中添加
        public static void AddCommunityDic(CommunityDic communityDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (string.IsNullOrEmpty(communityDic.CommunityName))
                    communityDic.CommunityName = null;

                var query = dbContext.CommunityDic.Where(t => t.CountyID == communityDic.CountyID).Where(t => t.NeighborhoodsID == communityDic.NeighborhoodsID).Where(t => t.CommunityName == communityDic.CommunityName).Where(t => t.CommunityName == communityDic.CommunityName).FirstOrDefault();
                if (query == null)
                {
                    communityDic.ID = Guid.NewGuid().ToString();
                    dbContext.CommunityDic.Add(communityDic);
                    dbContext.SaveChanges();
                }
            }
        }
        public static string AddRoadDic(RoadDic roadDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (string.IsNullOrEmpty(roadDic.CommunityName))
                    roadDic.CommunityName = null;
                if (string.IsNullOrEmpty(roadDic.RoadName))
                    roadDic.RoadName = null;
                if (string.IsNullOrEmpty(roadDic.RoadStart))
                    roadDic.RoadStart = null;
                if (string.IsNullOrEmpty(roadDic.RoadEnd))
                    roadDic.RoadEnd = null;
                if (string.IsNullOrEmpty(roadDic.BZRules))
                    roadDic.BZRules = null;
                if (string.IsNullOrEmpty(roadDic.StartEndNum))
                    roadDic.StartEndNum = null;
                if (string.IsNullOrEmpty(roadDic.Intersection))
                    roadDic.Intersection = null;

                var query = dbContext.RoadDic.Where(t => t.CountyID == roadDic.CountyID).Where(t => t.NeighborhoodsID == roadDic.NeighborhoodsID).Where(t => t.CommunityName == roadDic.CommunityName).Where(t => t.RoadName == roadDic.RoadName).Where(t => t.RoadStart == roadDic.RoadStart).Where(t => t.RoadEnd == roadDic.RoadEnd).Where(t => t.BZRules == roadDic.BZRules).Where(t => t.StartEndNum == roadDic.StartEndNum).Where(t => t.Intersection == roadDic.Intersection).FirstOrDefault();
                string ID = null;
                if (query == null)
                {
                    roadDic.ID = Guid.NewGuid().ToString();
                    dbContext.RoadDic.Add(roadDic);
                    dbContext.SaveChanges();
                    ID = roadDic.ID;
                }
                else
                    ID = query.ID;
                return ID;
            }
        }
        public static string AddResidenceDic(ResidenceDic residenceDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (string.IsNullOrEmpty(residenceDic.CommunityName))
                    residenceDic.CommunityName = null;
                if (string.IsNullOrEmpty(residenceDic.ResidenceName))
                    residenceDic.ResidenceName = null;

                var query = dbContext.ResidenceDic.Where(t => t.CountyID == residenceDic.CountyID).Where(t => t.NeighborhoodsID == residenceDic.NeighborhoodsID).Where(t => t.CommunityName == residenceDic.CommunityName).Where(t => t.ResidenceName == residenceDic.ResidenceName).FirstOrDefault();
                string ID = null;
                if (query == null)
                {
                    residenceDic.ID = Guid.NewGuid().ToString();
                    dbContext.ResidenceDic.Add(residenceDic);
                    dbContext.SaveChanges();
                    ID = residenceDic.ID;
                }
                else
                    ID = query.ID;
                return ID;
            }
        }
        public static string AddViligeDic(ViligeDic viligeDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (string.IsNullOrEmpty(viligeDic.CommunityName))
                    viligeDic.CommunityName = null;
                if (string.IsNullOrEmpty(viligeDic.ViligeName))
                    viligeDic.ViligeName = null;

                var query = dbContext.ViligeDic.Where(t => t.CountyID == viligeDic.CountyID).Where(t => t.NeighborhoodsID == viligeDic.NeighborhoodsID).Where(t => t.CommunityName == viligeDic.CommunityName).Where(t => t.ViligeName == viligeDic.ViligeName).FirstOrDefault();
                string ID = null;
                if (query == null)
                {
                    viligeDic.ID = Guid.NewGuid().ToString();
                    dbContext.ViligeDic.Add(viligeDic);
                    dbContext.SaveChanges();
                    ID = viligeDic.ID;
                }
                else
                    ID = query.ID;
                return ID;
            }
        }
        #endregion 社区名、道路名、小区名、自然村名是自动添加

        #region 地名标志
        public static Object GetDMBZ()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = (from t in dbContext.DMBZDic
                            where t.State == Enums.UseState.Enable
                            group t by t.Type into g
                            select new
                            {
                                Type = g.Key,
                                data = (from f in g
                                        select new { ID = f.IndetityID, Size = f.Size, Material = f.Material }).ToList(),
                            }).ToList();
                return data;
            }
        }
        public static List<DMBZDic> GetDMBZFromDic()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).OrderBy(t => t.Type).ToList();
                return data;
            }
        }
        public static DMBZDic GetDMBZFromDicByID(int id)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.IndetityID == id).FirstOrDefault();
                return data;
            }
        }
        public static List<string> GetMPType()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var types = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Select(t => t.Type).Distinct().ToList();
                return types;
            }
        }
        public static List<string> GetMPSizeByTypeName(string type)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == type).Select(t => t.Size).ToList();
                return sizes;
            }
        }
        public static List<string> GetMPSizeByMPType(int? mpType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> sizes = new List<string>();
                if (mpType == Enums.TypeInt.Residence)
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == "户室牌").Select(t => t.Size).ToList();
                else if (mpType == Enums.TypeInt.Road)
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == "大门牌" || t.Type == "小门牌").Select(t => t.Size).ToList();
                else if (mpType == Enums.TypeInt.Country)
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == Enums.MPTypeCh.Country).Select(t => t.Size).ToList();
                else
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Select(t => t.Size).ToList();
                return sizes;
            }
        }
        public static void ModifyDMBZ(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<DMBZDic>(oldDataJson);
                if (sourceData.IndetityID == 0) //新增
                {
                    var targetData = new DMBZDic();
                    targetData.Type = sourceData.Type;
                    targetData.Size = sourceData.Size;
                    targetData.Material = sourceData.Material;
                    targetData.State = Enums.UseState.Enable;
                    dbContext.DMBZDic.Add(targetData);
                }
                else //修改
                {
                    var targetData = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.IndetityID == sourceData.IndetityID).FirstOrDefault();
                    if (targetData == null)
                        throw new Exception("该门牌标志已经被删除");
                    targetData.Type = sourceData.Type;
                    targetData.Size = sourceData.Size;
                    targetData.Material = sourceData.Material;
                }
                dbContext.SaveChanges();
            }
        }
        public static void DeleteDMBZ(DMBZDic dmbz)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var p = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.IndetityID == dmbz.IndetityID).FirstOrDefault();
                if (p == null)
                    throw new Exception("该邮编已经被删除！");
                p.State = Enums.UseState.Cancel;
                dbContext.SaveChanges();
            }
        }
        #endregion 地名标志

        #region 邮编   字典管理
        public static List<DistrictNode> getDistrictTreeFromPostcodeData()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var neighborhoodsIDs = dbContext.District.Where(t => t.State == Enums.UseState.Enable).Select(t => t.ID).Distinct().ToList();
                neighborhoodsIDs = neighborhoodsIDs.Where(t => t.Split('.').Count() > 2).ToList();
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
                DistrictUtils.GetLeaf(parent, tree);
                return parent;
            }
        }
        public static List<string> getCommunityNames(string NeighborhoodsID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var codes = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    codes = codes.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                var data = codes.Select(t => t.CommunityName).ToList();
                return data;
            }
        }
        public static List<string> GetPostcodeByDID(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var codes = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(CountyID))
                    codes = codes.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    codes = codes.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    codes = codes.Where(t => t.CommunityName == CommunityName);
                var data = codes.Select(t => t.Postcode).FirstOrDefault();
                var query = new List<string>();
                if (data != null)
                    query = data.Split(',').ToList();
                return query;
            }
        }
        public static List<PostcodeDetails> GetPostcodesFromCode(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var codes = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(CountyID))
                    codes = codes.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    codes = codes.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    codes = codes.Where(t => t.CommunityName == CommunityName);
                var data = codes.ToList();
                var rt = (from t in data
                          select new PostcodeDetails
                          {
                              IndetityID = t.IndetityID,
                              CountyID = t.CountyID,
                              CountyName = t.CountyID.Split('.').Last(),
                              NeighborhoodsID = t.NeighborhoodsID,
                              NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                              CommunityName = t.CommunityName,
                              Postcode = t.Postcode,
                          }).OrderBy(t => t.CountyID).ToList();
                return rt;
            }
        }
        public static List<string> GetPostcodesFromData(int type, string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> code = new List<string>();
                if (type == Enums.TypeInt.PlaceName)
                {
                    IQueryable<Models.Entities.PlaceName> query = dbContext.PlaceName.Where(t => t.State == Enums.UseState.Enable);
                    query = BaseUtils.DataFilterWithTown<Models.Entities.PlaceName>(query);
                    if (!string.IsNullOrEmpty(CountyID))
                        query = query.Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighborhoodsID))
                        query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                    if (!string.IsNullOrEmpty(CommunityName))
                        query = query.Where(t => t.CommunityName == CommunityName);
                    code = query.Where(t => !string.IsNullOrEmpty(t.Postcode)).Select(t => t.Postcode).Distinct().ToList();
                }
                return code;
            }
        }
        public static PostcodeDetails GetPostcodeByID(int id)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var t = dbContext.PostcodeDic.Where(s => s.State == Enums.UseState.Enable).Where(s => s.IndetityID == id).FirstOrDefault();
                var data = new PostcodeDetails();
                if (t != null)
                    data = new PostcodeDetails
                    {
                        IndetityID = t.IndetityID,
                        CountyID = t.CountyID,
                        CountyName = t.CountyID.Split('.').Last(),
                        NeighborhoodsID = t.NeighborhoodsID,
                        NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                        CommunityName = t.CommunityName,
                        Postcode = t.Postcode,
                    };
                return data;
            }
        }
        public static void ModifyPostcode(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<PostcodeDetails>(oldDataJson);
                var data = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == sourceData.NeighborhoodsID).Where(t => t.CommunityName == sourceData.CommunityName).FirstOrDefault();

                if (data == null) //新增
                {
                    var targetData = new PostcodeDic();
                    targetData.CountyID = sourceData.CountyID;
                    targetData.NeighborhoodsID = sourceData.NeighborhoodsID;
                    targetData.CommunityName = sourceData.CommunityName;
                    targetData.Postcode = !string.IsNullOrEmpty(sourceData.Postcode) ? sourceData.Postcode : "";
                    targetData.State = Enums.UseState.Enable;
                    dbContext.PostcodeDic.Add(targetData);
                }
                else //修改
                {
                    data.Postcode = !string.IsNullOrEmpty(sourceData.Postcode) ? sourceData.Postcode : data.Postcode;
                }
                dbContext.SaveChanges();
            }
        }
        public static void DeletePostcode(PostcodeDic post)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var p = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.IndetityID == post.IndetityID).FirstOrDefault();
                if (p == null)
                    throw new Exception("该邮政编码已经被删除！");
                p.State = Enums.UseState.Cancel;
                dbContext.SaveChanges();
            }
        }
        #endregion 邮编

        #region 路牌标志
        public static List<string> GetDirectionFromDic()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var rt = dbContext.DirectionDic.Where(t => !string.IsNullOrEmpty(t.Diretion)).Select(t => t.Diretion).ToList();
                return rt;
            }
        }
        public static List<string> GetIntersectionFromData(string RoadName)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var data = db.RP.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(RoadName))
                    data = data.Where(t => t.RoadName == RoadName);
                var rt = data.Select(t => t.Intersection).Distinct().ToList();
                return rt;
            }
        }
        public static List<string> GetDirectionFromData(string RoadName, string Intersection)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var data = db.RP.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(RoadName))
                    data = data.Where(t => t.RoadName == RoadName);
                if (!string.IsNullOrEmpty(Intersection))
                    data = data.Where(t => t.Intersection == Intersection);
                var rt = data.Select(t => t.Direction).Distinct().ToList();
                return rt;
            }
        }
        public static List<string> GetRepairContentFromDic()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var rt = dbContext.RPBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Category == "维修内容").Select(t => t.Data).Distinct().ToList();
                return rt;
            }
        }
        public static object GetRPBZDataFromDic(string Category)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = (from t in dbContext.RPBZDic
                            where t.State == Enums.UseState.Enable
                            group t by t.Category into g
                            select new
                            {
                                Category = g.Key,
                                Data = g.Where(t => !string.IsNullOrEmpty(t.Data)).Select(t => t.Data).ToList(),
                            }).ToList();
                if (!string.IsNullOrEmpty(Category))
                    data = data.Where(t => t.Category == Category).ToList();
                return data;
            }
        }
        public static List<RPBZDic> GetRPBZFromDic()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.RPBZDic.Where(t => t.State == Enums.UseState.Enable).OrderBy(t => t.Category).ToList();
                return data;
            }
        }
        public static RPBZDic GetRPBZFromDicByID(int id)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.RPBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.IndetityID == id).FirstOrDefault();
                return data;
            }
        }
        public static List<string> GetRPCategory()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var types = dbContext.RPBZDic.Where(t => t.State == Enums.UseState.Enable).Select(t => t.Category).Distinct().ToList();
                return types;
            }
        }
        public static Dictionary<string, object> GetRPBZDataFromData()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var Intersection = dbContext.RP.Where(t => !string.IsNullOrEmpty(t.Intersection)).Select(t => t.Intersection).Distinct().ToList();
                var Direction = dbContext.RP.Where(t => !string.IsNullOrEmpty(t.Direction)).Select(t => t.Direction).Distinct().ToList();
                var Model = dbContext.RP.Where(t => !string.IsNullOrEmpty(t.Model)).Select(t => t.Model).Distinct().ToList();
                var Material = dbContext.RP.Where(t => !string.IsNullOrEmpty(t.Material)).Select(t => t.Material).Distinct().ToList();
                var Size = dbContext.RP.Where(t => !string.IsNullOrEmpty(t.Size)).Select(t => t.Size).Distinct().ToList();
                var Manufacturers = dbContext.RP.Where(t => !string.IsNullOrEmpty(t.Manufacturers)).Select(t => t.Manufacturers).Distinct().ToList();
                var RepairMode = dbContext.RPRepair.Select(t => t.RepairMode).Distinct().ToList();
                var RepairedCount = (from t in dbContext.RPRepair
                                     group t by t.RPID into g
                                     select g.Count()).Distinct().ToList();

                var RepairParts = dbContext.RPRepair.Where(t => !string.IsNullOrEmpty(t.RepairParts)).Select(t => t.RepairParts).Distinct().ToList();
                var RepairFactory = dbContext.RPRepair.Where(t => !string.IsNullOrEmpty(t.RepairFactory)).Select(t => t.RepairFactory).Distinct().ToList();
                return new Dictionary<string, object> {
                    { "Intersection",Intersection},
                    { "Direction",Direction},
                    { "Model",Model},
                    { "Material",Material},
                    { "Size",Size},
                    { "Manufacturers",Manufacturers},
                    {"RepairMode",RepairMode },
                    {"RepairedCount",RepairedCount },
                    {"RepairParts",RepairParts },
                    {"RepairFactory",RepairFactory },
                };
            }
        }
        public static void ModifyRPBZ(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<RPBZDic>(oldDataJson);
                var targetData = dbContext.RPBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.IndetityID == sourceData.IndetityID).FirstOrDefault();
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                if (targetData == null) //新增
                {
                    targetData = new RPBZDic();
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                    targetData.State = Enums.UseState.Enable;
                    dbContext.RPBZDic.Add(targetData);
                }
                else //修改
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);

                }
                dbContext.SaveChanges();
            }
        }
        public static void DeleteRPBZ(RPBZDic rpbz)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var RPBZ = dbContext.RPBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.IndetityID == rpbz.IndetityID).FirstOrDefault();
                if (RPBZ == null)
                    throw new Exception("该路牌编制数据已经被删除！");
                RPBZ.State = Enums.UseState.Cancel;
                dbContext.SaveChanges();
            }
        }

        #endregion 路牌标志

    }
}