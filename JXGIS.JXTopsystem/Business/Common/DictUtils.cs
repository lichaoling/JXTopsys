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
        public static List<string> GetMPType()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var types = dbContext.DMBZDic.Select(t => t.Type).Distinct().ToList();
                return types;
            }
        }

        public static List<string> GetMPSizeByTypeName(string type)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sizes = dbContext.DMBZDic.Where(t => t.Type == type).Select(t => t.Size).ToList();
                return sizes;
            }
        }

        public static List<string> GetMPSizeByMPType(int mpType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<string> sizes = new List<string>();
                if (mpType == Enums.MPType.Residence)
                    sizes = dbContext.DMBZDic.Where(t => t.Type == "户室牌").Select(t => t.Size).ToList();
                else if (mpType == Enums.MPType.Road)
                    sizes = dbContext.DMBZDic.Where(t => t.Type == "大门牌" || t.Type == "小门牌").Select(t => t.Size).ToList();
                else if (mpType == Enums.MPType.Country)
                    sizes = dbContext.DMBZDic.Where(t => t.Type == "农村门牌").Select(t => t.Size).ToList();
                else
                    sizes = dbContext.DMBZDic.Select(t => t.Size).ToList();
                return sizes;
            }
        }

        public static void AddMPSize(string type, string size)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var count = dbContext.DMBZDic.Where(t => t.Type == type).Where(t => t.Size == size).Count();
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



        #region 邮编
        public static List<string> GetPostcodeByPID(string CountyID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {

                var codes = dbContext.PostcodeDic.ToList();
                List<string> data = new List<string>();
                data = codes.Select(t => t.Postcode).ToList();
                if (!string.IsNullOrEmpty(CountyID))
                    data = codes.Where(t => t.PCode == CountyID).Select(t => t.Postcode).ToList();
                return data;
            }
        }
        #endregion

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
        #endregion

    }
}