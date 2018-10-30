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
        #region 地名标志
        public static List<DMBZDic> GetDMBZ()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).ToList();
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
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == "农村门牌").Select(t => t.Size).ToList();
                else
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Select(t => t.Size).ToList();
                return sizes;
            }
        }
        public static void AddMPSize(string type, string size)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == type).Where(t => t.Size == size).Count();
                if (count > 0)
                    throw new Exception("门牌规格已经存在");
                DMBZDic dmbz = new Models.Entities.DMBZDic();
                dmbz.ID = Guid.NewGuid().ToString();
                dmbz.Type = type;
                dmbz.Size = size;
                dbContext.DMBZDic.Add(dmbz);
                dbContext.SaveChanges();
            }
        }
        #endregion 地名标志

        #region 从当前表中获取社区名、小区名、道路名和自然村名
        public static List<string> getCommunityNamesFromData(int type, string NeighborhoodsID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var s = new List<string>();
                if (type == Enums.TypeInt.Residence)
                {
                    s = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.Road)
                {
                    s = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.Country)
                {
                    s = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.RP)
                {
                    s = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).Distinct().ToList();
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
                name = query.Where(t => !string.IsNullOrEmpty(t.ResidenceName)).Select(t => t.ResidenceName).ToList();

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
                name = query.Where(t => !string.IsNullOrEmpty(t.CommunityName)).Select(t => t.CommunityName).ToList();
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
                name = query.Where(t => !string.IsNullOrEmpty(t.RoadName)).Select(t => t.RoadName).ToList();
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
                name = query.Where(t => !string.IsNullOrEmpty(t.ViligeName)).Select(t => t.ViligeName).ToList();
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

        #region 字典添加
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
        #endregion

        #region 邮编
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
                var query = codes.Select(t => t.Postcode).ToList();
                return query;
            }
        }
        public static void AddPostcode(PostcodeDic postDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == postDic.CountyID).Where(t => t.NeighborhoodsID == postDic.NeighborhoodsID).Where(t => t.CommunityName == postDic.CommunityName).Where(t => t.Postcode == postDic.Postcode).Count();
                if (count > 0)
                    throw new Exception($"该{postDic.Postcode}已经存在");
                PostcodeDic pDic = new PostcodeDic();
                pDic.ID = Guid.NewGuid().ToString();
                pDic.CountyID = postDic.CountyID;
                pDic.NeighborhoodsID = postDic.NeighborhoodsID;
                pDic.CommunityName = postDic.CommunityName;
                pDic.Postcode = postDic.Postcode;
                dbContext.PostcodeDic.Add(pDic);
                dbContext.SaveChanges();
            }
        }
        public static void ModifyPostcode(PostcodeDic postDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == postDic.ID).FirstOrDefault();
                if (query == null)
                    throw new Exception($"该邮编已经被删除");
                query.CountyID = postDic.CountyID;
                query.NeighborhoodsID = postDic.NeighborhoodsID;
                query.CommunityName = postDic.CommunityName;
                query.Postcode = postDic.Postcode;
                dbContext.SaveChanges();
            }
        }
        public static Dictionary<string, object> GetPostcodes(int PageSize, int PageNum)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = from t in dbContext.PostcodeDic
                           where t.State == Enums.UseState.Enable
                           group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                           select new
                           {
                               g.Key.CountyID,
                               g.Key.NeighborhoodsID,
                               g.Key.CommunityName,
                               Postcode = g.Select(t => t.Postcode).ToList()
                           };
                int count = data.Count();
                var query = data.OrderByDescending(t => t.NeighborhoodsID).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var rt = (from t in query
                          select new
                          {
                              CountyName = t.CountyID.Split('.').Last(),
                              NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                              CommunityName = t.CommunityName,
                              Postcode = t.Postcode,
                          }).ToList();
                return new Dictionary<string, object> {
                   { "Data",rt},
                   { "Count",count}
                };
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
        public static List<string> GetRepairContentFromDic()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var rt = dbContext.RPRepairContent.Where(t => !string.IsNullOrEmpty(t.RepairContent)).Select(t => t.RepairContent).ToList();
                return rt;
            }
        }

        public static object GetRPBZDataFromDic(string Category)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = (from t in dbContext.RPBZDic
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
        public static void AddRPBZData(string Category, string Data)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.RPBZDic.Where(t => t.Category == Category).Where(t => t.Data == Data).Count();
                if (count > 0)
                    throw new Exception($"该{Category}已经存在");
                RPBZDic rpbz = new Models.Entities.RPBZDic();
                rpbz.ID = Guid.NewGuid().ToString();
                rpbz.Category = Category;
                rpbz.Data = Data;
                dbContext.RPBZDic.Add(rpbz);
                dbContext.SaveChanges();
            }
        }
        #endregion 路牌标志

    }
}