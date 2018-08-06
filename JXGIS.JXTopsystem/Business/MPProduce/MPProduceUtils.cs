using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.MPProduce
{
    public class MPProduceUtils
    {
        /// <summary>
        /// 分页返回道路门牌和农村门牌的制作情况
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID">行政区划ID</param>
        /// <param name="MPProduce">制作类型 1待制作 2不制作 3已制作 0全部</param>
        /// <param name="MPType">门牌类型 1住宅门牌 2道路门牌 3农村门牌 0全部</param>
        /// <param name="Name">道路名称或者自然村名称</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetProduceMP(int PageSize, int PageNum, string DistrictID, int MPProduce, int MPType, string Name)
        {
            int count = 0;
            List<MPProduceList> data = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var all = (from a in dbContext.MPOfRoad
                         join d in dbContext.Road
                         on a.RoadID == null ? a.RoadID : a.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                         from dt in dd.DefaultIfEmpty()
                         where a.State == Enums.UseState.Enable
                         select new MPProduceList
                         {
                             CountyID = a.CountyID,
                             NeighborhoodsID = a.NeighborhoodsID,
                             CommunityID = a.CommunityID,
                             MPType = Enums.MPType.Road,
                             MPID = a.ID,
                             MPTypeName = "道路门牌",
                             MPProduce = a.MPProduce,
                             MPProduceName = a.MPProduce == Enums.MPProduce.ToBeMade ? "待制作" : (a.MPProduce == Enums.MPProduce.NotMake ? "不制作" : "已制作"),
                             PlaceName = dt.RoadName,
                             MPNumber = a.MPNumber,
                             MPSize = a.MPSize,
                             Postcode = a.Postcode,
                             MPCreateTime = a.CreateTime
                         }).Concat(
                             from b in dbContext.MPOfCountry
                             where b.State == Enums.UseState.Enable
                             select new MPProduceList
                             {
                                 CountyID = b.CountyID,
                                 NeighborhoodsID = b.NeighborhoodsID,
                                 CommunityID = b.CommunityID,
                                 MPType = Enums.MPType.Country,
                                 MPID = b.ID,
                                 MPTypeName = "农村门牌",
                                 MPProduce = b.MPProduce,
                                 MPProduceName = b.MPProduce == Enums.MPProduce.ToBeMade ? "待制作" : (b.MPProduce == Enums.MPProduce.NotMake ? "不制作" : "已制作"),
                                 PlaceName = b.ViligeName,
                                 MPNumber = b.MPNumber,
                                 MPSize = b.MPSize,
                                 Postcode = b.Postcode,
                                 MPCreateTime = b.CreateTime
                             }
                            );

                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPProduceList>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                }
                var q = all.Where(where.Compile());

                if (!string.IsNullOrEmpty(DistrictID))
                {
                    q = q.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                }
                if (MPProduce != 0)
                {
                    q = q.Where(t => t.MPProduce == MPProduce);
                }
                if (MPType != Enums.MPType.All)
                {
                    q = q.Where(t => t.MPType == MPType);
                }
                if (string.IsNullOrEmpty(Name))
                {
                    q = q.Where(t => t.PlaceName.Contains(Name));
                }
                count = q.Count();

                data = q.OrderByDescending(t => t.MPCreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();

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
                        select new MPProduceList
                        {
                            CountyID = t.CountyID,
                            NeighborhoodsID = t.NeighborhoodsID,
                            CommunityID = t.CommunityID,
                            CountyName = at == null || at.Name == null ? null : at.Name,
                            NeighborhoodsName = bt == null || bt.Name == null ? null : bt.Name,
                            CommunityName = ct == null || ct.Name == null ? null : ct.Name,
                            MPType = Enums.MPType.Road,
                            MPID = t.ID,
                            MPTypeName = "道路门牌",
                            MPProduce = t.MPProduce,
                            MPProduceName = t.MPProduceName,
                            PlaceName = t.PlaceName,
                            MPNumber = t.MPNumber,
                            MPSize = t.MPSize,
                            Postcode = t.Postcode,
                            MPCreateTime = t.CreateTime
                        }).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        /// <summary>
        /// 将选中的记录保存到门牌制作数据库，并分页返回选中的记录
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="mps"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SaveAndGetProduceMPList(int PageSize, int PageNum, List<MPProduceList> mps)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var mpPro = (from t in mps
                             select new Models.Entities.MPProduce
                             {
                                 ID = Guid.NewGuid().ToString(),
                                 MPType = t.MPType,
                                 MPID = t.MPID,
                                 CreateTime = DateTime.Now.Date,
                                 CreateUser = LoginUtils.CurrentUser.UserID
                             }).ToList();
                dbContext.MPProduce.AddRange(mpPro);
                dbContext.SaveChanges();
            }
            var rt = mps.OrderBy(t => t.MPCreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
            return new Dictionary<string, object> {
                   { "Data",rt},
                   { "Count",mps.Count}
                };
        }
    }
}