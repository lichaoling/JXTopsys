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
                var mpOfRoad = BaseUtils.DataFilterWithTown<MPOfRoad>(dbContext.MPOfRoad);
                var mpOfCountry = BaseUtils.DataFilterWithTown<MPOfCountry>(dbContext.MPOfCountry);

                int count = 0;

                var all = (from t in mpOfRoad
                           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.LXProduceID)
                           group t by new { t.LXProduceID, t.MPProduceUser, t.MPProduceTime } into g
                           select new ProducedLXMPList
                           {
                               MPType = Enums.MPTypeCh.Road,
                               LXProduceID = g.Key.LXProduceID,
                               MPProduceUser = g.Key.MPProduceUser,
                               MPProduceTime = g.Key.MPProduceTime
                           }).Concat(
                    from t in mpOfCountry
                    where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.LXProduceID)
                    group t by new { t.LXProduceID, t.MPProduceUser, t.MPProduceTime } into g
                    select new ProducedLXMPList
                    {
                        MPType = Enums.MPTypeCh.Country,
                        LXProduceID = g.Key.LXProduceID,
                        MPProduceUser = g.Key.MPProduceUser,
                        MPProduceTime = g.Key.MPProduceTime
                    });

                count = all.Count();
                var data = all.OrderBy(t => t.MPProduceTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
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

                all = BaseUtils.DataFilterWithTown<NotProducedLXMPList>(all);

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
                var LXProduceID = DateTime.Now.ToString("yyyyMMddhhmmss");
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
                        query.MPProduceTime = DateTime.Now;

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
                        query.MPProduceTime = DateTime.Now;

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
                var mpOfRoad =BaseUtils.DataFilterWithTown<MPOfRoad>( dbContext.MPOfRoad);
                var mpOfResidence = BaseUtils.DataFilterWithTown<MPOfResidence>(dbContext.MPOfResidence); 
                var mpOfCountry = BaseUtils.DataFilterWithTown<MPOfCountry>(dbContext.MPOfCountry); 
                               
                int count = 0;
                var all = (from t in mpOfRoad
                           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                           group t by new { t.PLProduceID, t.MPProduceUser, t.MPProduceTime } into g
                           select new ProducedPLMPList
                           {
                               MPType = Enums.MPTypeCh.Road,
                               PLProduceID = g.Key.PLProduceID,
                               MPProduceUser = g.Key.MPProduceUser,
                               MPProduceTime = g.Key.MPProduceTime
                           }).Concat(
                    from t in mpOfCountry
                    where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                    group t by new { t.PLProduceID, t.MPProduceUser, t.MPProduceTime } into g
                    select new ProducedPLMPList
                    {
                        MPType = Enums.MPTypeCh.Country,
                        PLProduceID = g.Key.PLProduceID,
                        MPProduceUser = g.Key.MPProduceUser,
                        MPProduceTime = g.Key.MPProduceTime
                    }).Concat(
                    from t in mpOfResidence
                    where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.LX && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                    group t by new { t.PLProduceID, t.MPProduceUser, t.MPProduceTime } into g
                    select new ProducedPLMPList
                    {
                        MPType = Enums.MPTypeCh.Residence,
                        PLProduceID = g.Key.PLProduceID,
                        MPProduceUser = g.Key.MPProduceUser,
                        MPProduceTime = g.Key.MPProduceTime
                    });
                #region 注释
                //var lz = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLProduceID = t.PLProduceID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              MPBZTime = t.BZTime,
                //              MPProduceTime = t.MPProduceTime,
                //          }).Distinct();
                //var lzC = (from t in lz
                //           group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = g.Key.PLProduceID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count() * 2,
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               MPBZTime = g.Key.MPBZTime
                //           }).ToList();
                //var dy = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLProduceID = t.PLProduceID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              DYNumber = t.DYNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              MPBZTime = t.BZTime
                //          }).Distinct();
                //var dyC = (from t in dy
                //           group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.MPBZTime } into g
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = g.Key.PLProduceID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               MPBZTime = g.Key.MPBZTime
                //           }).ToList();
                //var hsC = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //           group t by new { t.PLProduceID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = g.Key.PLProduceID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               MPBZTime = g.Key.BZTime
                //           }).ToList();

                //var xqs = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //           select new ProducedPLMPList
                //           {
                //               PLProduceID = t.PLProduceID,
                //               SBDW = t.SBDW,
                //               ResidenceName = t.ResidenceName,
                //               Applicant = t.Applicant,
                //               ApplicantPhone = t.ApplicantPhone,
                //               MPBZTime = t.BZTime
                //           }).Distinct().ToList();
                //List<ProducedPLMPList> data = new List<ProducedPLMPList>();
                //foreach (var xq in xqs)
                //{
                //    var a = lzC.Where(t => t.PLProduceID == xq.PLProduceID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var b = dyC.Where(t => t.PLProduceID == xq.PLProduceID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var c = hsC.Where(t => t.PLProduceID == xq.PLProduceID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    xq.MPCount = a + b + c;
                //    data.Add(xq);
                //}

                //var dl = (from t in mpOfRoad
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          group t by new { t.PLProduceID, t.SBDW, t.RoadName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                //          select new ProducedPLMPList
                //          {
                //              PLProduceID = g.Key.PLProduceID,
                //              SBDW = g.Key.SBDW,
                //              RoadName = g.Key.RoadName,
                //              MPCount = g.Count(),
                //              Applicant = g.Key.Applicant,
                //              ApplicantPhone = g.Key.ApplicantPhone,
                //              MPBZTime = g.Key.BZTime
                //          }).ToList();

                //data.AddRange(dl);
                //var nc = (from t in mpOfCountry
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && !string.IsNullOrEmpty(t.PLProduceID)
                //          group t by new { t.PLProduceID, t.SBDW, t.ViligeName, t.Applicant, t.ApplicantPhone, t.BZTime } into g
                //          select new ProducedPLMPList
                //          {
                //              PLProduceID = g.Key.PLProduceID,
                //              SBDW = g.Key.SBDW,
                //              ViligeName = g.Key.ViligeName,
                //              MPCount = g.Count(),
                //              Applicant = g.Key.Applicant,
                //              ApplicantPhone = g.Key.ApplicantPhone,
                //              MPBZTime = g.Key.BZTime
                //          }).ToList();
                //data.AddRange(nc);
                #endregion
                count = all.Count();
                var result = all.OrderByDescending(t => t.MPProduceTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
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
                var mpOfRoad = BaseUtils.DataFilterWithTown<MPOfRoad>(dbContext.MPOfRoad);
                var mpOfResidence = BaseUtils.DataFilterWithTown<MPOfResidence>(dbContext.MPOfResidence);
                var mpOfCountry = BaseUtils.DataFilterWithTown<MPOfCountry>(dbContext.MPOfCountry);

                int count = 0;
                #region 注释
                //var lz = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLID = t.PLID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              CreateTime = t.CreateTime
                //          }).Distinct();
                //var lzC = (from t in lz
                //           group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                //           select new NotProducedPLMPList
                //           {
                //               PLID = g.Key.PLID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count() * 2,
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               CreateTime = g.Key.CreateTime
                //           }).ToList();
                //var dy = (from t in mpOfResidence
                //          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //          select new
                //          {
                //              PLID = t.PLID,
                //              SBDW = t.SBDW,
                //              ResidenceName = t.ResidenceName,
                //              LZNumber = t.LZNumber,
                //              DYNumber = t.DYNumber,
                //              Applicant = t.Applicant,
                //              ApplicantPhone = t.ApplicantPhone,
                //              CreateTime = t.CreateTime
                //          }).Distinct();
                //var dyC = (from t in dy
                //           group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                //           select new NotProducedPLMPList
                //           {
                //               PLID = g.Key.PLID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               CreateTime = g.Key.CreateTime
                //           }).ToList();
                //var hsC = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //           group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                //           select new NotProducedPLMPList
                //           {
                //               PLID = g.Key.PLID,
                //               SBDW = g.Key.SBDW,
                //               ResidenceName = g.Key.ResidenceName,
                //               MPCount = g.Count(),
                //               Applicant = g.Key.Applicant,
                //               ApplicantPhone = g.Key.ApplicantPhone,
                //               CreateTime = g.Key.CreateTime
                //           }).ToList();
                //var xqs = (from t in mpOfResidence
                //           where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                //           select new NotProducedPLMPList
                //           {
                //               PLID = t.PLID,
                //               SBDW = t.SBDW,
                //               ResidenceName = t.ResidenceName,
                //               Applicant = t.Applicant,
                //               ApplicantPhone = t.ApplicantPhone,
                //               CreateTime = t.CreateTime
                //           }).Distinct().ToList();

                //List<NotProducedPLMPList> data = new List<NotProducedPLMPList>();
                //foreach (var xq in xqs)
                //{
                //    var a = lzC.Where(t => t.PLID == xq.PLID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var b = dyC.Where(t => t.PLID == xq.PLID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    var c = hsC.Where(t => t.PLID == xq.PLID).Where(t => t.ResidenceName == xq.ResidenceName).Where(t => t.SBDW == xq.SBDW).Select(t => t.MPCount).FirstOrDefault();
                //    xq.MPCount = a + b + c;
                //    data.Add(xq);
                //}
                #endregion

                List<NotProducedPLMPList> data = new List<NotProducedPLMPList>();
                var zz = (from t in mpOfResidence
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                          group t by new { t.PLID, t.SBDW, t.ResidenceName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                          select new NotProducedPLMPList
                          {
                              PLID = g.Key.PLID,
                              MPType = Enums.MPTypeCh.Residence,
                              SBDW = g.Key.SBDW,
                              ResidenceName = g.Key.ResidenceName,
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              CreateTime = g.Key.CreateTime,
                              MPCount = g.Count() + (from s in g group s by new { s.LZNumber, s.DYNumber } into h select g).Count() + (from s in g group s by new { s.LZNumber } into h select g).Count() * 2,
                          }).ToList();
                data.AddRange(zz);

                var dl = (from t in mpOfRoad
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                          group t by new { t.PLID, t.SBDW, t.RoadName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                          select new NotProducedPLMPList
                          {
                              PLID = g.Key.PLID,
                              MPType = Enums.MPTypeCh.Road,
                              SBDW = g.Key.SBDW,
                              RoadName = g.Key.RoadName,
                              MPCount = g.Count(),
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              CreateTime = g.Key.CreateTime
                          }).ToList();
                data.AddRange(dl);

                var nc = (from t in mpOfCountry
                          where t.State == Enums.UseState.Enable && t.AddType == Enums.MPAddType.PL && t.MPProduce == Enums.MPProduce.Yes && string.IsNullOrEmpty(t.PLProduceID)
                          group t by new { t.PLID, t.SBDW, t.ViligeName, t.Applicant, t.ApplicantPhone, t.CreateTime } into g
                          select new NotProducedPLMPList
                          {
                              PLID = g.Key.PLID,
                              MPType = Enums.MPTypeCh.Country,
                              SBDW = g.Key.SBDW,
                              ViligeName = g.Key.ViligeName,
                              MPCount = g.Count(),
                              Applicant = g.Key.Applicant,
                              ApplicantPhone = g.Key.ApplicantPhone,
                              CreateTime = g.Key.CreateTime
                          }).ToList();
                data.AddRange(nc);

                count = data.Count();
                var result = data.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
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
                var PLProduceID = DateTime.Now.ToString("yyyyMMddhhmmss");
                var MPProduceTime = DateTime.Now;
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
                                      group t by new { t.MPNumber, t.MPSize } into g
                                      select new PLMPSL
                                      {
                                          Number = g.Key.MPNumber,
                                          MPSize = g.Key.MPSize,
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