using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
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
        /// 获取已经制作的零星门牌
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="MPProduceComplete"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetProducedLXMP(int PageSize, int PageNum)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var mpOfRoad = dbContext.MPOfRoad.Where(t => true);
                var mpOfCountry = dbContext.MPOfCountry.Where(t => true);
                // 先删选出当前用户权限内的数据
                if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                {
                    var where = PredicateBuilder.False<MPOfRoad>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfRoad = mpOfRoad.Where(where);

                    var where2 = PredicateBuilder.False<MPOfCountry>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where2 = where2.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfCountry = mpOfCountry.Where(where2);
                }

                int count = 0;

                var all = (from t in mpOfRoad
                           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && t.LXProduceID != null
                           group t by new { t.LXProduceID, t.MPProduceUser, t.MPProduceTime } into g
                           select new ProducedLXMPList
                           {
                               MPType = Enums.MPTypeCh.Road,
                               LXProduceID = g.Key.LXProduceID,
                               MPProduceUser = g.Key.MPProduceUser,
                               MPProduceTime = g.Key.MPProduceTime
                           }).Concat(
                    from t in mpOfCountry
                    where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && t.LXProduceID != null
                    group t by new { t.LXProduceID, t.MPProduceUser, t.MPProduceTime } into g
                    select new ProducedLXMPList
                    {
                        MPType = Enums.MPTypeCh.Country,
                        LXProduceID = g.Key.LXProduceID,
                        MPProduceUser = g.Key.MPProduceUser,
                        MPProduceTime = g.Key.MPProduceTime
                    });

                count = all.Count();
                var data = all.OrderBy(t => t.LXProduceID).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        /// <summary>
        /// 获取未制作的零星门牌
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetNotProducedLXMP(int PageSize, int PageNum)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                var roadMPProduce = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => string.IsNullOrEmpty(t.LXProduceID));
                var countryMPProduce = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => string.IsNullOrEmpty(t.LXProduceID));
                var all = (from a in roadMPProduce
                           select new NotProducedLXMPList
                           {
                               MPID = a.ID,
                               CountyID = a.CountyID,
                               //CountyName = !string.IsNullOrEmpty(a.CountyID) ? a.CountyID.Split('.').Last() : null,
                               NeighborhoodsID = a.NeighborhoodsID,
                               //NeighborhoodsName = !string.IsNullOrEmpty(a.NeighborhoodsID) ? a.NeighborhoodsID.Split('.').Last() : null,
                               CommunityName = a.CommunityName,
                               MPType = Enums.TypeInt.Road,
                               MPTypeName = Enums.MPTypeCh.Road,
                               PlaceName = a.RoadName,
                               MPNumber = a.MPNumber,
                               MPSize = a.MPSize,
                               Postcode = a.Postcode,
                               MPBZTime = a.BZTime
                           }).Concat(
                            from b in countryMPProduce
                            select new NotProducedLXMPList
                            {
                                MPID = b.ID,
                                CountyID = b.CountyID,
                                //CountyName = !string.IsNullOrEmpty(b.CountyID) ? b.CountyID.Split('.').Last() : null,
                                NeighborhoodsID = b.NeighborhoodsID,
                                //NeighborhoodsName = !string.IsNullOrEmpty(b.NeighborhoodsID) ? b.NeighborhoodsID.Split('.').Last() : null,
                                CommunityName = b.CommunityName,
                                MPType = Enums.TypeInt.Country,
                                MPTypeName = Enums.MPTypeCh.Country,
                                PlaceName = b.ViligeName,
                                MPNumber = b.MPNumber,
                                MPSize = b.MPSize,
                                Postcode = b.Postcode,
                                MPBZTime = b.BZTime
                            });


                // 先删选出当前用户权限内的数据
                if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                {
                    var where = PredicateBuilder.False<NotProducedLXMPList>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    all = all.Where(where);
                }

                count = all.Count();
                var data = all.OrderByDescending(t => t.MPBZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                data = (from t in data
                        select new NotProducedLXMPList
                        {
                            MPID = t.MPID,
                            CountyID = t.CountyID,
                            CountyName = !string.IsNullOrEmpty(t.CountyID) ? t.CountyID.Split('.').Last() : null,
                            NeighborhoodsID = t.NeighborhoodsID,
                            NeighborhoodsName = !string.IsNullOrEmpty(t.NeighborhoodsID) ? t.NeighborhoodsID.Split('.').Last() : null,
                            CommunityName = t.CommunityName,
                            MPType = Enums.TypeInt.Country,
                            MPTypeName = Enums.MPTypeCh.Country,
                            PlaceName = t.PlaceName,
                            MPNumber = t.MPNumber,
                            MPSize = t.MPSize,
                            Postcode = t.Postcode,
                            MPBZTime = t.MPBZTime
                        }).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }

        /// <summary>
        /// 批量选择零星增加的需制作但未制作的门牌，进行批量制作
        /// </summary>
        /// <param name="mpLists"></param>
        public static List<LXMPHZ> ProduceLXMP(List<NotProducedLXMPList> mpLists)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<LXMPHZ> lxmphzs = new List<LXMPHZ>();
                var LXProduceID = DateTime.Now.Date.ToString("yyyyMMddhhmmss");
                foreach (var mp in mpLists)
                {
                    LXMPHZ lxmphz = new LXMPHZ();
                    if (mp.MPType == Enums.TypeInt.Road)
                    {
                        var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.ID == mp.MPID).FirstOrDefault();
                        if (query == null)
                            throw new Exception($"ID为{mp.MPID}门牌已被注销");
                        query.LXProduceID = LXProduceID;
                        query.MPProduceUser = LoginUtils.CurrentUser.UserName;
                        query.MPProduceTime = DateTime.Now.Date;

                        lxmphz.PlaceName = query.RoadName;
                        lxmphz.MPType = Enums.MPTypeCh.Road;
                        lxmphz.MPNumber = query.MPNumber;
                        lxmphz.MPSize = query.MPSize;
                        lxmphz.Count = 1;
                    }
                    else if (mp.MPType == Enums.TypeInt.Country)
                    {
                        var query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.ID == mp.MPID).FirstOrDefault();
                        if (query == null)
                            throw new Exception($"ID为{mp.MPID}门牌已被注销");
                        query.LXProduceID = LXProduceID;
                        query.MPProduceUser = LoginUtils.CurrentUser.UserName;
                        query.MPProduceTime = DateTime.Now.Date;

                        lxmphz.PlaceName = query.ViligeName;
                        lxmphz.MPType = Enums.MPTypeCh.Country;
                        lxmphz.MPNumber = query.MPNumber;
                        lxmphz.MPSize = query.MPSize;
                        lxmphz.Count = 1;
                    }
                    lxmphzs.Add(lxmphz);
                    dbContext.SaveChanges();
                }
                return lxmphzs;
            }
        }

        public static List<LXMPHZ> GetProducedLXMPDetails(ProducedLXMPList producedLXMPList)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<LXMPHZ> data = new List<LXMPHZ>();
                if (producedLXMPList.MPType == Enums.MPTypeCh.Road)
                {
                    data = (from t in dbContext.MPOfRoad
                            where t.LXProduceID == producedLXMPList.LXProduceID
                            group t by new { t.RoadName, t.MPNumber, t.MPSize } into g
                            select new LXMPHZ
                            {
                                PlaceName = g.Key.RoadName,
                                MPNumber = g.Key.MPNumber,
                                MPSize = g.Key.MPSize,
                                MPType = producedLXMPList.MPType,
                                Count = 1
                            }).ToList();
                }
                else if (producedLXMPList.MPType == Enums.MPTypeCh.Country)
                {
                    data = (from t in dbContext.MPOfCountry
                            where t.LXProduceID == producedLXMPList.LXProduceID
                            group t by new { t.ViligeName, t.MPNumber, t.MPSize } into g
                            select new LXMPHZ
                            {
                                PlaceName = g.Key.ViligeName,
                                MPNumber = g.Key.MPNumber,
                                MPSize = g.Key.MPSize,
                                MPType = producedLXMPList.MPType,
                                Count = 1
                            }).ToList();
                }
                return data;
            }
        }
        /// <summary>
        /// 获取批量导入的已制作或未制作的门牌，根据申报单位、标准名、申办人、联系电话、编制日期和批量导入的ID进行分组，统计数量
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="PLMPProduceComplete"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetProducedPLMP(int PageSize, int PageNum)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var mpOfRoad = dbContext.MPOfRoad.Where(t => true);
                var mpOfResidence = dbContext.MPOfResidence.Where(t => true);
                var mpOfCountry = dbContext.MPOfCountry.Where(t => true);

                if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<MPOfRoad>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfRoad = mpOfRoad.Where(where);

                    // 先删选出当前用户权限内的数据
                    var where2 = PredicateBuilder.False<MPOfCountry>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where2 = where2.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfCountry = mpOfCountry.Where(where2);

                    // 先删选出当前用户权限内的数据
                    var where3 = PredicateBuilder.False<MPOfResidence>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where3 = where3.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfResidence = mpOfResidence.Where(where3);
                }

                int count = 0;

                var lz = (from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                          select new
                          {
                              PLProduceID = t.PLProduceID,
                              SBDW = t.SBDW,
                              ResidenceName = t.ResidenceName,
                              LZNumber = t.LZNumber,
                              Applicant = t.Applicant,
                              ApplicantPhone = t.ApplicantPhone,
                              MPBZTime = t.BZTime
                          }).Distinct();
                var lzC = from t in lz
                          group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                          select new ProducedPLMPList
                          {
                              PLProduceID = g.Key.PLProduceID,
                              SBDW = g.Key.SBDW,
                              ResidenceName = g.Key.ResidenceName,
                              MPCount = g.Count() * 2,
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              MPBZTime = g.Key.MPBZTime
                          };
                var dy = (from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                          select new
                          {
                              PLProduceID = t.PLProduceID,
                              SBDW = t.SBDW,
                              ResidenceName = t.ResidenceName,
                              LZNumber = t.LZNumber,
                              DYNumber = t.DYNumber,
                              Applicant = t.Applicant,
                              ApplicantPhone = t.ApplicantPhone,
                              MPBZTime = t.BZTime
                          }).Distinct();
                var dyC = from t in dy
                          group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                          select new ProducedPLMPList
                          {
                              PLProduceID = g.Key.PLProduceID,
                              SBDW = g.Key.SBDW,
                              ResidenceName = g.Key.ResidenceName,
                              MPCount = g.Count(),
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              MPBZTime = g.Key.MPBZTime
                          };
                var hsC = from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                          group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                          select new ProducedPLMPList
                          {
                              PLProduceID = g.Key.PLProduceID,
                              SBDW = g.Key.SBDW,
                              ResidenceName = g.Key.ResidenceName,
                              MPCount = g.Count(),
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              MPBZTime = g.Key.BZTime
                          };

                var xq = (from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                          select new
                          {
                              PLProduceID = t.PLProduceID,
                              SBDW = t.SBDW,
                              ResidenceName = t.ResidenceName,
                              Applicant = t.Applicant,
                              ApplicantPhone = t.ApplicantPhone,
                              MPBZTime = t.BZTime
                          }).Distinct();
                var zz = from t in xq
                         join a in lzC on t.PLProduceID equals a.PLProduceID
                         join b in dyC on t.PLProduceID equals b.PLProduceID
                         join c in hsC on t.PLProduceID equals c.PLProduceID
                         select new ProducedPLMPList
                         {
                             PLProduceID = t.PLProduceID,
                             MPType = Enums.MPTypeCh.Residence,
                             SBDW = t.SBDW,
                             ResidenceName = t.ResidenceName,
                             MPCount = a.MPCount + b.MPCount + c.MPCount,
                             Applicant = t.Applicant,
                             ApplicantPhone = t.ApplicantPhone,
                             MPBZTime = t.MPBZTime
                         };

                var dl = from t in mpOfRoad
                         where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                         group t by new { t.PLProduceID, t.SBDW, t.RoadName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                         select new ProducedPLMPList
                         {
                             PLProduceID = g.Key.PLProduceID,
                             SBDW = g.Key.SBDW,
                             RoadName = g.Key.RoadName,
                             MPCount = g.Count(),
                             Applicant = g.Key.Applicant,
                             ApplicantPhone = g.Key.ApplicantPhone,
                             MPBZTime = g.Key.BZTime
                         };
                var nc = from t in mpOfCountry
                         where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                         group t by new { t.PLProduceID, t.SBDW, t.ViligeName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                         select new ProducedPLMPList
                         {
                             PLProduceID = g.Key.PLProduceID,
                             SBDW = g.Key.SBDW,
                             ViligeName = g.Key.ViligeName,
                             MPCount = g.Count(),
                             Applicant = g.Key.Applicant,
                             ApplicantPhone = g.Key.ApplicantPhone,
                             MPBZTime = g.Key.BZTime
                         };
                var all = zz.Concat(dl).Concat(nc);
                count = all.Count();
                var result = all.OrderByDescending(t => t.MPBZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",result},
                   { "Count",count}
                };
            }
        }
        public static Dictionary<string, object> GetNotProducedPLMP(int PageSize, int PageNum)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var mpOfRoad = dbContext.MPOfRoad.Where(t => true);
                var mpOfResidence = dbContext.MPOfResidence.Where(t => true);
                var mpOfCountry = dbContext.MPOfCountry.Where(t => true);

                if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<MPOfRoad>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfRoad = mpOfRoad.Where(where);

                    // 先删选出当前用户权限内的数据
                    var where2 = PredicateBuilder.False<MPOfCountry>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where2 = where2.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfCountry = mpOfCountry.Where(where2);

                    // 先删选出当前用户权限内的数据
                    var where3 = PredicateBuilder.False<MPOfResidence>();
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where3 = where3.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    mpOfResidence = mpOfResidence.Where(where3);
                }

                int count = 0;

                var lz = (from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.PLProduceID == null
                          select new
                          {
                              PLID = t.PLID,
                              SBDW = t.SBDW,
                              ResidenceName = t.ResidenceName,
                              LZNumber = t.LZNumber,
                              Applicant = t.Applicant,
                              ApplicantPhone = t.ApplicantPhone,
                              MPBZTime = t.BZTime
                          }).Distinct();
                var lzC = from t in lz
                          group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                          select new NotProducedPLMPList
                          {
                              PLID = g.Key.PLID,
                              SBDW = g.Key.SBDW,
                              ResidenceName = g.Key.ResidenceName,
                              MPCount = g.Count() * 2,
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              MPBZTime = g.Key.MPBZTime
                          };
                var dy = (from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.PLProduceID == null
                          select new
                          {
                              PLID = t.PLID,
                              SBDW = t.SBDW,
                              ResidenceName = t.ResidenceName,
                              LZNumber = t.LZNumber,
                              DYNumber = t.DYNumber,
                              Applicant = t.Applicant,
                              ApplicantPhone = t.ApplicantPhone,
                              MPBZTime = t.BZTime
                          }).Distinct();
                var dyC = from t in dy
                          group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                          select new NotProducedPLMPList
                          {
                              PLID = g.Key.PLID,
                              SBDW = g.Key.SBDW,
                              ResidenceName = g.Key.ResidenceName,
                              MPCount = g.Count(),
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              MPBZTime = g.Key.MPBZTime
                          };
                var hsC = from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.PLProduceID == null
                          group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                          select new NotProducedPLMPList
                          {
                              PLID = g.Key.PLID,
                              SBDW = g.Key.SBDW,
                              ResidenceName = g.Key.ResidenceName,
                              MPCount = g.Count(),
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              MPBZTime = g.Key.BZTime
                          };
                var xq = (from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.PLProduceID == null
                          select new
                          {
                              PLID = t.PLID,
                              SBDW = t.SBDW,
                              ResidenceName = t.ResidenceName,
                              Applicant = t.Applicant,
                              ApplicantPhone = t.ApplicantPhone,
                              MPBZTime = t.BZTime
                          }).Distinct();
                var zz = from t in xq
                         join a in lzC on t.PLID equals a.PLID
                         join b in dyC on t.PLID equals b.PLID
                         join c in hsC on t.PLID equals c.PLID
                         select new NotProducedPLMPList
                         {
                             PLID = t.PLID,
                             MPType = Enums.MPTypeCh.Residence,
                             SBDW = t.SBDW,
                             ResidenceName = t.ResidenceName,
                             MPCount = a.MPCount + b.MPCount + c.MPCount,
                             Applicant = t.Applicant,
                             ApplicantPhone = t.ApplicantPhone,
                             MPBZTime = t.MPBZTime
                         };

                var dl = from t in mpOfRoad
                         where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.PLProduceID == null
                         group t by new { t.PLID, t.SBDW, t.RoadName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                         select new NotProducedPLMPList
                         {
                             PLID = g.Key.PLID,
                             MPType = Enums.MPTypeCh.Road,
                             SBDW = g.Key.SBDW,
                             RoadName = g.Key.RoadName,
                             MPCount = g.Count(),
                             Applicant = g.Key.Applicant,
                             ApplicantPhone = g.Key.ApplicantPhone,
                             MPBZTime = g.Key.BZTime
                         };
                var nc = from t in mpOfCountry
                         where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.PLProduceID == null
                         group t by new { t.PLID, t.SBDW, t.ViligeName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                         select new NotProducedPLMPList
                         {
                             PLID = g.Key.PLID,
                             MPType = Enums.MPTypeCh.Country,
                             SBDW = g.Key.SBDW,
                             ViligeName = g.Key.ViligeName,
                             MPCount = g.Count(),
                             Applicant = g.Key.Applicant,
                             ApplicantPhone = g.Key.ApplicantPhone,
                             MPBZTime = g.Key.BZTime
                         };
                var all = zz.Concat(dl).Concat(nc);
                count = all.Count();
                var result = all.OrderByDescending(t => t.MPBZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",result},
                   { "Count",count}
                };
            }
        }

        public static List<PLMPHZ> ProducePLMP(List<NotProducedPLMPList> mpLists)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<PLMPHZ> plmphzs = new List<PLMPHZ>();
                var PLProduceID = DateTime.Now.Date.ToString("yyyyMMddhhmmss");
                var MPProduceTime = DateTime.Now.Date;
                foreach (var mp in mpLists)
                {
                    if (mp.MPType == Enums.MPTypeCh.Residence)
                    {
                        var query = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => string.IsNullOrEmpty(t.PLProduceID)).Where(t => t.PLID == mp.PLID).ToList();
                        foreach (var q in query)
                        {
                            q.PLProduceID = PLProduceID;
                            q.MPProduceUser = LoginUtils.CurrentUser.UserName;
                            q.MPProduceTime = MPProduceTime;
                        }
                        var PlaceNames = query.Select(t => t.ResidenceName).Distinct().ToList();
                        foreach (var PlaceName in PlaceNames)
                        {
                            var plmphz = new PLMPHZ();
                            plmphz.PlaceName = PlaceName;

                            plmphz.LZP = (from t in query
                                          where t.ResidenceName == PlaceName
                                          group t by t.LZNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = 2
                                          }).ToList();
                            var dy = (from t in query
                                      where t.ResidenceName == PlaceName
                                      select new
                                      {
                                          LZNumber = t.LZNumber,
                                          DYNumner = t.DYNumber
                                      }).Distinct();
                            plmphz.DYP = (from t in dy
                                          group t by t.DYNumner into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = g.Count()
                                          }).ToList();
                            plmphz.HSP = (from t in query
                                          where t.ResidenceName == PlaceName
                                          group t by t.HSNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = g.Count()
                                          }).ToList();
                            plmphzs.Add(plmphz);
                        }
                    }
                    else if (mp.MPType == Enums.MPTypeCh.Road)
                    {
                        var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => string.IsNullOrEmpty(t.PLProduceID)).Where(t => t.PLID == mp.PLID).ToList();
                        foreach (var q in query)
                        {
                            q.PLProduceID = PLProduceID;
                            q.MPProduceUser = LoginUtils.CurrentUser.UserName;
                            q.MPProduceTime = MPProduceTime;
                        }
                        var PlaceNames = query.Select(t => t.RoadName).Distinct().ToList();
                        foreach (var PlaceName in PlaceNames)
                        {
                            var plmphz = new PLMPHZ();
                            plmphz.PlaceName = PlaceName;

                            plmphz.DLP = (from t in query
                                          where t.RoadName == PlaceName
                                          group t by t.MPNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              MPSize = g.Select(t => t.MPSize).First(),
                                              Count = g.Count()
                                          }).ToList();

                            plmphzs.Add(plmphz);
                        }
                    }
                    else if (mp.MPType == Enums.MPTypeCh.Country)
                    {
                        var query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => string.IsNullOrEmpty(t.PLProduceID)).Where(t => t.PLID == mp.PLID).ToList();
                        foreach (var q in query)
                        {
                            q.PLProduceID = PLProduceID;
                            q.MPProduceUser = LoginUtils.CurrentUser.UserName;
                            q.MPProduceTime = MPProduceTime;
                        }
                        var PlaceNames = query.Select(t => t.ViligeName).Distinct().ToList();
                        foreach (var PlaceName in PlaceNames)
                        {
                            var plmphz = new PLMPHZ();
                            plmphz.PlaceName = PlaceName;

                            plmphz.NCP = (from t in query
                                          where t.ViligeName == PlaceName
                                          group t by t.MPNumber into g
                                          select new PLMPSL
                                          {
                                              Number = g.Key,
                                              Count = g.Count()
                                          }).ToList();
                            plmphzs.Add(plmphz);
                        }
                    }
                    dbContext.SaveChanges();
                }
                return plmphzs;
            }
        }

        public static List<PLMPHZ> GetProducedPLMPDetails(ProducedPLMPList producedPLMPList)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<PLMPHZ> plmphzs = new List<PLMPHZ>();
                if (producedPLMPList.MPType == Enums.MPTypeCh.Residence)
                {
                    var query = dbContext.MPOfResidence.Where(t => t.PLProduceID == producedPLMPList.PLProduceID);
                    var PlaceNames = query.Select(t => t.ResidenceName).Distinct().ToList();
                    foreach (var PlaceName in PlaceNames)
                    {
                        var plmphz = new PLMPHZ();
                        plmphz.PlaceName = PlaceName;

                        plmphz.LZP = (from t in query
                                      where t.ResidenceName == PlaceName
                                      group t by t.LZNumber into g
                                      select new PLMPSL
                                      {
                                          Number = g.Key,
                                          Count = 2
                                      }).ToList();
                        var dy = (from t in query
                                  where t.ResidenceName == PlaceName
                                  select new
                                  {
                                      LZNumber = t.LZNumber,
                                      DYNumner = t.DYNumber
                                  }).Distinct();
                        plmphz.DYP = (from t in dy
                                      group t by t.DYNumner into g
                                      select new PLMPSL
                                      {
                                          Number = g.Key,
                                          Count = g.Count()
                                      }).ToList();
                        plmphz.HSP = (from t in query
                                      where t.ResidenceName == PlaceName
                                      group t by t.HSNumber into g
                                      select new PLMPSL
                                      {
                                          Number = g.Key,
                                          Count = g.Count()
                                      }).ToList();
                        plmphzs.Add(plmphz);
                    }
                }
                else if (producedPLMPList.MPType == Enums.MPTypeCh.Road)
                {
                    var query = dbContext.MPOfRoad.Where(t => t.PLProduceID == producedPLMPList.PLProduceID);
                    var PlaceNames = query.Select(t => t.RoadName).Distinct().ToList();
                    foreach (var PlaceName in PlaceNames)
                    {
                        var plmphz = new PLMPHZ();
                        plmphz.PlaceName = PlaceName;

                        plmphz.DLP = (from t in query
                                      where t.RoadName == PlaceName
                                      group t by t.MPNumber into g
                                      select new PLMPSL
                                      {
                                          Number = g.Key,
                                          MPSize = g.Select(t => t.MPSize).First(),
                                          Count = g.Count()
                                      }).ToList();

                        plmphzs.Add(plmphz);
                    }
                }
                else if (producedPLMPList.MPType == Enums.MPTypeCh.Country)
                {
                    var query = dbContext.MPOfCountry.Where(t => t.PLProduceID == producedPLMPList.PLProduceID);
                    var PlaceNames = query.Select(t => t.ViligeName).Distinct().ToList();
                    foreach (var PlaceName in PlaceNames)
                    {
                        var plmphz = new PLMPHZ();
                        plmphz.PlaceName = PlaceName;

                        plmphz.NCP = (from t in query
                                      where t.ViligeName == PlaceName
                                      group t by t.MPNumber into g
                                      select new PLMPSL
                                      {
                                          Number = g.Key,
                                          Count = g.Count()
                                      }).ToList();
                        plmphzs.Add(plmphz);
                    }
                }
                return plmphzs;
            }
        }
    }
}
public class PLMPHZ
{
    public string PlaceName { get; set; }
    public List<PLMPSL> LZP { get; set; }
    public List<PLMPSL> DYP { get; set; }
    public List<PLMPSL> HSP { get; set; }
    public List<PLMPSL> DLP { get; set; }
    public List<PLMPSL> NCP { get; set; }
}
public class PLMPSL
{
    public string Number { get; set; }
    public string MPSize { get; set; }
    public int Count { get; set; }
}
public class LXMPHZ
{
    public string PlaceName { get; set; }
    public string MPSize { get; set; }
    public string MPType { get; set; }
    public string MPNumber { get; set; }
    public string Postcode { get; set; }
    public int Count { get; set; }
}