﻿using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class StatisticUtils
    {
        /// <summary>
        /// 分页返回门牌的地名证明和门牌证打印两个业务数据，根据当前用户的行政区划ID去判断是否有权限查看
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetMPBusinessDatas(int PageSize, int PageNum, string start, string end, string DistrictID, string Window, string CreateUser, int CertificateType)
        {
            int count = 0;
            List<MPBusiness> query = new List<Models.Extends.MPBusiness>();
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                #region 住宅类
                var queryResidence = from t in dbContext.MPOfCertificate

                                     join r in dbContext.MPOFResidence
                                     on t.MPID equals r.ID into rr
                                     from rt in rr.DefaultIfEmpty()

                                     where t.MPType == Enums.MPType.Residence
                                     select new MPBusiness
                                     {
                                         ID = t.ID,
                                         MPID = t.MPID,
                                         MPType = t.MPType,
                                         MPTypeName = "住宅门牌",
                                         CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地名证明" : "门牌证打印",
                                         CreateTime = t.CreateTime,
                                         CreateUser = t.CreateUser,
                                         Window = t.Window,
                                         CertificateType = t.CertificateType,
                                         CountyID = rt.CountyID,
                                         NeighborhoodsID = rt.NeighborhoodsID,
                                         CommunityID = rt.CommunityID,
                                         StandardAddress = rt.StandardAddress,
                                         MPBZTime = rt.BZTime
                                     };
                #endregion
                #region 道路类
                var queryRoad = from t in dbContext.MPOfCertificate

                                join r in dbContext.MPOfRoad
                                on t.MPID equals r.ID into rr
                                from rt in rr.DefaultIfEmpty()

                                where t.MPType == Enums.MPType.Road
                                select new MPBusiness
                                {
                                    ID = t.ID,
                                    MPID = t.MPID,
                                    MPType = t.MPType,
                                    MPTypeName = "道路门牌",
                                    CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地名证明" : "门牌证打印",
                                    CreateTime = t.CreateTime,
                                    CreateUser = t.CreateUser,
                                    Window = t.Window,
                                    CertificateType = t.CertificateType,
                                    CountyID = rt.CountyID,
                                    NeighborhoodsID = rt.NeighborhoodsID,
                                    CommunityID = rt.CommunityID,
                                    StandardAddress = rt.StandardAddress,
                                    MPBZTime = rt.BZTime
                                };
                #endregion
                #region 农村类
                var queryCountry = from t in dbContext.MPOfCertificate

                                   join r in dbContext.MPOfCountry
                                   on t.MPID equals r.ID into rr
                                   from rt in rr.DefaultIfEmpty()

                                   where t.MPType == Enums.MPType.Country
                                   select new MPBusiness
                                   {
                                       ID = t.ID,
                                       MPID = t.MPID,
                                       MPType = t.MPType,
                                       MPTypeName = "农村门牌",
                                       CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地名证明" : "门牌证打印",
                                       CreateTime = t.CreateTime,
                                       CreateUser = t.CreateUser,
                                       Window = t.Window,
                                       CertificateType = t.CertificateType,
                                       CountyID = rt.CountyID,
                                       NeighborhoodsID = rt.NeighborhoodsID,
                                       CommunityID = rt.CommunityID,
                                       StandardAddress = rt.StandardAddress,
                                       MPBZTime = rt.BZTime
                                   };
                #endregion
                var All = queryResidence.Concat(queryRoad).Concat(queryCountry);
                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPBusiness>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                }
                var queryAll = All.Where(where.Compile());


                if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                {
                    if (!string.IsNullOrEmpty(start))
                    {
                        queryAll = queryAll.Where(t => String.Compare(t.CreateTime.ToString(), start, StringComparison.Ordinal) >= 0);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        queryAll = queryAll.Where(t => String.Compare(t.CreateTime.ToString(), end, StringComparison.Ordinal) <= 0);
                    }
                }
                if (!string.IsNullOrEmpty(DistrictID))
                {
                    queryAll = queryAll.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                }
                if (!string.IsNullOrEmpty(Window))
                {
                    queryAll = queryAll.Where(t => t.Window.Contains(Window));
                }
                if (!string.IsNullOrEmpty(CreateUser))
                {
                    queryAll = queryAll.Where(t => t.CreateUser == CreateUser);
                }
                if (CertificateType != Enums.CertificateType.All)
                    queryAll = queryAll.Where(t => t.CertificateType == CertificateType);

                count = queryAll.Count();
                query = queryAll.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = (from t in query
                            join a in SystemUtils.Districts
                            on t.CountyID equals a.ID into aa
                            from at in aa.DefaultIfEmpty()

                            join b in SystemUtils.Districts
                            on t.NeighborhoodsID equals b.ID into bb
                            from bt in bb.DefaultIfEmpty()

                            join c in SystemUtils.Districts
                            on t.CommunityID equals c.ID into cc
                            from ct in cc.DefaultIfEmpty()

                            select new MPBusiness
                            {
                                ID = t.ID,
                                MPID = t.MPID,
                                MPType = t.MPType,
                                MPTypeName = t.MPTypeName,
                                CertificateTypeName = t.CertificateTypeName,
                                CreateTime = t.CreateTime,
                                CreateUser = t.CreateUser,
                                Window = t.Window,
                                CertificateType = t.CertificateType,
                                CountyID = t.CountyID,
                                NeighborhoodsID = t.NeighborhoodsID,
                                CommunityID = t.CommunityID,
                                CountyName = at == null || at.Name == null ? null : at.Name,
                                NeighborhoodsName = bt == null || bt.Name == null ? null : bt.Name,
                                CommunityName = ct == null || ct.Name == null ? null : ct.Name,
                                StandardAddress = t.StandardAddress,
                                MPBZTime = t.MPBZTime
                            }).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }

        /// <summary>
        /// 获取各类门牌制作数量的统计数据
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetMPProduceStatistic(int PageSize, int PageNum, string DistrictID, string start, string end)
        {
            int count = 0;
            List<Models.Entities.MPProduce> query = new List<Models.Entities.MPProduce>();

            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                #region 注释原
                //var bigMPsize = dbContext.DMBZDic.Where(t => t.Type == "大门牌").Select(t => t.Size).ToList();
                //var smallMPsize = dbContext.DMBZDic.Where(t => t.Type == "小门牌").Select(t => t.Size).ToList();
                //var all = (from t in dbContext.MPProduce

                //           join f in dbContext.MPOfRoad
                //           on t.MPID equals f.ID into ff
                //           from ft in ff.DefaultIfEmpty()
                //           where t.MPType == Enums.MPType.Road
                //           select new MPProduceStatistic
                //           {
                //               CountyID = ft.CountyID,
                //               NeighborhoodsID = ft.NeighborhoodsID,
                //               CommunityID = ft.CommunityID,
                //               MPType = bigMPsize.Contains(ft.MPSize) ? "大门牌" : "小门牌",
                //               MPProduceTime = t.CreateTime
                //           }).Concat(
                //            from t in dbContext.MPProduce

                //            join f in dbContext.MPOfCountry
                //            on t.MPID equals f.ID into ff
                //            from ft in ff.DefaultIfEmpty()
                //            where t.MPType == Enums.MPType.Country
                //            select new MPProduceStatistic
                //            {
                //                CountyID = ft.CountyID,
                //                NeighborhoodsID = ft.NeighborhoodsID,
                //                CommunityID = ft.CommunityID,
                //                MPType = "农村门牌",
                //                MPProduceTime = t.CreateTime
                //            });
                //// 先删选出当前用户权限内的数据
                //var where = PredicateBuilder.False<MPProduceStatistic>();
                //foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                //{
                //    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                //}
                //var q = all.Where(where.Compile());

                //if (!string.IsNullOrEmpty(DistrictID))
                //{
                //    q = q.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                //}

                //if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                //{
                //    if (!string.IsNullOrEmpty(start))
                //    {
                //        q = q.Where(t => String.Compare(t.MPProduceTime.ToString(), start, StringComparison.Ordinal) >= 0);
                //    }
                //    if (!string.IsNullOrEmpty(end))
                //    {
                //        q = q.Where(t => String.Compare(t.MPProduceTime.ToString(), end, StringComparison.Ordinal) <= 0);
                //    }
                //}

                //var mps = from t in q.ToList()
                //          join a in SystemUtils.Districts
                //          on t.CountyID equals a.ID into aa
                //          from at in aa.DefaultIfEmpty()

                //          join b in SystemUtils.Districts
                //          on t.NeighborhoodsID equals b.ID into bb
                //          from bt in bb.DefaultIfEmpty()

                //          join c in SystemUtils.Districts
                //          on t.CommunityID equals c.ID into cc
                //          from ct in cc.DefaultIfEmpty()
                //          select new MPProduceStatistic
                //          {
                //              CountyID = t.CountyID,
                //              NeighborhoodsID = t.NeighborhoodsID,
                //              CommunityID = t.CommunityID,
                //              CountyName = at == null || at.Name == null ? null : at.Name,
                //              NeighborhoodsName = bt == null || bt.Name == null ? null : bt.Name,
                //              CommunityName = ct == null || ct.Name == null ? null : ct.Name,
                //              //MPSizeType = t.MPSizeType,
                //              MPType = t.MPType
                //          };

                ////var MPSizeCount = (from t in mps
                ////                   group t by new
                ////                   {
                ////                       t.CountyName,
                ////                       t.NeighborhoodsName,
                ////                       t.CommunityName,
                ////                       t.MPSizeType
                ////                   }
                ////                  into g
                ////                   select new Statistic
                ////                   {
                ////                       CountyName = g.Key.CountyName,
                ////                       NeighborhoodsName = g.Key.NeighborhoodsName,
                ////                       CommunityName = g.Key.CommunityName,
                ////                       type = g.Key.MPSizeType,
                ////                       Count = g.Count()
                ////                   }).ToList();
                //var MPTypeCount = (from t in mps
                //                   group t by new
                //                   {
                //                       t.CountyName,
                //                       t.NeighborhoodsName,
                //                       t.CommunityName,
                //                       t.MPType
                //                   }
                //                  into g
                //                   select new Statistic
                //                   {
                //                       CountyName = g.Key.CountyName,
                //                       NeighborhoodsName = g.Key.NeighborhoodsName,
                //                       CommunityName = g.Key.CommunityName,
                //                       type = g.Key.MPType,
                //                       Count = g.Count()
                //                   }).ToList();
                //var MPTotalCount = (from t in mps
                //                    group t by new
                //                    {
                //                        t.CountyName,
                //                        t.NeighborhoodsName,
                //                        t.CommunityName,
                //                    }
                //                   into g
                //                    select new Statistic
                //                    {
                //                        CountyName = g.Key.CountyName,
                //                        NeighborhoodsName = g.Key.NeighborhoodsName,
                //                        CommunityName = g.Key.CommunityName,
                //                        type = "总数",
                //                        Count = g.Count()
                //                    }).ToList();
                //foreach (var data in MPTotalCount)
                //{
                //    MPProduceStatistic mp = new MPProduceStatistic();
                //    mp.CountyName = data.CountyName;
                //    mp.NeighborhoodsName = data.NeighborhoodsName;
                //    mp.CommunityName = data.CommunityName;
                //    mp.TotalMPCount = data.Count;
                //    mp.BigMPCount = MPSizeCount.Where(t => t.CountyName == data.CountyName).Where(t => t.NeighborhoodsName == data.NeighborhoodsName).Where(t => t.CommunityName == data.CommunityName).Where(t => t.type == "大门牌").Select(t => t.Count).FirstOrDefault();
                //    mp.SmallMPCount = MPSizeCount.Where(t => t.CountyName == data.CountyName).Where(t => t.NeighborhoodsName == data.NeighborhoodsName).Where(t => t.CommunityName == data.CommunityName).Where(t => t.type == "小门牌").Select(t => t.Count).FirstOrDefault();
                //    mp.RoadMPCount = MPTypeCount.Where(t => t.CountyName == data.CountyName).Where(t => t.NeighborhoodsName == data.NeighborhoodsName).Where(t => t.CommunityName == data.CommunityName).Where(t => t.type == "道路门牌").Select(t => t.Count).FirstOrDefault();
                //    mp.CountryMPCount = MPTypeCount.Where(t => t.CountyName == data.CountyName).Where(t => t.NeighborhoodsName == data.NeighborhoodsName).Where(t => t.CommunityName == data.CommunityName).Where(t => t.type == "农村门牌").Select(t => t.Count).FirstOrDefault();
                //    query.Add(mp);
                //}
                //count = query.Count();
                //var rt = query.OrderByDescending(t => t.TotalMPCount).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                #endregion

                //查询出用户权限内的数据
                var where = PredicateBuilder.False<Models.Entities.MPProduce>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.CommunityID.IndexOf(userDID + ".") == 0);
                }
                var q = dbContext.MPProduce.Where(where.Compile());
                //门牌制作时间筛选
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
                //行政区划ID筛选
                if (!string.IsNullOrEmpty(DistrictID))
                {
                    q = q.Where(t => t.CommunityID.IndexOf(DistrictID + ".") == 0);
                }
                count = q.Count();
                query = q.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",query},
                   { "Count",count}
                };
            }
        }
    }
    public class Statistic
    {
        public string CountyName { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public int Count { get; set; }
        public string type { get; set; }
    }
}