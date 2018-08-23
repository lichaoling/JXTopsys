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
    }
}