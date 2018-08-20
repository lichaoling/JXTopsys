using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Common
{
    public class RoadUtils
    {
        //public static Dictionary<string, object> GetRoads(int PageSize, int PageNum, string Name)
        //{
        //    using (var dbContext = SystemUtils.NewEFDbContext)
        //    {
        //        var query = dbContext.Road.Where(t => true);

        //        if (!string.IsNullOrEmpty(Name))
        //        {
        //            query = query.Where(t => t.RoadName.Contains(Name));
        //        }

        //        var count = query.Count();
        //        var rt = query.OrderBy(t => t.RoadName).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();

        //        var data = (from t in rt
        //                    join a in SystemUtils.Districts
        //                    on t.CountyID equals a.ID into aa
        //                    from at in aa.DefaultIfEmpty()

        //                    join b in SystemUtils.Districts
        //                    on t.NeighborhoodsID equals b.ID into bb
        //                    from bt in bb.DefaultIfEmpty()
        //                    select new RoadDetails
        //                    {
        //                        RoadID = t.RoadID,
        //                        RoadName = t.RoadName,
        //                        StartPoint = t.StartPoint,
        //                        EndPoint = t.EndPoint,
        //                        Category = t.Category,
        //                        PassBy = t.PassBy,
        //                        CountyID = t.CountyID,
        //                        CountyName = at == null || at.Name == null ? null : at.Name,
        //                        NeighborhoodsID = t.NeighborhoodsID,
        //                        NeighborhoodsName = bt == null || bt.Name == null ? "其它" : bt.Name,
        //                        Description = t.Description,
        //                        RoadGeom = t.RoadGeom,
        //                    }).ToList();

        //        for (var i = 0; i < data.Count(); i++)
        //        {
        //            var json = alatas.GeoJSON4EntityFramework.GeoJsonGeometry.FromDbGeography(data[i].RoadGeom);
        //            data[i].RoadGeomStr = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        //        }

        //        return new Dictionary<string, object> {
        //           { "Data",data},
        //           { "Count",count}
        //        };
        //    }
        //}

        //public static List<RoadDetails> GetRoadByID(string roadID)
        //{
        //    using (var dbContext = SystemUtils.NewEFDbContext)
        //    {
        //        List<RoadDetails> rt = null;
        //        if (!string.IsNullOrEmpty(roadID))
        //        {
        //            var query = dbContext.Road.Where(t => t.RoadID.ToString().ToLower() == roadID.ToLower());
        //            var qt = (from t in query
        //                      select t).ToList();
        //            var q = from t in qt
        //                    join a in SystemUtils.Districts
        //                    on t.CountyID equals a.ID into aa
        //                    from at in aa.DefaultIfEmpty()

        //                    join b in SystemUtils.Districts
        //                    on t.NeighborhoodsID equals b.ID into bb
        //                    from bt in bb.DefaultIfEmpty()
        //                    select new RoadDetails
        //                    {
        //                        RoadID = t.RoadID,
        //                        RoadName = t.RoadName,
        //                        StartPoint = t.StartPoint,
        //                        EndPoint = t.EndPoint,
        //                        Category = t.Category,
        //                        PassBy = t.PassBy,
        //                        CountyID = t.CountyID,
        //                        CountyName = at == null || at.Name == null ? null : at.Name,
        //                        NeighborhoodsID = t.NeighborhoodsID,
        //                        NeighborhoodsName = bt == null || bt.Name == null ? "其它" : bt.Name,
        //                        Description = t.Description,
        //                        RoadGeom = t.RoadGeom,
        //                    };

        //            rt = q.ToList();
        //            for (var i = 0; i < rt.Count(); i++)
        //            {
        //                var json = alatas.GeoJSON4EntityFramework.GeoJsonGeometry.FromDbGeography(rt[i].RoadGeom);
        //                rt[i].RoadGeomStr = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        //            }
        //        }
        //        return rt;
        //    }
        //}


        public static List<string> GetRoadsByDistrict(string CountyID, string NeighborhoodsID, string CommunityID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var names = dbContext.MPOfRoad.Where(t => t.CountyID == CountyID).Where(t => t.NeighborhoodsID == NeighborhoodsID).Where(t => t.CommunityID == CommunityID).Select(t => t.RoadName).Distinct().ToList();
                return names;
            }
        }
    }
}