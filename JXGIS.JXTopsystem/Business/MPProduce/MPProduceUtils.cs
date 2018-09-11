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
        /// 获取已经制作或者未制作的零星门牌
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="MPProduceComplete"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetLXMPProduce(int PageSize, int PageNum, int LXMPProduceComplete)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                List<LXMPProduceList> data = null;
                var roadMPProduce = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.MPProduceComplete == LXMPProduceComplete).ToList();
                var countryMPProduce = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.MPProduceComplete == LXMPProduceComplete).ToList();
                var all = (from a in roadMPProduce
                           select new LXMPProduceList
                           {
                               MPID = a.ID,
                               CountyID = a.CountyID,
                               CountyName = a.CountyID.Split('.').Last(),
                               NeighborhoodsID = a.NeighborhoodsID,
                               NeighborhoodsName = a.NeighborhoodsID.Split('.').Last(),
                               CommunityName = a.CommunityName,
                               MPType = Enums.TypeInt.Road,
                               MPTypeName = "道路门牌",
                               LXMPProduceComplete = LXMPProduceComplete,
                               LXMPProduceCompleteName = LXMPProduceComplete == Enums.Complete.Yes ? "已制作" : "未制作",
                               PlaceName = a.RoadName,
                               MPNumber = a.MPNumber,
                               MPSize = a.MPSize,
                               Postcode = a.Postcode,
                               MPBZTime = a.BZTime
                           }).Concat(
                            from b in countryMPProduce
                            select new LXMPProduceList
                            {
                                MPID = b.ID,
                                CountyID = b.CountyID,
                                CountyName = b.CountyID.Split('.').Last(),
                                NeighborhoodsID = b.NeighborhoodsID,
                                NeighborhoodsName = b.NeighborhoodsID.Split('.').Last(),
                                CommunityName = b.CommunityName,
                                MPType = Enums.TypeInt.Country,
                                MPTypeName = "农村门牌",
                                LXMPProduceComplete = LXMPProduceComplete,
                                LXMPProduceCompleteName = LXMPProduceComplete == Enums.Complete.Yes ? "已制作" : "未制作",
                                PlaceName = b.ViligeName,
                                MPNumber = b.MPNumber,
                                MPSize = b.MPSize,
                                Postcode = b.Postcode,
                                MPBZTime = b.BZTime
                            });
                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<LXMPProduceList>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var q = all.Where(where.Compile());

                count = q.Count();
                data = q.OrderByDescending(t => t.MPBZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
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
        public static void ProduceLXMP(List<LXMPProduceList> mpLists)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<LXMPHZ> lxmphzs = new List<LXMPHZ>();
                foreach (var mp in mpLists)
                {
                    LXMPHZ lxmphz = new LXMPHZ();
                    if (mp.MPType == Enums.TypeInt.Road)
                    {
                        var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.ID == mp.MPID).First(); ;
                        if (query == null)
                            throw new Exception($"ID为{mp.MPID}门牌已被注销");
                        if (query.MPProduceComplete == Enums.Complete.NO)
                        {
                            query.MPProduceComplete = Enums.Complete.Yes;
                            query.MPProduceCompleteTime = DateTime.Now.Date;
                        }

                        lxmphz.PlaceName = query.RoadName;
                        lxmphz.MPType = "道路门牌";
                        lxmphz.MPNumber = query.MPNumber;
                        lxmphz.MPSize = query.MPSize;
                        lxmphz.Count = 1;
                    }
                    else if (mp.MPType == Enums.TypeInt.Country)
                    {
                        var query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.LX).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.ID == mp.MPID).First(); ;
                        if (query == null)
                            throw new Exception($"ID为{mp.MPID}门牌已被注销");
                        if (query.MPProduceComplete == Enums.Complete.NO)
                        {
                            query.MPProduceComplete = Enums.Complete.Yes;
                            query.MPProduceCompleteTime = DateTime.Now.Date;
                        }

                        lxmphz.PlaceName = query.ViligeName;
                        lxmphz.MPType = "农村门牌";
                        lxmphz.MPNumber = query.MPNumber;
                        lxmphz.MPSize = query.MPSize;
                        lxmphz.Count = 1;
                    }
                    lxmphzs.Add(lxmphz);
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 获取批量导入的已制作或未制作的门牌，根据申报单位、标准名、申办人、联系电话、编制日期和批量导入的ID进行分组，统计数量
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="PLMPProduceComplete"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetPLMPProduce(int PageSize, int PageNum, int PLMPProduceComplete)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                List<PLMPProduceList> datas = null;

                var ZZPLIDs = (from t in dbContext.MPOfResidence
                               where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.MPProduceComplete == PLMPProduceComplete
                               group t by t.PLID into g
                               select g.Key).ToList();

                foreach (var id in ZZPLIDs)
                {
                    var rt = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.MPProduceComplete == PLMPProduceComplete).Where(t => t.PLID == id);
                    var CountryID = rt.Select(t => t.CountyID).Distinct().First();
                    var NeighborhoodsID = rt.Select(t => t.NeighborhoodsID).Distinct().First();
                    var SBDW = rt.Select(t => t.SBDW).Distinct().ToList();
                    var ResidenceName = rt.Select(t => t.ResidenceName).Distinct().ToList();
                    var Applicant = rt.Select(t => t.Applicant).Distinct().ToList();
                    var ApplicantPhone = rt.Select(t => t.ApplicantPhone).Distinct().ToList();
                    var BZTime = rt.Select(t => t.BZTime.ToString()).Distinct().ToList();

                    var lzCount = (from t in rt
                                   group t by new
                                   {
                                       t.ResidenceName,
                                       t.LZNumber
                                   } into g
                                   select g.Key).Count() * 2;
                    var dyCount = (from t in rt
                                   group t by new
                                   {
                                       t.ResidenceName,
                                       t.LZNumber,
                                       t.DYNumber
                                   } into g
                                   select g.Key).Count();
                    var hsCount = (from t in rt
                                   group t by new
                                   {
                                       t.ResidenceName,
                                       t.LZNumber,
                                       t.DYNumber,
                                       t.HSNumber
                                   } into g
                                   select g.Key).Count();

                    PLMPProduceList data = new PLMPProduceList()
                    {
                        CountyID = CountryID,
                        NeighborhoodsID = NeighborhoodsID,
                        PLID = id,
                        MPType = Enums.TypeInt.Residence,
                        MPTypeName = "住宅门牌",
                        PLMPProduceComplete = PLMPProduceComplete,
                        PLMPProduceCompleteName = PLMPProduceComplete == Enums.Complete.Yes ? "未制作" : "已制作",
                        ResidenceName = string.Join(",", ResidenceName),
                        SBDW = string.Join(",", SBDW),
                        Applicant = string.Join(",", Applicant),
                        ApplicantPhone = string.Join(",", ApplicantPhone),
                        MPBZTime = string.Join(",", BZTime),
                        MPCount = lzCount + dyCount + hsCount
                    };
                    datas.Add(data);
                }

                var DLPLIDs = (from t in dbContext.MPOfRoad
                               where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.MPProduceComplete == PLMPProduceComplete
                               group t by t.PLID into g
                               select g.Key).ToList();
                foreach (var id in DLPLIDs)
                {
                    var rt = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.MPProduceComplete == PLMPProduceComplete).Where(t => t.PLID == id);
                    var CountryID = rt.Select(t => t.CountyID).Distinct().First();
                    var NeighborhoodsID = rt.Select(t => t.NeighborhoodsID).Distinct().First();
                    var SBDW = rt.Select(t => t.SBDW).Distinct().ToList();
                    var RoadName = rt.Select(t => t.ResidenceName).Distinct().ToList();
                    var Applicant = rt.Select(t => t.Applicant).Distinct().ToList();
                    var ApplicantPhone = rt.Select(t => t.ApplicantPhone).Distinct().ToList();
                    var BZTime = rt.Select(t => t.BZTime.ToString()).Distinct().ToList();

                    var Count = (from t in rt
                                 group t by new
                                 {
                                     t.RoadName,
                                     t.MPNumber
                                 } into g
                                 select g.Key).Count();
                    PLMPProduceList data = new PLMPProduceList()
                    {
                        CountyID = CountryID,
                        NeighborhoodsID = NeighborhoodsID,
                        PLID = id,
                        MPType = Enums.TypeInt.Residence,
                        MPTypeName = "道路门牌",
                        PLMPProduceComplete = PLMPProduceComplete,
                        PLMPProduceCompleteName = PLMPProduceComplete == Enums.Complete.NO ? "未制作" : "已制作",
                        RoadName = string.Join(",", RoadName),
                        SBDW = string.Join(",", SBDW),
                        Applicant = string.Join(",", Applicant),
                        ApplicantPhone = string.Join(",", ApplicantPhone),
                        MPBZTime = string.Join(",", BZTime),
                        MPCount = Count
                    };
                    datas.Add(data);
                }

                var NCPLIDs = (from t in dbContext.MPOfCountry
                               where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && t.MPProduceComplete == PLMPProduceComplete
                               group t by t.PLID into g
                               select g.Key).ToList();
                foreach (var id in NCPLIDs)
                {
                    var rt = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => t.MPProduce == Enums.MPProduce.Yes).Where(t => t.MPProduceComplete == PLMPProduceComplete).Where(t => t.PLID == id);
                    var CountryID = rt.Select(t => t.CountyID).Distinct().First();
                    var NeighborhoodsID = rt.Select(t => t.NeighborhoodsID).Distinct().First();
                    var SBDW = rt.Select(t => t.SBDW).Distinct().ToList();
                    var ViligeName = rt.Select(t => t.ViligeName).Distinct().ToList();
                    var Applicant = rt.Select(t => t.Applicant).Distinct().ToList();
                    var ApplicantPhone = rt.Select(t => t.ApplicantPhone).Distinct().ToList();
                    var BZTime = rt.Select(t => t.BZTime.ToString()).Distinct().ToList();

                    var Count = (from t in rt
                                 group t by new
                                 {
                                     t.ViligeName,
                                     t.MPNumber
                                 } into g
                                 select g.Key).Count();
                    PLMPProduceList data = new PLMPProduceList()
                    {
                        CountyID = CountryID,
                        NeighborhoodsID = NeighborhoodsID,
                        PLID = id,
                        MPType = Enums.TypeInt.Residence,
                        MPTypeName = "农村门牌",
                        PLMPProduceComplete = PLMPProduceComplete,
                        PLMPProduceCompleteName = PLMPProduceComplete == Enums.Complete.NO ? "未制作" : "已制作",
                        ViligeName = string.Join(",", ViligeName),
                        SBDW = string.Join(",", SBDW),
                        Applicant = string.Join(",", Applicant),
                        ApplicantPhone = string.Join(",", ApplicantPhone),
                        MPBZTime = string.Join(",", BZTime),
                        MPCount = Count
                    };
                    datas.Add(data);
                }

                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<PLMPProduceList>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var q = datas.Where(where.Compile());

                count = q.Count();
                var result = q.OrderByDescending(t => t.MPBZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                return new Dictionary<string, object> {
                   { "Data",result},
                   { "Count",count}
                };
            }
        }

        public static List<PLMPHZ> ProducePLMP(List<PLMPProduceList> mpLists)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var plmphzs = new List<PLMPHZ>();
                foreach (var mp in mpLists)
                {
                    if (mp.MPType == Enums.TypeInt.Residence)
                    {
                        var query = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => t.MPProduceComplete == Enums.Complete.NO).Where(t => t.PLID == mp.PLID).ToList();
                        foreach (var q in query)
                        {
                            q.MPProduceComplete = Enums.Complete.Yes;
                            q.MPProduceCompleteTime = DateTime.Now.Date;
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
                    else if (mp.MPType == Enums.TypeInt.Road)
                    {
                        var query = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => t.MPProduceComplete == Enums.Complete.NO).Where(t => t.PLID == mp.PLID).ToList();
                        foreach (var q in query)
                        {
                            q.MPProduceComplete = Enums.Complete.Yes;
                            q.MPProduceCompleteTime = DateTime.Now.Date;
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
                    else if (mp.MPType == Enums.TypeInt.Country)
                    {
                        var query = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.AddType == Enums.MPAddType.PL).Where(t => t.MPProduceComplete == Enums.Complete.NO).Where(t => t.PLID == mp.PLID).ToList();
                        foreach (var q in query)
                        {
                            q.MPProduceComplete = Enums.Complete.Yes;
                            q.MPProduceCompleteTime = DateTime.Now.Date;
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
}