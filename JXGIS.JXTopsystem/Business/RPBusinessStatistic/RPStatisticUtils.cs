using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPBusinessStatistic
{
    public class RPStatisticUtils
    {
        public static Dictionary<string, object> GetRPNumTJ(int PageSize, int PageNum, string start, string end, string DistrictID, string CommunityName, string RoadName, string Model, string Material, string Size)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                {
                    if (!string.IsNullOrEmpty(start))
                    {
                        query = query.Where(t => String.Compare(t.BZTime.ToString(), start, StringComparison.Ordinal) >= 0);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        query = query.Where(t => String.Compare(t.BZTime.ToString(), end, StringComparison.Ordinal) <= 0);
                    }
                }

                if (!string.IsNullOrEmpty(DistrictID))
                    query = query.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + '.') == 0 || t.NeighborhoodsID == DistrictID);

                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);

                if (!string.IsNullOrEmpty(RoadName))
                    query = query.Where(t => t.RoadName == RoadName);

                if (!string.IsNullOrEmpty(Model))
                    query = query.Where(t => t.Model == Model);

                if (!string.IsNullOrEmpty(Material))
                    query = query.Where(t => t.Material == Material);

                if (!string.IsNullOrEmpty(Size))
                    query = query.Where(t => t.Size == Size);

                var re = from t in query
                         group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName, t.RoadName, t.Model, t.Material, t.Size } into g
                         select new
                         {
                             CountyID = g.Key.CountyID,
                             NeighborhoodsID = g.Key.NeighborhoodsID,
                             CommunityName = g.Key.CommunityName,
                             RoadName = g.Key.RoadName,
                             Model = g.Key.Model,
                             Material = g.Key.Material,
                             Size = g.Key.Size,
                             Count = g.Count(),
                         };
                var count = re.Count();
                var result = re.Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = from t in result
                           select new
                           {
                               CountyID = t.CountyID,
                               CountyName = t.CountyID.Split('.').Last(),
                               NeighborhoodsID = t.NeighborhoodsID,
                               NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                               CommunityName = t.CommunityName,
                               RoadName = t.RoadName,
                               Model = t.Model,
                               Material = t.Material,
                               Size = t.Size,
                               Count = t.Count,
                           };
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };

            }
        }

        public static Dictionary<string, object> GetRPRepairTJ(int PageSize, int PageNum, string DistrictID, string CommunityName)
        {
            return null;
        }
    }
}