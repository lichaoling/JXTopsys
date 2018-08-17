using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPSearch
{
    public class RPSearchUtils
    {
        public static Dictionary<string, object> SearchRP(int PageSize, int PageNum, string DistrictID, string RoadName, string Intersection, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, string start, string end, int UseState)
        {
            int count = 0;
            List<RPDetails> data = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var q = dbContext.RP.Where(t => t.State == UseState);
                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<RP>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                }
                var query = q.Where(where.Compile());
                //行政区划筛选
                if (!string.IsNullOrEmpty(DistrictID))
                {
                    query = query.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                }
                //交叉路口筛选
                if (!string.IsNullOrEmpty(Intersection))
                {
                    query = query.Where(t => t.Intersection.Contains(Intersection));
                }
                //样式筛选
                if (!string.IsNullOrEmpty(Model))
                {
                    query = query.Where(t => t.Model.Contains(Model));
                }
                //规格筛选
                if (!string.IsNullOrEmpty(Size))
                {
                    query = query.Where(t => t.Size.Contains(Size));
                }
                //材质筛选
                if (!string.IsNullOrEmpty(Material))
                {
                    query = query.Where(t => t.Material.Contains(Material));
                }
                //生产厂家筛选
                if (!string.IsNullOrEmpty(Manufacturers))
                {
                    query = query.Where(t => t.Manufacturers.Contains(Manufacturers));
                }
                //正面宣传语筛选
                if (!string.IsNullOrEmpty(FrontTagline))
                {
                    query = query.Where(t => t.FrontTagline.Contains(FrontTagline));
                }
                //反面宣传语筛选
                if (!string.IsNullOrEmpty(BackTagline))
                {
                    query = query.Where(t => t.BackTagline.Contains(BackTagline));
                }
                //设置时间筛选
                if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                {
                    if (!string.IsNullOrEmpty(start))
                    {
                        q = q.Where(t => String.Compare(t.CreateTime.ToString(), start, StringComparison.Ordinal) >= 0);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        q = q.Where(t => String.Compare(t.CreateTime.ToString(), end, StringComparison.Ordinal) <= 0);
                    }
                }
                //道路名称筛选
                if (!string.IsNullOrEmpty(RoadName))
                {
                    query = from t in query
                            join d in dbContext.Road
                            on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                            from dt in dd.DefaultIfEmpty()
                            where dt.RoadName.Contains(RoadName)
                            select t;
                }
                count = query.Count();
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    query = query.OrderByDescending(t => t.CreateTime);
                }
                //如果是分页查询，就分页返回
                else
                {
                    query = query.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize);
                }

                data = (from t in query
                        join d in dbContext.Road
                        on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                        from dt in dd.DefaultIfEmpty()
                        select new RPDetails
                        {
                            ID = t.ID,
                            CountyID = t.CountyID,
                            NeighborhoodsID = t.NeighborhoodsID,
                            CommunityID = t.CommunityID,
                            RoadName = dt.RoadName,
                            Intersection = t.Intersection,
                            Direction = t.Direction,
                            CreateTime = t.CreateTime,
                            RepairedCount = t.RepairedCount
                        }).ToList();

                data = (from t in data
                        join a in SystemUtils.Districts
                        on t.CountyID equals a.ID into aa
                        from at in aa.DefaultIfEmpty()

                        join b in SystemUtils.Districts
                        on t.NeighborhoodsID equals b.ID into bb
                        from bt in bb.DefaultIfEmpty()

                        join c in SystemUtils.Districts
                        on t.CommunityID equals c.ID into cc
                        from ct in cc.DefaultIfEmpty()
                        select new RPDetails
                        {
                            ID = t.ID,
                            CountyID = t.CountyID,
                            NeighborhoodsID = t.NeighborhoodsID,
                            CommunityID = t.CommunityID,
                            CountyName = at == null || at.Name == null ? null : at.Name,
                            NeighborhoodsName = bt == null || bt.Name == null ? null : bt.Name,
                            CommunityName = ct == null || ct.Name == null ? null : ct.Name,
                            RoadName = t.RoadName,
                            Intersection = t.Intersection,
                            Direction = t.Direction,
                            CreateTime = t.CreateTime,
                            RepairedCount = t.RepairedCount
                        }).ToList();

                List<RPDetails> rt = new List<RPDetails>();
                foreach (var d in data)
                {
                    RPDetails r = new RPDetails();
                    r = d;
                    var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == d.ID);
                    if (files.Count() > 0)
                    {
                        var filelst = (from t in files
                                       select new Pictures
                                       {
                                           pid = t.ID,
                                           name = t.Name,
                                           url = "Files/RP/" + d.ID + "/" + t.ID + t.FileType
                                       }).ToList();
                        r.Files = filelst;
                    }
                    rt.Add(r);
                }
                return new Dictionary<string, object> {
                   { "Data",rt},
                   { "Count",count}
                };

            }
        }

        public static RPDetails SearchRPByID(string ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var q = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t=>t.ID==ID).FirstOrDefault();
                if(q==null)
                    throw new Exception("该路牌已经被注销！");

            }
        }
    }
}