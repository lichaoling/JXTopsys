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
        public static List<string> GetMPSizeByMPType(int mpType)
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

        #region 道路字典
        #region 从当前表中获取社区名、小区名、道路名和自然村名
        public static List<string> getCommunityNamesFromData(int type, string NeighborhoodsID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var s = new List<string>();
                if (type == Enums.TypeInt.Residence)
                {
                    s = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == NeighborhoodsID).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.Road)
                {
                    s = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == NeighborhoodsID).Select(t => t.CommunityName).Distinct().ToList();
                }
                else if (type == Enums.TypeInt.Country)
                {
                    s = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.NeighborhoodsID == NeighborhoodsID).Select(t => t.CommunityName).Distinct().ToList();
                }
                return s;
            }
        }
        public static List<string> getResidenceNamesFromData(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<MPOfResidence> query = null;
                if (!string.IsNullOrEmpty(CountyID))
                    query = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Select(t => t.ResidenceName).ToList();

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
                    IQueryable<MPOfRoad> query = null;
                    if (!string.IsNullOrEmpty(CountyID))
                        query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighborhoodsID))
                        query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                    if (!string.IsNullOrEmpty(CommunityName))
                        query = query.Where(t => t.CommunityName == CommunityName);
                    name = query.Select(t => t.RoadName).ToList();
                }
                else if (type == Enums.TypeInt.RP)
                {
                    IQueryable<RP> query = null;
                    if (!string.IsNullOrEmpty(CountyID))
                        query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID);
                    if (!string.IsNullOrEmpty(NeighborhoodsID))
                        query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                    if (!string.IsNullOrEmpty(CommunityName))
                        query = query.Where(t => t.CommunityName == CommunityName);
                    name = query.Select(t => t.RoadName).ToList();
                }
                return name;
            }
        }
        public static List<string> getViligeNamesFromData(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<MPOfCountry> query = null;
                if (!string.IsNullOrEmpty(CountyID))
                    query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Select(t => t.ViligeName).ToList();
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
                IQueryable<ResidenceDic> query = null;
                if (!string.IsNullOrEmpty(CountyID))
                    query = dbContext.ResidenceDic.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Select(t => t.ResidenceName).ToList();

                return name;
            }
        }
        public static List<string> getCommunityNamesFromDic(string CountyID, string NeighborhoodsID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<CommunityDic> query = null;
                if (!string.IsNullOrEmpty(CountyID))
                    query = dbContext.CommunityDic.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                name = query.Select(t => t.CommunityName).ToList();
                return name;
            }
        }
        public static List<string> getRoadNamesFromDic(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<RoadDic> query = null;
                if (!string.IsNullOrEmpty(CountyID))
                    query = dbContext.RoadDic.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Select(t => t.RoadName).ToList();
                return name;
            }
        }
        public static List<string> getViligeNamesFromDic(string CountyID, string NeighborhoodsID, string CommunityName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> name = new List<string>();
                IQueryable<ViligeDic> query = null;
                if (!string.IsNullOrEmpty(CountyID))
                    query = dbContext.ViligeDic.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = query.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                name = query.Select(t => t.ViligeName).ToList();
                return name;
            }
        }
        #endregion
        public static string AddRoadDic(RoadDic roadDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.RoadDic.Where(t => t.CountyID == roadDic.CountyID).Where(t => t.NeighborhoodsID == roadDic.NeighborhoodsID).Where(t => t.CommunityName == roadDic.CommunityName).Where(t => t.RoadName == roadDic.RoadName).Where(t => t.RoadStart == roadDic.RoadStart).Where(t => t.RoadEnd == roadDic.RoadEnd).Where(t => t.BZRules == roadDic.BZRules).Where(t => t.StartEndNum == roadDic.StartEndNum).Where(t => t.Intersection == roadDic.Intersection
                ).FirstOrDefault();
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
        public static void AddCommunityDic(CommunityDic communityDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.CommunityDic.Where(t => t.CountyID == communityDic.CountyID).Where(t => t.NeighborhoodsID == communityDic.NeighborhoodsID).Where(t => t.CommunityName == communityDic.CommunityName).Where(t => t.CommunityName == communityDic.CommunityName).FirstOrDefault();
                if (query == null)
                {
                    communityDic.ID = Guid.NewGuid().ToString();
                    dbContext.CommunityDic.Add(communityDic);
                    dbContext.SaveChanges();
                }
            }
        }
        public static string AddViligeDic(ViligeDic viligeDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
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
        public static List<RoadDic> GetRoadInfosFromDic(string NeighborhoodsID, string CommunityName, string RoadName)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                IQueryable<RoadDic> query = null;
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    query = dbContext.RoadDic.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);
                if (!string.IsNullOrEmpty(RoadName))
                    query = query.Where(t => t.RoadName == RoadName);
                var data = query.ToList();
                return data;
            }
        }

        #endregion 道路字典

        #region 邮编
        public static List<string> GetPostcodeByDID(string CountyID, string NeighborhoodsID, string CommunityID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var codes = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(CountyID))
                    codes = codes.Where(t => t.CountyID == CountyID);
                if (!string.IsNullOrEmpty(NeighborhoodsID))
                    codes = codes.Where(t => t.NeighborhoodsID == NeighborhoodsID);
                if (!string.IsNullOrEmpty(CommunityID))
                    codes = codes.Where(t => t.CommunityID == CommunityID);
                var query = codes.Select(t => t.Postcode).ToList();
                return query;
            }
        }
        public static void AddPostcode(PostcodeDic postDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.PostcodeDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.CountyID == postDic.CountyID).Where(t => t.NeighborhoodsID == postDic.NeighborhoodsID).Where(t => t.CommunityID == postDic.CommunityID).Where(t => t.Postcode == postDic.Postcode).Count();
                if (count > 0)
                    throw new Exception($"该{postDic.Postcode}已经存在");
                PostcodeDic pDic = new PostcodeDic();
                pDic.ID = Guid.NewGuid().ToString();
                pDic.CountyID = postDic.CountyID;
                pDic.NeighborhoodsID = postDic.NeighborhoodsID;
                pDic.CommunityID = postDic.CommunityID;
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
                query.CommunityID = postDic.CommunityID;
                query.Postcode = postDic.Postcode;
                dbContext.SaveChanges();
            }
        }
        #endregion 邮编

        #region 路牌标志
        public static object GetRPBZDataFromDic(string Category)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = (from t in dbContext.RPBZDic
                            group t by t.Category into g
                            select new
                            {
                                Category = g.Key,
                                Data = g.Select(t => t.Data).ToList(),
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
                var Model = dbContext.RP.Select(t => t.Model).Distinct().ToList();
                var Material = dbContext.RP.Select(t => t.Material).Distinct().ToList();
                var Size = dbContext.RP.Select(t => t.Size).Distinct().ToList();
                return new Dictionary<string, object> {
                    { "Model",Model},
                    { "Material",Material},
                    { "Size",Size},
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