using JXGIS.JXTopsystem.Models;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Common
{
    public class DictUtils
    {
        #region 地名标志
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
                if (mpType == Enums.MPType.Residence)
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == "户室牌").Select(t => t.Size).ToList();
                else if (mpType == Enums.MPType.Road)
                    sizes = dbContext.DMBZDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.Type == "大门牌" || t.Type == "小门牌").Select(t => t.Size).ToList();
                else if (mpType == Enums.MPType.Country)
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
        public static void ModifyRoadDic(RoadDic roadDic)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.RoadDic.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == roadDic.ID).FirstOrDefault();
                if (query == null)
                    throw new Exception($"该道路已经被删除");
                query.RoadStart = roadDic.RoadStart;
                query.RoadEnd = roadDic.RoadEnd;
                query.MPRules = roadDic.MPRules;
                dbContext.SaveChanges();
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
        public static List<string> GetRPBZData(string Category)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var datas = dbContext.RPBZDic.Where(t => t.Category == Category).Select(t => t.Data).ToList();
                return datas;
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